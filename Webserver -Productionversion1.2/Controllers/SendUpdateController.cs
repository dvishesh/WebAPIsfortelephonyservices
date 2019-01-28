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
using Webserver.PerformCallModels;
using Webserver.SendFaxModels;
using Webserver.SendUpdateModels;
using Webserver.Contexts;


namespace Webserver.Controllers
{
    // api expects unique ID and status value, retrieves remaining details from primary database table
    [Route("api/[controller]")]
    public class SendUpdateController : Controller
    {
        
        private SendUpdateDbContext sendUpdateDbContext;
        
       
        public SendUpdateController() {
            string connectionString = "Data Source=127.0.0.1,1433;Initial Catalog=ivrs;User Id=user;Password=1234";
            sendUpdateDbContext = SendUpdateDbContextFactory.Create(connectionString);
            
        }

        // POST api/values
        [HttpPost]
        
        public ActionResult Post([FromBody] SendUpdate_Receive statusupdate)
        {   
            try{
            string requestbody = "";
            HttpClient client =  new HttpClient(); 
        
            //retrieve consignment details from database
            switch (statusupdate.faxOrCallUpdateRequest)
            {
            case "CALL": var currentTableRecordCall = sendUpdateDbContext.CallDBObject.Single(a => a.ID == statusupdate.requestID);    
            if (statusupdate.status == "SUCCESS")
            statusupdate.status = "S";
            if (statusupdate.status == "FAILURE")
            statusupdate.status = "F";

            requestbody  = string.Format(@"{{""msgSeqNo"":""{0}"",""datetime"":""{1}"", ""awbPfx"":""{2}"", ""awbNum"":""{3}"", ""ackFlg"":""{4}""}}", currentTableRecordCall.msgSeqNo, currentTableRecordCall.datetime.ToString("s"), currentTableRecordCall.awbPfx, currentTableRecordCall.awbNum, statusupdate.status);
            break;
            case "FX": var currentTableRecordFax = sendUpdateDbContext.FaxDBObject.Single(a => a.ID == statusupdate.requestID);    
            requestbody  = string.Format(@"{{""msgSeqNo"":""{0}"",""datetime"":""{1}"", ""awbPfx"":""{2}"", ""awbNum"":""{3}"", ""ackFlg"":""{4}""}}", currentTableRecordFax.msgSeqNo, currentTableRecordFax.datetime.ToString("s"), currentTableRecordFax.awbPfx, currentTableRecordFax.awbNum, statusupdate.status);
            break;
            }
            
            //evoke external web api and send update status
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization","Basic c2F0czNjeDphczFhdGVsJDEyMw==");            
            HttpResponseMessage cosysresponse = CreateStatusUpdateAsync(requestbody,client).GetAwaiter().GetResult();
            
            
            //check response for acknowledgement
            var cosysresponse_content = cosysresponse.Content.ReadAsStringAsync().Result;
            PerformCall_Response cosysresponse_content_object = new PerformCall_Response();
            cosysresponse_content_object = JsonConvert.DeserializeObject<PerformCall_Response>(cosysresponse_content);
            
            if (cosysresponse_content_object.status== "S")
            //think about updating database fields if need be
            return Ok();
            else
            return BadRequest();
            }
            catch (Exception error)
            {
                return StatusCode(500, error.ToString());
            }
        }

            static async Task<HttpResponseMessage> CreateStatusUpdateAsync(string requestbody, HttpClient client)
        {
            StringContent httpcontent = new StringContent(requestbody, Encoding.UTF8, "application/json");
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("http://10.10.150.102:5500/api/cosyssendstatustest", httpcontent);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }


}

 
  