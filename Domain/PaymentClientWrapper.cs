using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain
{
    public class PaymentClientWrapper
    {
        public int? transactionId { get; set; }
        public string bankCode { get; set; }
        public string bankRef { get; set; }
        public string pnr { get; set; }        
        public double amount { get; set; }
        public string customerName { get; set; }
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public DateTime transactionDate { get; set; }
        private string _currency;

        public string currency
        {
            get { return _currency; }
            set { _currency = value.ToUpper(); }
        }
    }
}
