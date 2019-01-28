using System ; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Webserver.EnquireAwbInboundModels {

     public class EnquireAwbInbound_Receive
{
    public int awbPfx {get;set;}
    public int awbNum {get;set;}
}

     public class AwbEnquireAWBDetails
    {
    public string flightCar {get;set;}
    public string flightNum {get; set;}
    public string flightDate {get; set;}
    public int pieces {get; set;}
    public double weight {get;set;}
    public string fsuStatus {get;set;}
    }
 

    public class AwbEnquireConsignmentDetails {

        public string msgSeqNo { get; set; } 
        public string datetime{get;set;} // generated by api, for record keeping only
        public string status { get; set; } 
        public string errorCode {get;set;}
        public string errorDesc {get;set;}
        public string awbPfx {get;set;}
        public string awbNum {get; set;}
        public int totPieces {get;set;}
        public double totWeight {get;set;}
        public string origin {get;set;}
        public string destination {get;set;}
        public string consigneeName {get;set;}
        public List<AwbEnquireAWBDetails> awbDetails {get;set;}
        
        //Constructor
        public AwbEnquireConsignmentDetails()
        {
            this.awbDetails = new List<AwbEnquireAWBDetails>(); 
        }
         
    }

//domain model

[Table("AwbEnquireRequest_MainDetails")]
public class AwbEnquireRequest_MainDetails
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("msgSeqNo")]
        public long msgSeqNo {get;set;}
        [Column("datetime")]  
        public string datetime { get; set; }
        [Column("status")] 
        public string status { get; set; }
        [Column("errorCode")]  
        public string errorCode {get;set;}
        [Column("errorDesc")] 
        public string errorDesc {get;set;}
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

        [Column("numberConsignmentParts")] //populated by api for later direct use
        public int numberConsignmentParts {get;set;}

        //Constructor for status = "S"
        public AwbEnquireRequest_MainDetails (string datetime, string status, string errorCode, string errorDesc,string awbPfx, string awbNum, string consigneeName, int totPieces, double totWeight, string origin, string destination, int numberConsignmentParts)
        {
        this.msgSeqNo = msgSeqNo;
        this.datetime = datetime; // generated by api - for record keeping purposes only
        this.status = status;
        this.errorCode = errorCode;
        this.errorDesc = errorDesc;
        this.awbPfx = awbPfx;
        this.awbNum = awbNum;
        this.consigneeName = consigneeName;
        this.totPieces = totPieces;
        this.totWeight = totWeight;
        this.origin = origin;
        this.destination = destination;
        this.numberConsignmentParts = numberConsignmentParts; //generated by api - for use later
        }
        // Constructor in case of status = "F"
        public AwbEnquireRequest_MainDetails(string datetime, string status, string errorCode, string errorDesc, string awbPfx, string awbNum)
        {
            this.msgSeqNo = msgSeqNo;
            this.datetime = datetime;
            this.status = status;
            this.errorCode = errorCode;
            this.errorDesc = errorDesc;
            this.awbPfx = awbPfx;
            this.awbNum = awbNum;
        }
        public AwbEnquireRequest_MainDetails ()
        {
            //paramterless constructor
        }
    }


[Table("AwbEnquireRequest_ConsignmentParts_Details")]
public class AwbEnquireRequest_ConsignmentParts_Details
    {   
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("SrNo")]
        public int SrNo {get;set;} //should be auto-set
        [Column("msgSeqNo")]     
        public  long msgSeqNo { get; set; } 
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
        public AwbEnquireRequest_ConsignmentParts_Details (long msgSeqNo, string flightCar, string flightNum, string flightDate,string fsuStatus, int pieces, double weight, int consignmentPartID)
        {
        this.msgSeqNo = msgSeqNo;
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