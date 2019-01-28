using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Webserver;
using Webserver.SendFaxModels;
using Webserver.Contexts;


namespace Webserver.Controllers
{
    
    [Route("api/[controller]"), RequireHttps]
    public class SendFaxController : Controller
    {

        
        private SendFaxDbInjection dbContext;

        public SendFaxController() {
            string connectionString = "Data Source=127.0.0.1,1433;Initial Catalog=ivrs;User Id=user;Password=1234";
            dbContext = SendFaxDbContextFactory.Create(connectionString);
        }

      
        // POST api/values
        [HttpPost]
        
        public ActionResult Post()
        {
            string json_string;
            string messagesequence;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                json_string = reader.ReadToEnd();
            }
            
             //Extract msgSeqNo
            string temp;
            int index = json_string.IndexOf(@"""msgSeqNo"":");
            temp = json_string.Substring (index + @"""msgSeqNo"":".Length);
            string[] tmp2 = temp.Split(',');
            temp = tmp2[0].Trim();
            messagesequence = temp.Substring(1, temp.Length-2);
            //Check for authorization first
            string authHeader = Request.Headers["Authorization"];  
            string encodedUsernamePassword;
            int requestId; //for later use
            
            try{
            if (authHeader != null && authHeader.StartsWith("Basic")) {
            //Extract credentials
            encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            } else {
            //Handle what happens if that isn't the case
            throw new Exception("The authorization header is either empty or isn't Basic.");
                }
             }
             catch (Exception error)
             {
                SendFax_Response responsemessage = new SendFax_Response (messagesequence, "F", "403", error.ToString());
                return StatusCode(403, responsemessage); 
             }

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
        
            int seperatorIndex = usernamePassword.IndexOf(':'); //Assume this will work
            string username = usernamePassword.Substring(0, seperatorIndex);
            string password = usernamePassword.Substring(seperatorIndex + 1);
    

            if (username!= "sats3cx" || password!= "as1atel$123")
            {
                SendFax_Response responsemessage = new SendFax_Response (messagesequence, "F", "403", "Authorization failure");
                return StatusCode(403, responsemessage);
            }

    
            //Step 1: Deserialize JSON request body
           ConsignmentDetails record = new ConsignmentDetails();

           try{ 
           record = JsonConvert.DeserializeObject<ConsignmentDetails>(json_string);}
           catch (JsonException error) {
           
           string error_description = (error.ToString().Length <= 256 ? error.ToString() : error.ToString().Substring(0, 256));
           
            SendFax_Response responsemessage = new SendFax_Response (messagesequence, "F", "400", error_description);
            return StatusCode(400, responsemessage);
           }


            //Step 2: Database access and operations
        try{
            //extract contact nos.
            string[] contacts = record.contactNo.Split(',');
            for(int i = 0; i < contacts.Length; i++)
            {
                contacts[i] = contacts[i].Trim();
            }
            //2.1 Create and Add database objects to database - Suggestion: use a try-catch block for this step

            RequestMain_Details primarytable_entry = new RequestMain_Details(Convert.ToInt32(record.msgSeqNo),record.datetime, record.awbPfx, record.awbNum,record.consigneeName, record.totPieces, record.totWeight, record.origin, record.destination, "N", contacts.Length, "N", record.awbDetails.Count());
            dbContext.PrimaryTableDBObject.Add(primarytable_entry);
            dbContext.SaveChanges();

            //Populate secondary database table
            
            //Grab the highest value using the Max() method
            requestId = dbContext.PrimaryTableDBObject.Max(p => p.requestID);
              
            PhNoDetails[] secondarytable_entries= new PhNoDetails[contacts.Length];
            
            for(int i = 0; i < contacts.Length; i++)
            {
                secondarytable_entries[i] = new PhNoDetails(requestId, Convert.ToInt32(contacts[i]), 0, "MN", "N") ;
            }

            for(int i = 0; i < contacts.Length; i++)
            {
                dbContext.SecondaryTableDBObject.Add(secondarytable_entries[i]);
                dbContext.SaveChanges();            
            }
           
           //populate tertiary database table
           AwbDetails [] tertiarytable_entries = new AwbDetails[record.awbDetails.Count()];
           
           {
            int i = 0;
            string pdfContentPath;
            

            foreach (AWBDetails consignmentpart in record.awbDetails)
             {  
                //Create dbinjection record and create pdf file
                pdfContentPath = string.Format(@"{0}-{1}-{2}.pdf", requestId.ToString(),record.awbPfx,i.ToString());
                tertiarytable_entries[i] = new AwbDetails(requestId, consignmentpart.flightCar, consignmentpart.flightNum, consignmentpart.flightDate, consignmentpart.fsuStatus, consignmentpart.pieces, consignmentpart.weight,pdfContentPath);
                
                //decode pdf base-64 encoded bytes after removing <>
                using (FileStream stream = System.IO.File.Create("c:\\IVRS\\pdf\\"+pdfContentPath))
                 {
                    string data = consignmentpart.pdfContent.Substring(1, consignmentpart.pdfContent.Length-2);
                    if (data.Length==0)
                    {
                        throw (new Exception("no pfd content found!"));
                    }
                    //Console.WriteLine(data); 
                    byte [] pdfFile = Convert.FromBase64String(data);
                    stream.Write(pdfFile, 0, pdfFile.Length);
                    }
                                  
                i++;
                }
            }

            for(int i = 0; i < record.awbDetails.Count(); i++)
            {
            dbContext.TertiaryTableDBObject.Add(tertiarytable_entries[i]);
            dbContext.SaveChanges();
            }
        }
        catch (Exception error) {
        string error_description = (error.ToString().Length <= 256 ? error.ToString() : error.ToString().Substring(0, 256));    
        SendFax_Response responsemessage = new SendFax_Response (messagesequence, "F", "500", error_description);
        return StatusCode(500, responsemessage);
        }        
            //Step 3 Generate response code if successful
            SendFax_Response successmessage = new SendFax_Response (record.msgSeqNo, "S", "", "");             
            //update cosysIntAck field and save database changes
            var currentTableRecord = dbContext.PrimaryTableDBObject.Single(a => a.requestID == requestId);
            currentTableRecord.cosysIntAck = "S";
            dbContext.SaveChanges();

            return StatusCode(200, successmessage);
        }
     
    }

} 


  