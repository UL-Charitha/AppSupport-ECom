using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain
{
    public class PnrClientWrapper
    {
        public string pnr { get; set; }
        public double? pnrTotalValue { get; set; }
        public string currency { get; set; }
        public DateTime? expiry { get; set; }
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public string paxList { get; set; }
    }
}
