using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Ticket;
using PSSServicesConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBConnector;
using PayLater.Domain;

namespace PayLater.Controllers
{
    public class ReconciliationController : ApiController
    {
        DBUtility dbUtil = new DBUtility();       
        Helper helper = new Helper();

        [HttpGet]
        [Route("api/BankRecon/{bankcode}/{fromdate}/{todate}")]
        public IHttpActionResult SendReconciliationData(string bankCode, DateTime fromDate, DateTime toDate)
        {
            BankReconciliationClientWrapper payments= dbUtil.GetPaymentInfo(bankCode,fromDate, toDate.AddDays(1)); //add one day to toDate to include data in toDate in the response
            return Ok(payments);                    
        }

        [HttpPost]
        [Route("api/AirlineRecon")]
        public IHttpActionResult ReconcileBankData(UlReconciliationClientWrapper reconciliationData)
        {
            try
            {
                PaylaterLogger.Info("Reconciliation data received from " + reconciliationData.fromDate + " to " + reconciliationData.toDate + " for " + reconciliationData.bankCode);
                dbUtil.SaveReconcileData(reconciliationData);
                return Ok();
            }
            catch (PaylaterCommonException ex)
            {
                if (ex.ErrorCode > 1999 && ex.ErrorCode < 3000) //internal error
                {
                    return InternalServerError();
                }
                
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                return InternalServerError();
            }
        }
    }
}
