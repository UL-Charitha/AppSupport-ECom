using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBConnector;
using PSSServicesConnector;
using PayLater.Domain;
using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Fare;
using PaylaterException.Common;
using Domain;
using Newtonsoft.Json;
using System.Configuration;

namespace PayLater.Controllers
{
    public class PNRController : ApiController
    {
        private string test;
        Helper helper = new Helper();
        /// <summary>
        /// paylater new
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="bankId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Route("api/pnr/{pnr}/{bankcode}/{currency}")]
        public IHttpActionResult GetPNR(string pnr, string bankCode, string currency)
        {
            currency = currency.ToUpper();
            Helper helper = new Helper();
            // Test for request Validation @ 27APR
            try
            {
                ValidatePnrFareRequest(pnr, bankCode, currency);
            }
            catch (InvalidRequestInputException ex)
            {
                PnrClientWrapper pnrResponse = new PnrClientWrapper();
                pnrResponse.responseMessage = ex.Message;
                pnrResponse.responseCode = ex.ErrorCode;
                pnrResponse.currency = currency;
                pnrResponse.pnr = pnr;
                pnrResponse.expiry = null;
                pnrResponse.pnrTotalValue = null;
                
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.BadRequest, pnrResponse);
                return ResponseMessage(response);
            }
            // End : Test for request Validation

            Pnr pnrRet;

            try
            {
                PaylaterLogger.Info("Booking received. " + pnr + " " + bankCode + " " + currency);
                DBUtility dbUtil = new DBUtility();

                //Get office id related to bank id
                //string officeId = "CMBUL08P2";//dbUtil.GetOfficeId(bankCode);

                string officeId = "LONUL08P1"; // dbUtil.GetOfficeIdNew(bankCode);

                PaylaterLogger.Info("Using office id - " + officeId);
                PNRService pnrService = new PNRService(officeId);
                
                //todo check the bank id format in existing paylater

                //Get pnr object with calculated fare in it.
                pnrRet = pnrService.GetPnrInfo(pnr, currency);

                // Saving to DB : dbo.PYL_T_PNR_FARE 
                pnrRet.requestingBankCode = bankCode;
                pnrRet.transactionUTCTime = System.DateTime.UtcNow;
                bool saved = dbUtil.SavePnrFareRequest(pnrRet);                
                return Ok(helper.CreatePnrClientResponse(pnrRet));
            }
            
