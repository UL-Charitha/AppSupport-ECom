using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain
{
    public class UlReconciliationClientWrapper
    {
        public string bankCode { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public List<UlReconciliationPayment> paymentList { get; set; }
    }
}
