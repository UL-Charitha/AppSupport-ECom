using DBConnector;
using PayLater.Domain;
using PayLater.Domain.Util;
using PaylaterException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayLater.Controllers
{
    public class SearchByBankRefController : ApiController
    {
        [Route("api/SearchByBankRef/{bankRef}")] //todo check bank code while searching
        public IHttpActionResult GetPaymentInfo(string bankRef)
        {
            Helper helper = new Helper();
            Payment payment = null;
            // Test for request Validation @ 27APR
            try
            {
                PaylaterLogger.Info("Payment Search received. : " + bankRef);
                DBUtility dbUtil = new DBUtility();
                payment = dbUtil.GetPaymentInfoByBankRef(bankRef); 

                return Ok(helper.CreateSearchClientResponse(payment));
            }
            catch (PaylaterCommonException ex)
            {
                SearchClientWrapper searchClientResponse = new SearchClientWrapper();
                searchClientResponse.responseMessage = ex.Message;
                searchClientResponse.responseCode = ex.ErrorCode;
                searchClientResponse.transactionDate = DateTime.UtcNow;

                var response = Request.CreateResponse<SearchClientWrapper>(System.Net.HttpStatusCode.BadRequest, searchClientResponse);
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


    }
}
