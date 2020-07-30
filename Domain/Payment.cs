using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayLater.Domain
{
    public class Payment
    {
        public Int32 paymentId { get; set; }
        public string pnrID { get; set; }
        public double totalAmountOrig { get; set; }
        public string currencyOrig { get; set; }
        public double totalAmountDest { get; set; }
        public string currencyDest { get; set; }
        public string bankRef { get; set; }
        public string bankCode { get; set; }
        public string customerName { get; set; }
        public DateTime transactionDate { get; set; }
        public DateTime expDate { get; set; }
        public string msg { get; set; } 
        public int statusCode { get; set; } 
        public int responseCode { get; set; }
        public List<Ticket> tickets { get; set; } 


    }
}