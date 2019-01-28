using System ; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Webserver.SendFaxModels {

     public class AWBDetails
{
    public string flightCar {get;set;}
    public string flightNum {get; set;}
    public string flightDate {get; set;}
    public int pieces {get; set;}
    public double weight {get;set;}
    public string fsuStatus {get;set;}
    public string pdfContent {get;set;}
}
 

    public class ConsignmentDetails {
        public string msgSeqNo { get; set; } 
        public string datetime { get; set; }
        public string contactNo {get; set;}
        public string awbPfx {get;set;}
        public string awbNum {get; set;}
        public int totPieces {get;set;}
        public double totWeight {get;set;}
        public string origin {get;set;}
        public string destination {get;set;}
        public string consigneeName {get;set;}
        public List<AWBDetails> awbDetails {get;set;}
        
        //Constructor
        public ConsignmentDetails()
        {
            this.awbDetails = new List<AWBDetails>(); 
        }
         
    }

    public class SendFax_Response {
        public string msgSeqNo {get;set;}
        public string status {get;set;}
        public string errorCode {get;set;}
        public string errorDesc {get;set;}

//Constructor
        public SendFax_Response (string msgSeqNo, string status, string errorCode, string errorDesc)
        {
            this.msgSeqNo = msgSeqNo;
            this.status = status;
            this.errorCode = errorCode;
            this.errorDesc = errorDesc;
        }
    }


//Domain models


[Table("RequestMain")]
public class RequestMain_Details
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("requestID")]
        public int requestID {get;set;}

        [Column("msgSeqNo")]     
        public  int msgSeqNo { get; set; } //will require a string to long conversion from Consignment Details object

        [Column("dateTm")]  
        public string dateTm { get; set; }
        [Column("awbPfx")] 
        public string awbPfx {get;set;}
        [Column("awbNum")] 
        public string awbNum {get; set;}

        [Column("consigneeName")] 
        public string consigneeName {get;set;}
        
        [Column("totPieces")] 
        public int totPieces {get;set;}

        [Column("totWeight")] 
        public double totWeight {get;set;}

        [Column("origin")] 
        public string origin {get;set;}

        [Column("destination")] 
        public string destination {get;set;}
    
        [Column("isSingleNumber")] 
        public int numberContacts {get;set;}

        [Column("sendStatus")] 
        public string sendStatus {get;set;}
    
        [Column("cosysFinalupdate")]
        public string cosysFinalupdate {get;set;}

        [Column("cosysIntAck")] //does not need to be initialized while creating the DB record
        public string cosysIntAck {get;set;}

        [Column("noAwbContent")]
        public int noAwbContent {get;set;}

        //Constructor
        public RequestMain_Details (int msgSeqNo, string dateTm, string awbPfx, string awbNum, string consigneeName, int totPieces, double totWeight, string origin, string destination, string sendStatus, int numberContacts,  string cosysFinalupdate, int noAwbContent)
        {
        this.msgSeqNo = msgSeqNo;
        this.dateTm = dateTm;
        this.awbPfx = awbPfx;
        this.awbNum = awbNum;
        this.consigneeName = consigneeName;
        this.totPieces = totPieces;
        this.totWeight = totWeight;
        this.origin = origin;
        this.destination = destination;
        this.numberContacts = numberContacts;
        this.sendStatus = sendStatus;
        this.cosysFinalupdate = cosysFinalupdate;
        this.noAwbContent = noAwbContent;
        }

        public RequestMain_Details ()
        {
            //parameterless constructor
        }
    }

[Table("PhNoDetails")]
public class PhNoDetails
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("PhoneID")]
        public int PhoneID {get;set;} //should be auto-set
        [Column("requestID")]
        public int requestID {get;set;}
        [Column("phoneNumber")]     
        public  int phoneNumber { get; set; } 
        [Column("noTried")]     
        public  int noTried { get; set; }
        [Column("masterSentStatus")]     
        public  string masterSentStaus { get; set; }
        [Column("phoneSendStatus")] 
        public string phoneSendStatus {get; set;}
    
        //Constructor
        public PhNoDetails (int requestID, int phoneNumber, int noTried, string masterSentStaus, string phoneSendStatus)
        {
        this.requestID = requestID;
        this.phoneNumber = phoneNumber;
        this.noTried = noTried;
        this.masterSentStaus = masterSentStaus;
        this.phoneSendStatus = phoneSendStatus;
        }
    }

[Table("AwbDetails")]
public class AwbDetails
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("awbID")]
        public int SrNo {get;set;} //should be auto-set
        [Column("requestID")]
        public int requestID {get;set;}
        [Column("awbDetailsFlightCar")]     
        public  string flightCar { get; set; }
        [Column("awbDetailsFlightNum")]     
        public  string flightNum { get; set; }
        [Column("awbDetailsFlightDate")]     
        public string flightDate { get; set; } 
        [Column("awbDetailsFsuStatus")] 
        public string fsuStatus {get;set;} 
        [Column("awbDetailsPieces")] 
        public int pieces {get; set;} //should be auto-set
        [Column("awbDetailsWeight")] 
        public double weight {get; set;}

        [Column("awbDetailspdfContentPath")]
        public string pdfContentPath {get;set;}

        //Constructor
        public AwbDetails (int requestID,string flightCar, string flightNum, string flightDate,string fsuStatus, int pieces, double weight, string pdfContentPath)
        {
        this.requestID = requestID;
        this.flightCar = flightCar;
        this.flightNum = flightNum;
        this.flightDate = flightDate;
        this.fsuStatus = fsuStatus;
        this.pieces = pieces;
        this.weight = weight;
        this.pdfContentPath = pdfContentPath;
        }

    }

}
