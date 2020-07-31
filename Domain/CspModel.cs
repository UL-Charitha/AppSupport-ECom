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
        public string paymentStatus { get; set; }
        public double PaymentAmount { get; set; }
        public string paymentCurrency { get; set; }

        public string TicketedDateString { get; set; }
        public string TicketNumbers { get; set; }

        public string emailOnPnr { get; set; }
        public string contactInfo { get; set; }
        public DateTime LastUpdated{ get; set; }

        public string hasServices { get; set; }
        
        public double servicesFare { get; set; }
        public string travelDatesInfo { get; set; }

    }
}
