using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Webserver;
using Webserver.EnquireAwbInboundModels;
using Webserver.Contexts;


namespace Webserver.Controllers
{
    // api expects unique ID and status value, retrieves remaining details from primary database table
    [Route("api/[controller]")]
    public class EnquireAwbInboundController : Controller
    {
        private EnquireAwbDbInjection awbenquiredbcontext;
      
        public EnquireAwbInboundController() {
            string connectionString = "Data Source=127.0.0.1,1433;Initial Catalog=ivrs;User Id=user;Password=1234" ;
            awbenquiredbcontext = EnquireAwbDbContextFactory.Create(connectionString);
        }

        // POST api/values
        [HttpPost]
        
        public ActionResult Post([FromBody] EnquireAwbInbound_Receive inboundcallrequest)
        {   
         try{
            string cosysboundrequestbody;
            HttpClient client =  new HttpClient(); 
            
            //generate datetime and msgSeqNo for inboundcallrequest
            long inboundcallrequestId = awbenquiredbcontext.PrimaryTableDBObject.Max(p => p.msgSeqNo);
            inboundcallrequestId++;

            DateTime tmp = DateTime.Now;
            string inboundcallrequestdatetime = tmp.ToString("s");
            
            //create the message
            cosysboundrequestbody  = string.Format(@"{{""msgSeqNo"":""{0}"",""datetime"":""{1}"", ""awbPfx"":""{2}"", ""awbNum"":""{3}""}}", inboundcallrequestId, inboundcallrequestdatetime, inboundcallrequest.awbPfx, inboundcallrequest.awbNum);
                     
            //print requestbody to confirm it's been properly generated
            //Console.WriteLine(cosysboundrequestbody); 
            
            
            //evoke external web api and send update status
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization","Basic c2F0czNjeDphczFhdGVsJDEyMw==");            
            HttpResponseMessage cosysresponse = CreateStatusUpdateAsync(cosysboundrequestbody,client).GetAwaiter().GetResult();
            
            //parse response and check for status first
            var cosysresponse_content = cosysresponse.Content.ReadAsStringAsync().Result;
            AwbEnquireConsignmentDetails cosysresponse_content_object = new AwbEnquireConsignmentDetails();
            cosysresponse_content_object = JsonConvert.DeserializeObject<AwbEnquireConsignmentDetails>(cosysresponse_content);
            
            if (cosysresponse_content_object.status== "S")
            //populate the necessary tables in case of successful response
                {
                //populate the main details table    
                    AwbEnquireRequest_MainDetails primarytable_entry = new AwbEnquireRequest_MainDetails(inboundcallrequestdatetime,cosysresponse_content_object.status, cosysresponse_content_object.errorCode, cosysresponse_content_object.errorDesc, inboundcallrequest.awbPfx.ToString() , inboundcallrequest.awbNum.ToString(),cosysresponse_content_object.consigneeName, cosysresponse_content_object.totPieces, cosysresponse_content_object.totWeight, cosysresponse_content_object.origin, cosysresponse_content_object.destination, cosysresponse_content_object.awbDetails.Count());
                    awbenquiredbcontext.PrimaryTableDBObject.Add(primarytable_entry);
                    awbenquiredbcontext.SaveChanges();

                //populate the consignment details table
                    AwbEnquireRequest_ConsignmentParts_Details[] secondarytable_entries = new AwbEnquireRequest_ConsignmentParts_Details[cosysresponse_content_object.awbDetails.Count()];
           
                    {
                    int i = 0;
                    foreach (AwbEnquireAWBDetails consignmentpart in cosysresponse_content_object.awbDetails)
                        {
                        secondarytable_entries[i] = new AwbEnquireRequest_ConsignmentParts_Details(inboundcallrequestId, consignmentpart.flightCar, consignmentpart.flightNum, consignmentpart.flightDate, consignmentpart.fsuStatus, consignmentpart.pieces, consignmentpart.weight, i+1);
                        i++;
                        }
                    }

                    for(int i = 0; i < cosysresponse_content_object.awbDetails.Count(); i++)
                    {
                        awbenquiredbcontext.SecondaryTableDBObject.Add(secondarytable_entries[i]);
                        awbenquiredbcontext.SaveChanges();
                    }
                    return Ok(inboundcallrequestId);
                }
            else if (cosysresponse_content_object.status== "F")

                {
                    AwbEnquireRequest_MainDetails primarytable_entry = new AwbEnquireRequest_MainDetails(inboundcallrequestdatetime,cosysresponse_content_object.status, cosysresponse_content_object.errorCode, cosysresponse_content_object.errorDesc, inboundcallrequest.awbPfx.ToString(), inboundcallrequest.awbNum.ToString());
                    awbenquiredbcontext.PrimaryTableDBObject.Add(primarytable_entry);
                    awbenquiredbcontext.SaveChanges();
                    return BadRequest(inboundcallrequestId);
                }

            //redundant return
            return Ok();
            }
            catch (Exception error)
            {
                // (in case of failure at any point - catch exception and return BadRequest();
                return BadRequest(error.ToString());
            }
            
        }

            static async Task<HttpResponseMessage> CreateStatusUpdateAsync(string requestbody, HttpClient client)
        {
            StringContent httpcontent = new StringContent(requestbody, Encoding.UTF8, "application/json");
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

             HttpResponseMessage response = await client.PostAsync("http://10.10.150.102:5500/api/cosysawbenquiretest", httpcontent);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}

 
  