            catch (PaylaterCommonException ex)
            {                
                PnrClientWrapper pnrResponse = new PnrClientWrapper() { pnr = pnr, pnrTotalValue = null, currency = currency, expiry = null };
                pnrResponse.responseMessage = ex.Message;
                pnrResponse.responseCode = ex.ErrorCode;                
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.BadRequest, pnrResponse);
                if (ex.ErrorCode > 1999 && ex.ErrorCode < 3000) //internal error
                {
                    response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.InternalServerError, pnrResponse);
                }
                return ResponseMessage(response);
   
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                PnrClientWrapper pnrResponse = new PnrClientWrapper() { pnr = pnr, pnrTotalValue = null, currency = currency, expiry = null };
                pnrResponse.responseMessage = StatusMessage.INTERNAL_ERROR;
                pnrResponse.responseCode = 2015;
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.InternalServerError, pnrResponse);
                return ResponseMessage(response);                
            }
        }

        private void ValidatePnrFareRequest(string pnr, string bankCode, string currency)
        {
            try
            {
                if (pnr.Length != 6)
                {
                    throw new InvalidRequestInputException(4004, StatusMessage.INCORRECT_PNR_INPUT);
                }
                if (bankCode.Length > 8 || bankCode.Length < 4)
                {
                    throw new InvalidRequestInputException(4007, StatusMessage.INCORRECT_BANK_CODE_INPUT);
                }
                if (currency.Length != 3)
                {
                    throw new InvalidRequestInputException(4005, StatusMessage.INCORRECT_PNR_DESTINATION_CURRENCY_INPUT);
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
      

        private string GetPnrInfo(string pnr, string cur)
        {
            return String.Format("The Request for PNR: {0} in {1} was Recieved", pnr, cur);
        }

        [Route("api/pnr2/{pnr}")]
        public IHttpActionResult GetPNRx(string pnr) 
        {
            string currency = "LKR";
            Pnr pnrRet;
            TicketInfo ticketInfo;
            string paymentID = "1234"; // jz ignore
            CspModel model = new CspModel();
            //pnr >> http://localhost:13235/api/PNR2/WVH93G 

            if (!IsValidPnr(pnr))
            {
                return BadRequest();
            }

            try
            {
                DBUtility dbUtil = new DBUtility();

                //Get office id related to bank id
                //string officeId = "CMBUL08P2";//dbUtil.GetOfficeId(bankCode);
                string officeId = ConfigurationManager.AppSettings["DefaultOfficeId"].ToString(); // "LONUL08P1"; // dbUtil.GetOfficeIdNew(bankCode);

                PaylaterLogger.Info("Using office id - " + officeId);
                PNRService pnrService = new PNRService(officeId);

                //Get pnr object with calculated fare in it.
                pnrRet = pnrService.GetPnrInfoModel(pnr, currency, model);

                ticketInfo = new TicketInfo(Convert.ToInt32(paymentID));
                ticketInfo.pnrID = pnr;
                //bool DBvaluesUpdated = SetTicketInfoValues(ticketInfo);
                // Fill with Amadeus Info
                bool TicketInfoUpdated = pnrService.GetTicketInfo(ticketInfo);
                //Finally Generate HTML
                bool done = GenerateHtmlTicketInformation(ticketInfo);

                bool cspDataDone = SetCspModelWithTicketInfo(model, ticketInfo);

                return Ok(model);
            }


            catch (Exception ex)
            {
                throw;
            }
        }

        private bool IsValidPnr(string pnr)
        {
            try
            {
                if (pnr.Length == 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SetCspModelWithTicketInfo(CspModel model, TicketInfo ticketInfo)
        {
            try
            {
                model.TicketNumbers = "";
                model.travelDatesInfo = "";
                foreach (var item in ticketInfo.ticketList)
                {
                    model.TicketNumbers += $"{item.ticket} ({item.paxLName} {item.paxFName}) . ";
                }
                foreach (var item in ticketInfo.itineraryInfoList)
                {
                    string dateVal = item.dateTimeDep.ToString("dd.MMM.yyyy @ HH:mm");
                    model.travelDatesInfo += $"|| {dateVal} ({item.stationDep}) [{item.flightID}] ";
                }
                return true;
            }
            catch (Exception)
            { 
                return false;
            }
        }

        private bool GenerateHtmlTicketInformation(TicketInfo ticketInfo)
        {
            try
            {
                //todo : bind to html template !!
                ticketInfo.ticketInformation = helper.GenerateHtmlTicketInformation(ticketInfo);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("api/pnrTest/{pnr}")]
        public IHttpActionResult GetPNRTest(string pnr)  
        {
            if (!IsValidPnr(pnr))
            {
                return BadRequest();
            }
            string officeId = ConfigurationManager.AppSettings["DefaultOfficeId"].ToString();
            PaylaterLogger.Info("Testing : Using office id - " + officeId); 

            CspModel model = new CspModel();
            try
            {
                model.contactInfo = "+94 772258983-B . +94 772258983-H . UL/E+CHARITHA.WARAVITA@SRILANKAN.COM . ";
                model.emailOnPnr = "CHARITHA.WARAVITA@SRILANKAN.COM";
                model.hasServices = "No";
                model.LastUpdated = DateTime.Now;
                model.OfficeId = "CMBUL08AI ( CMBUL07AE )";
                model.PaymentAmount = 550;
                model.paymentCurrency = "USD";
                model.paymentStatus = "FP:CASH";
                model.pnrStatus = "Retrieved";
                model.servicesFare = 0;
                model.TicketedDateString = "30.Jul.2020 (CMBUL07AE)";
                model.TicketNumbers = "603-2111508570 (WARAVITA CHARITHA MR) . ";
                model.travelDatesInfo = "|| 10.Sep.2020 @ 00:20 (CMB) [UL604] ";

                return Ok(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("api/pnrTest/{pnr}/{meta}")]
        public string GetPNRTest2(string pnr, string meta)  
        {
            CspModel model = new CspModel();

            try
            {
                model.contactInfo = "+94 772258983-B . +94 772258983-H . UL/E+CHARITHA.WARAVITA@SRILANKAN.COM . ";
                model.emailOnPnr = "CHARITHA.WARAVITA@SRILANKAN.COM";
                model.hasServices = "No";
                model.LastUpdated = DateTime.Now;
                model.OfficeId = "CMBUL08AI ( CMBUL07AE )";
                model.PaymentAmount = 550;
                model.paymentCurrency = "USD";
                model.paymentStatus = "FP:CASH";
                model.pnrStatus = "Retrieved";
                model.servicesFare = 0;
                model.TicketedDateString = "30.Jul.2020 (CMBUL07AE)";
                model.TicketNumbers = "603-2111508570 (WARAVITA CHARITHA MR) . ";
                model.travelDatesInfo = "|| 10.Sep.2020 @ 00:20 (CMB) [UL604] ";

                var dd1 = JsonConvert.SerializeObject(model);
                return dd1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}