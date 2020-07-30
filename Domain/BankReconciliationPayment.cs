using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain
{
    public class BankReconciliationPayment
    {
        public string branchCode { get; set; }
        public Int32 transactionId { get; set; }
        public string pnr { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string bankRef { get; set; }        
        public string customerName { get; set; }        
        public DateTime transactionDate { get; set; }
        public int responseCode { get; set; }        
    }
}
