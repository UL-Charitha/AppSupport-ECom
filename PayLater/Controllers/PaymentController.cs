using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayLater.Domain;
using PSSServicesConnector;
using DBConnector;
using PaylaterException;
using PaylaterException.Ticket;
using System.Configuration;
using PayLater.Domain.Util;
using PaylaterException.Common;

namespace PayLater.Controllers
{
    public class PaymentController : ApiController
    {
        [HttpPost]
        [Route("api/Payment")]
        public IHttpActionResult Payment(PaymentClientWrapper paymentClientRequest)
        {
            return BadRequest();
            Helper helper = new Helper();
            PNRService pnrService = null;
            Payment payment = null;
            DBUtility dbUtil = new DBUtility();
            // Test for request Validation @ 27APR
            try
            {
                PaylaterLogger.Info("Payment received for " + paymentClientRequest.pnr + " from " + paymentClientRequest.bankCode);
                ValidatePaymentRequest(paymentClientRequest);
                // End : Test for request Validation               
                payment = helper.CreatePaymentDomainRequest(paymentClientRequest);
                //Get office id related to bank id
                string officeId = dbUtil.GetOfficeId(payment.bankCode);
                PaylaterLogger.Info("Using office id - " + officeId);

                pnrService = new PNRService(officeId);

                payment.transactionDate = System.DateTime.UtcNow;

                bool status = pnrService.ProcessPayment(payment);

                int paymentId = dbUtil.SavePaymentRequest(payment);
                if (paymentId > 0)
                {
                    payment.paymentId = paymentId;
                }


                return Ok(helper.CreatePaymentClientResponse(payment));
            }
            catch (InvalidRequestInputException ex)
            {
                PaymentClientWrapper paymentClientResponse = paymentClientRequest;
                paymentClientResponse.responseMessage = ex.Message;
                paymentClientResponse.responseCode = ex.ErrorCode;
                paymentClientResponse.transactionDate = DateTime.UtcNow;

                var response = Request.CreateResponse<PaymentClientWrapper>(System.Net.HttpStatusCode.BadRequest, paymentClientResponse);
                return ResponseMessage(response);
            }
            catch (TicketingFailedAfterPricingException ex)
            {
                try
                {
                    payment.statusCode = 4001;
                    payment.responseCode = 4001;
                    payment.msg = StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS;
                    //queue the pnr                    
                    string queueId = ConfigurationManager.AppSettings["queueId"].ToString();
                    bool queued = pnrService.PlacePnrToQueue(payment.pnrID, queueId, ConfigurationManager.AppSettings["queueOfficeId"].ToString());

                    int paymentId = dbUtil.SavePaymentRequest(payment);
                    if (paymentId > 0)
                    {
                        payment.paymentId = paymentId;
                    }
                    try
                    {
                        NotificationSender.SendMailNotification(StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS, payment.pnrID);
                    }
                    catch (Exception x)
                    {
                        PaylaterLogger.Error("email sending failed " + x.Message);
                    }
                    return Ok(helper.CreatePaymentClientResponse(payment));
                }
                catch (Exception ex1)
                {
                    PaylaterLogger.Error("failed to add to queue " + payment.pnrID + " failed adding to queue");
                    PaylaterLogger.Error(ex1.Message);
                    NotificationSender.SendMailNotification(StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS, "Failed to add to queue" + payment.pnrID);

                    int paymentId = dbUtil.SavePaymentRequest(payment);
                    if (paymentId > 0)
                    {
                        payment.paymentId = paymentId;
                    }

                    return Ok(helper.CreatePaymentClientResponse(payment));
                }
            }

            catch (PaylaterCommonException ex)
            {
                payment.msg = ex.Message;
                payment.responseCode = ex.ErrorCode;
                var response = Request.CreateResponse<PaymentClientWrapper>(System.Net.HttpStatusCode.BadRequest, helper.CreatePaymentClientResponse(payment));
                if (ex.ErrorCode > 1999 && ex.ErrorCode < 3000) //internal error
                {
                    response = Request.CreateResponse<PaymentClientWrapper>(System.Net.HttpStatusCode.InternalServerError, helper.CreatePaymentClientResponse(payment));
                }
                return ResponseMessage(response);
            }
            catch (Exception)
            {
                payment.msg = StatusMessage.INTERNAL_ERROR;
                payment.responseCode = 2014;
                var response = Request.CreateResponse<PaymentClientWrapper>(System.Net.HttpStatusCode.InternalServerError, helper.CreatePaymentClientResponse(payment));
                return ResponseMessage(response);
            }
        }



        private void ValidatePaymentRequest(PaymentClientWrapper payment)
        {
            throw new InvalidRequestInputException(4010, StatusMessage.WRONG_REQUEST_INPUTS);
            // pnrID: 'LKDU9Y',  currencyDest: 'LKR', totalAmountDest: '296949', bankCode: 'samp0011', bankRef:'IPG-0129', 
            // customerName: 'Shashi SL5' 
            try
            {
                if (payment.pnr.Length != 6)
                {
                    throw new InvalidRequestInputException(4004, StatusMessage.INCORRECT_PNR_INPUT);
                }
                if (payment.currency.Length != 3)
                {
                    throw new InvalidRequestInputException(4005, StatusMessage.INCORRECT_PNR_DESTINATION_CURRENCY_INPUT);
                }
                try
                {
                    payment.amount = Convert.ToDouble(payment.amount);
                    if (payment.amount == 0)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new InvalidRequestInputException(4006, StatusMessage.INCORRECT_PNR_DESTINATION_AMOUNT_INPUT);
                }
                if (payment.bankCode.Length > 8 || payment.bankCode.Length < 4)
                {
                    throw new InvalidRequestInputException(4007, StatusMessage.INCORRECT_BANK_CODE_INPUT);
                }
                if (payment.bankRef.Length > 30)
                {
                    throw new InvalidRequestInputException(4008, StatusMessage.TOO_LONG_BANK_REFERENCE_CODE_INPUT);
                }
                if (payment.customerName.Length > 100)
                {
                    throw new InvalidRequestInputException(4009, StatusMessage.TOO_LONG_CUSTOMER_NAME_INPUT);
                }
            }
            catch (InvalidRequestInputException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InvalidRequestInputException(4010, StatusMessage.WRONG_REQUEST_INPUTS);
            }
        }

    }
}
