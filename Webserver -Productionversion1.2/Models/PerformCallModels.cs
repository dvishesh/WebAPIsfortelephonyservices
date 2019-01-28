using System ; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Webserver.PerformCallModels 
{


     public class AWBDetails
    {
    public string flightCar {get;set;}
    public string flightNum {get; set;}
    public string flightDate {get; set;}
    public int pieces {get; set;}
    public double weight {get;set;}
    public string fsuStatus {get;set;}
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

    public class PerformCall_Response {
        public string msgSeqNo {get;set;}
        public string status {get;set;}
        public string errorCode {get;set;}
        public string errorDesc {get;set;}

        //Constructor
        public PerformCall_Response (string msgSeqNo, string status, string errorCode, string errorDesc)
        {
            this.msgSeqNo = msgSeqNo;
            this.status = status;
            this.errorCode = errorCode;
            this.errorDesc = errorDesc;
        }

        public PerformCall_Response() {
        //parameterless constructor}
        }
    }

//Domain models
[Table("SATS_NOTIFICATIONREQUEST_DETAILS")]
public class Sats_NotificationRequest_Details
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID")]
        public int ID {get;set;}

        [Column("msgSeqNum")]     
        public  long msgSeqNum { get; set; } //will require a string to long conversion from Consignment Details object

        [Column("datetime")]  
        public string datetime { get; set; }
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


        [Column("numberContacts")] 
        public int numberContacts {get;set;}

        [Column("requestStatus")] 
        public string requestStatus {get;set;}

        [Column("numberConsignmentParts")]
        public int numberConsignmentParts {get;set;}

        //Constructor
        public Sats_NotificationRequest_Details (long msgSeqNum, string datetime, string awbPfx, string awbNum, string consigneeName, int totPieces, double totWeight, string origin, string destination, int numberContacts, string requestStatus, int numberConsignmentParts)
        {
        this.msgSeqNum = msgSeqNum;
        this.datetime = datetime;
        this.awbPfx = awbPfx;
        this.awbNum = awbNum;
        this.consigneeName = consigneeName;
        this.totPieces = totPieces;
        this.totWeight = totWeight;
        this.origin = origin;
        this.destination = destination;
        this.numberContacts = numberContacts;
        this.requestStatus = requestStatus;
        this.numberConsignmentParts = numberConsignmentParts;
        }

        public Sats_NotificationRequest_Details ()
        {
            //paramterless constructor
        }
    }

[Table("SATS_OUTBOUND_CALLNOTIFICATION_RECORDS")]
public class Sats_Outbound_CallNotification_Records
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("SrNo")]
        public int SrNo {get;set;} //should be auto-set
        [Column("ID")]
        public int ID {get;set;}
        [Column("MsgSeqNum")]     
        public  string msgSeqNum { get; set; } 
        [Column("ContactNum")]     
        public  string contactNum { get; set; }
        [Column("ContactID")]     
        public  int contactID { get; set; }
   
        [Column("IsPresentContact")] 
        public int isPresentContact {get; set;}
    
        //Constructor
        public Sats_Outbound_CallNotification_Records (int ID, string msgSeqNum, string contactNum, int contactID, int isPresentContact)
        {
        this.ID = ID;
        this.msgSeqNum = msgSeqNum;
        this.contactNum = contactNum;
        this.contactID = contactID;
        this.isPresentContact = isPresentContact;
        }

        public Sats_Outbound_CallNotification_Records()
        {
            //parameterless constructor
        }
    }

[Table("SATS_CONSIGNMENTPARTS_DETAILS")]
public class Sats_ConsignmentParts_Details
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("SrNo")]
        public int SrNo {get;set;} //should be auto-set
        [Column("ID")]
        public int ID {get;set;}
        [Column("msgSeqNum")]     
        public  long msgSeqNum { get; set; } 
        [Column("flightCar")]     
        public  string flightCar { get; set; }
        [Column("flightNum")]     
        public  string flightNum { get; set; }
        [Column("flightDate")]     
        public string flightDate { get; set; } 
        [Column("fsuStatus")] 
        public string fsuStatus {get;set;} 
        [Column("pieces")] 
        public int pieces {get; set;} //should be auto-set
        [Column("weight")] 
        public double weight {get; set;}

        [Column("ConsignmentPartID")]
        public int consignmentPartID {get;set;}

        //Constructor
        public Sats_ConsignmentParts_Details (int ID, long msgSeqNum, string flightCar, string flightNum, string flightDate,string fsuStatus, int pieces, double weight, int consignmentPartID)
        {
        this.ID = ID;
        this.msgSeqNum = msgSeqNum;
        this.flightCar = flightCar;
        this.flightNum = flightNum;
        this.flightDate = flightDate;
        this.fsuStatus = fsuStatus;
        this.pieces = pieces;
        this.weight = weight;
        this.consignmentPartID = consignmentPartID;
        }
    }
  }
