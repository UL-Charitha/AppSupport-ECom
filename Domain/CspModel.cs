using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CspModel
    {
        public string OfficeId { get; set; }
        public string pnrStatus { get; set; }
        public TicketInfo ticketInfo { get; set; }
        public string paymentStatus { get; set; }
        public string PaymentAmount { get; set; }

        public string TicketedDateString { get; set; }

        public string emailOnPnr { get; set; }
        public string LastUpdatedString { get; set; }
    }
}
