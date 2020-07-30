using PayLater.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketInfo
    {


        #region Properties

        public Int32 paymentId { get; set; }
        public decimal TotalPrice { get; set; }
        public string TotalPriceCurrency { get; set; }
        public string pnrID { get; set; }
        public string bankRef { get; set; }
        public int responseCode { get; set; }
        public string msg { get; set; }
        public string ticketInformation { get; set; }
        public List<ItineraryInfo> itineraryInfoList { get; set; }
        public List<Ticket> ticketList { get; set; }

        #endregion

        public TicketInfo(int paymentID)
        {
            this.paymentId = paymentID;
            itineraryInfoList = new List<ItineraryInfo>();
            ticketList = new List<Ticket>();
        }
    }
}
