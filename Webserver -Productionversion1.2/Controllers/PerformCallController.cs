using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Webserver;
using Webserver.PerformCallModels;
using Webserver.Contexts;


namespace Webserver.Controllers
{
    
    [Route("api/[controller]"), RequireHttps]
    public class PerformCallController : Controller
    {
        
        private PerformCallDbInjection dbContext;

        public PerformCallController() {
            string connectionString = "Data Source=127.0.0.1,1433;Initial Catalog=ivrs;User Id=user;Password=1234" ;
            dbContext = PerformCallDbContextFactory.Create(connectionString);
        }

        
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

            
            //Check for authorization
            string authHeader = Request.Headers["Authorization"]; 
            string encodedUsernamePassword;
            int requestId; //used later
            
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
                PerformCall_Response responsemessage = new PerformCall_Response (messagesequence, "F", "403", error.ToString());
                return StatusCode(403, responsemessage); 
             }

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
        
            int seperatorIndex = usernamePassword.IndexOf(':'); //Assume this will work
            string username = usernamePassword.Substring(0, seperatorIndex);
            string password = usernamePassword.Substring(seperatorIndex + 1);
    

            if (username!= "sats3cx" || password!= "as1atel$123")
            {
                PerformCall_Response responsemessage = new PerformCall_Response (messagesequence, "F", "403", "Authorization failure");
                return StatusCode(403, responsemessage);
            }
    
            //Step 1: Deserialize JSON request body
          
           ConsignmentDetails record = new ConsignmentDetails();

           try{ 
           record = JsonConvert.DeserializeObject<ConsignmentDetails>(json_string);}
           catch (JsonException error) {
           
           string error_description = (error.ToString().Length <= 256 ? error.ToString() : error.ToString().Substring(0, 256));
           
            PerformCall_Response responsemessage = new PerformCall_Response (messagesequence, "F", "400", error_description);
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

            Sats_NotificationRequest_Details primarytable_entry = new Sats_NotificationRequest_Details(Convert.ToInt64(record.msgSeqNo),record.datetime, record.awbPfx, record.awbNum,record.consigneeName, record.totPieces, record.totWeight, record.origin, record.destination, contacts.Length, "IP", record.awbDetails.Count());
            dbContext.PrimaryTableDBObject.Add(primarytable_entry);
            dbContext.SaveChanges();

            
            //Grab the highest value using the Max() method
            requestId = dbContext.PrimaryTableDBObject.Max(p => p.ID);
           
           //population tertiary database table before secondary table
           Sats_ConsignmentParts_Details[] tertiarytable_entries = new Sats_ConsignmentParts_Details[record.awbDetails.Count()];
           
           {
            int i = 0;
            foreach (AWBDetails consignmentpart in record.awbDetails)
             {
                tertiarytable_entries[i] = new Sats_ConsignmentParts_Details(requestId, Convert.ToInt64(record.msgSeqNo), consignmentpart.flightCar, consignmentpart.flightNum, consignmentpart.flightDate, consignmentpart.fsuStatus, consignmentpart.pieces, consignmentpart.weight, i+1);
                i++;
                }
           }

            for(int i = 0; i < record.awbDetails.Count(); i++)
            {
            dbContext.TertiaryTableDBObject.Add(tertiarytable_entries[i]);
            dbContext.SaveChanges();
            }

            //Populate secondary database table
            Sats_Outbound_CallNotification_Records[] secondarytable_entries= new Sats_Outbound_CallNotification_Records[contacts.Length];
            
            for(int i = 0; i < contacts.Length; i++)
            {
                secondarytable_entries[i] = new Sats_Outbound_CallNotification_Records(requestId, record.msgSeqNo, contacts[i], i+1, 0) ;
            }

            for(int i = 0; i < contacts.Length; i++)
            {
            dbContext.SecondaryTableDBObject.Add(secondarytable_entries[i]);
            dbContext.SaveChanges();
            }

            //update the isPresentContact field for the first Contact to true
            var currentTableRecord = dbContext.SecondaryTableDBObject.Single(a => a.ID == requestId && a.contactID == 1);
            currentTableRecord.isPresentContact = 1;
            dbContext.SaveChanges();
        }
        catch (Exception error) {
        string error_description = (error.ToString().Length <= 256 ? error.ToString() : error.ToString().Substring(0, 256));    
        
        PerformCall_Response responsemessage = new PerformCall_Response (messagesequence, "F", "500", error_description);
        return StatusCode(500, responsemessage);
    
            }        

            PerformCall_Response successmessage = new PerformCall_Response (record.msgSeqNo, "S", "", "");             
            return StatusCode(200, successmessage);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

 
  