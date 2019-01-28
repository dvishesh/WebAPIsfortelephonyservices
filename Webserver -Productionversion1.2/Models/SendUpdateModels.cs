using System ; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Webserver.SendUpdateModels {

     public class SendUpdate_Receive
{
    public string faxOrCallUpdateRequest {get;set;}
    public int requestID {get;set;}
    public string status {get;set;}
}
 

// domain models
[Table("SATS_NOTIFICATIONREQUEST_DETAILS")]  
    public class SendUpdate_Details_Call {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID")]
        public int ID {get;set;}
        [Column("msgSeqNum")]
        public long msgSeqNo {get;set;}
        [Column("datetime")]
        public DateTime datetime {get;set;}
        [Column("awbPfx")]
        public string awbPfx {get;set;}
        [Column("awbNum")]
        public string awbNum {get;set;}
    }

    [Table("RequestMain")]  
    public class SendUpdate_Details_Fax {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("requestID")]
        public int ID {get;set;}
        [Column("msgSeqNo")]
        public int msgSeqNo {get;set;}
        [Column("dateTm")]
        public DateTime datetime {get;set;}
        [Column("awbPfx")]
        public string awbPfx {get;set;}
        [Column("awbNum")]
        public string awbNum {get;set;}
    }
}
