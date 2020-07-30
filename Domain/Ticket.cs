using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain
{
    public class Ticket
    {
        public Int32 paymentId { get; set; }
        public string ticket { get; set; }
        public string paxType { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public string paxRef { get; set; }
        public string paxFName { get; set; }
        public string paxLName { get; set; }  

    }
}
