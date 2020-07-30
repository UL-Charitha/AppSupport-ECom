using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayLater.Domain
{
    public class Pnr
    {
        public string pnrID { get; set; }
        public double totalAmountOrigin { get; set; }
        public string originCurrency { get; set; }
        public double totalAmountDestination { get; set; }
        public string DestinationCurrency { get; set; } 
        public DateTime expDate { get; set; }
        public DateTime transactionUTCTime { get; set; }
        public int responseCode { get; set; }
        public string msg { get; set; }
        public string requestingBankCode { get; set; }
        public bool hasServices { get; set; }
        public List<string> paxList { get; set; }

    }
}