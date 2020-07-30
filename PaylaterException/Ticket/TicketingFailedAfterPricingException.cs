using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException.Ticket
{
    /// <summary>
    /// thrown if updating FOP is successful, but failed to ticket. such PNR need to be queued.
    /// </summary>
    public class TicketingFailedAfterPricingException:PaylaterCommonException
    {
        public TicketingFailedAfterPricingException(Int32 ErrorCode, string Message) : base(ErrorCode, Message) { }
    }
}
