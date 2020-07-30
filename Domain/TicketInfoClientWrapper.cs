using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketInfoClientWrapper
    {
        #region Properties

        public Int32 transactionId { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string pnr { get; set; }
        public string bankRef { get; set; }
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public string ticketInformation { get; set; }


        #endregion
    }
}
