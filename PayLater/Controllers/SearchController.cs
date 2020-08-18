using DBConnector;
using PayLater.Domain;
using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayLater.Controllers
{
    public class SearchController : ApiController
    {
        [Route("api/Search/{id}")]
        public IHttpActionResult GetPaymentInfo(int id)
        {
            return BadRequest();
            Helper helper = new Helper();
            Payment payment = null;
            // Test for request Validation @ 27APR
            try
            {
                PaylaterLogger.Info("Payment Search received. : " + id);
                DBUtility dbUtil = new DBUtility();
                payment = dbUtil.GetPaymentInfo(id);

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
