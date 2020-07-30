using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using System.Reflection;
using PayLater.Domain;
using PayLater.Domain.Util;
using PaylaterException.Common;
using PaylaterException;
using PaylaterException.Ticket;
using System.Data.Entity.Validation;

namespace DBConnector
{
    public class DBUtility
    {
        PayLaterEntities db = new PayLaterEntities();
        Payment paymentObj = new Payment();
        Helper helperObj = null;

        public string GetOfficeId(string bankCode)
        {
            try
            {
                bankCode = bankCode.Substring(0, 4);
                var context = new PayLaterEntities();
                var result = (from obj in context.PYL_T_BANK_OFFICEID
                              where obj.BankCode == bankCode
                              select obj.OfficeId).FirstOrDefault();
                string officeId = result;
                if (officeId == null)
                {
                    throw new InvalidBankCodeException(1000, StatusMessage.BANK_CODE_NOT_FOUND);
                }
                return officeId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SavePnrFareRequest(Pnr pnr)
        {
            PYL_T_PNR_FARE dbPnrObj = new PYL_T_PNR_FARE();
            dbPnrObj.Pnr = pnr.pnrID;
            dbPnrObj.TotalAmountOriginal = Convert.ToDecimal(pnr.totalAmountOrigin);
            dbPnrObj.CurrencyOriginal = pnr.originCurrency;
            dbPnrObj.TotalAmountDestination = Convert.ToDecimal(pnr.totalAmountDestination);
            dbPnrObj.CurrencyDestination = pnr.DestinationCurrency;
            dbPnrObj.BankCode = pnr.requestingBankCode;
            dbPnrObj.TransactionUTCTime = pnr.transactionUTCTime;
            dbPnrObj.ResponseCode = pnr.responseCode;
            dbPnrObj.ResponseMessage = pnr.msg;
            try
            {
                db.PYL_T_PNR_FARE.Add(dbPnrObj);
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetOfficeIdNew(string bankCode)
        {
            try
            {
                bankCode = bankCode.Substring(0, 4);
                var context = new PayLaterEntities();
                var result = (from x in context.PYL_T_BANK_OFFICEID
                              where x.BankCode == bankCode
                              select x.OfficeId).FirstOrDefault();
                string officeId = result;
                if (officeId == null)
                {
                    throw new InvalidBankCodeException(1000, StatusMessage.BANK_CODE_NOT_FOUND);
                }
                return officeId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int SavePaymentRequest(Payment paymentRes)
        {
            PYL_T_PAYMENT dbPaymentObj = new PYL_T_PAYMENT();
            dbPaymentObj.Pnr = paymentRes.pnrID;
            dbPaymentObj.TotalAmountOriginal = Convert.ToDecimal(paymentRes.totalAmountOrig);
            dbPaymentObj.CurrencyOriginal = paymentRes.currencyOrig;
            dbPaymentObj.TotalAmountDestination = Convert.ToDecimal(paymentRes.totalAmountDest);
            dbPaymentObj.CurrencyDestination = paymentRes.currencyDest;
            dbPaymentObj.BankRef = paymentRes.bankRef;
            dbPaymentObj.BankCode = paymentRes.bankCode;
            dbPaymentObj.CustomerName = paymentRes.customerName;
            dbPaymentObj.TransactionDate = paymentRes.transactionDate;
            dbPaymentObj.ExpiryDate = paymentRes.expDate;
            dbPaymentObj.StatusCode = paymentRes.statusCode;
            dbPaymentObj.ResponseCode = paymentRes.responseCode;
            dbPaymentObj.ResponseMessage = paymentRes.msg;

            try
            {
                db.PYL_T_PAYMENT.Add(dbPaymentObj);
                db.SaveChanges();
                // *******  Then Return the Payment ID *********************
                if (paymentRes.tickets != null)
                {
                    if (paymentRes.tickets.Count > 0)
                    {
                        foreach (var ticket in paymentRes.tickets)
                        {
                            PYL_T_TICKET dbTicket = new PYL_T_TICKET();
                            //  Set Values
                            dbTicket.PaymentId = dbPaymentObj.PaymentId;
                            dbTicket.TicketNo = ticket.ticket;
                            dbTicket.PassengerType = ticket.paxType;
                            dbTicket.TicketPrice = ticket.price;
                            dbTicket.Currency = ticket.currency;

                            db.PYL_T_TICKET.Add(dbTicket);
                        }
                        db.SaveChanges();
                    }
                }

                return dbPaymentObj.PaymentId;
                // **Alternative way**// var pnrs = db.Set<PYL_T_PNR_FARE>();pnrs.Add(dbPnrObj);db.SaveChanges();
            }
            catch (Exception ex)
            {
                helperObj = new Helper();
                StringBuilder sb = helperObj.GetSavePaymentFailSB(paymentRes);
                NotificationSender.SendMailNotification("Failure - save payment", sb.ToString());

                PaylaterLogger.Error(ex.Message);
                PaylaterLogger.Error(sb.ToString());
                return -1;
            }
        }

        public Payment GetPaymentInfo(int id)
        {
            try
            {
                var context = new PayLaterEntities();
                var result = (from payment in context.PYL_T_PAYMENT
                              where payment.PaymentId == id
                              select payment).FirstOrDefault();

                if (result == null)
                {
                    throw new PaylaterCommonException(5001, StatusMessage.PAYMENT_NOT_FOUND);
                }

                paymentObj.paymentId = result.PaymentId;
                paymentObj.pnrID = result.Pnr;
                paymentObj.totalAmountDest = Convert.ToDouble(result.TotalAmountDestination);
                paymentObj.currencyDest = result.CurrencyDestination;
                paymentObj.bankCode = result.BankCode;
                paymentObj.bankRef = result.BankRef;
                paymentObj.customerName = result.CustomerName;
                paymentObj.responseCode = result.ResponseCode;
                paymentObj.msg = result.ResponseMessage;
                paymentObj.transactionDate = result.TransactionDate;

                return paymentObj;
            }
            catch (PaylaterCommonException ex)
            {
                throw;
            }
        }

        // Search By Bank Ref !!
        public Payment GetPaymentInfoByBankRef(string bankRef)
        {
            try
            {
                var context = new PayLaterEntities();
                var result = (from payment in context.PYL_T_PAYMENT
                              where payment.BankRef == bankRef
                              orderby payment.PaymentId descending
                              select payment).FirstOrDefault();

                if (result == null)
                {
                    throw new PaylaterCommonException(5001, StatusMessage.PAYMENT_NOT_FOUND);
                }

                paymentObj.paymentId = result.PaymentId;
                paymentObj.pnrID = result.Pnr;
                paymentObj.totalAmountDest = Convert.ToDouble(result.TotalAmountDestination);
                paymentObj.currencyDest = result.CurrencyDestination;
                paymentObj.bankCode = result.BankCode;
                paymentObj.bankRef = result.BankRef;
                paymentObj.customerName = result.CustomerName;
                paymentObj.responseCode = result.ResponseCode;
                paymentObj.msg = result.ResponseMessage;
                paymentObj.transactionDate = result.TransactionDate;

                return paymentObj;
            }
            catch (PaylaterCommonException ex)
            {
                throw;
            }
        }

        public int GetPaymentInfoByPost(PaymentClientWrapper req)
        {
            try
            {
                var result = (from x in db.PYL_T_PAYMENT
                              where x.BankRef == req.bankRef
                              orderby x.PaymentId descending
                              select x).FirstOrDefault();

                if (result == null)
                {
                    throw new PaylaterCommonException(2016, StatusMessage.INTERNAL_ERROR);
                }
                int id = result.PaymentId;
                return id;
            }
            catch (Exception)
            {
                return -2;
            }
        }



        public PYL_T_PAYMENT GetPaymentInfoByPaymentID(int paymentId)
        {
            try
            {
                var context = new PayLaterEntities();
                var result = (from payment in context.PYL_T_PAYMENT
                              where payment.PaymentId == paymentId && payment.ResponseCode == 0
                              select payment).FirstOrDefault();

                if (result == null)
                {
                    //
                    result = (from payment in context.PYL_T_PAYMENT
                              where payment.PaymentId == paymentId && payment.ResponseCode == 4001
                              select payment).FirstOrDefault();

                    if (result == null)
                    {
                        throw new PaylaterCommonException(5001, StatusMessage.PAYMENT_NOT_FOUND);
                    }
                    else
                    {
                        throw new TicketingFailedAfterPricingException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
                    }
                }
                else
                {
                    return result;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        public BankReconciliationClientWrapper GetPaymentInfo(string bankCode, DateTime fromDate, DateTime toDate)
        {
            try
            {
                BankReconciliationClientWrapper reconWrapper = new BankReconciliationClientWrapper();
                reconWrapper.bankCode = bankCode;
                reconWrapper.fromDate = fromDate;
                reconWrapper.toDate = toDate;
                List<BankReconciliationPayment> payments = new List<BankReconciliationPayment>();
                var context = new PayLaterEntities();
                var result = (from payment in context.PYL_T_PAYMENT
                              where payment.TransactionDate >= fromDate && payment.TransactionDate <= toDate && payment.BankCode.Contains(bankCode)
                              select payment).ToList();
                if (result == null)
                {
                    throw new PaylaterCommonException(5001, StatusMessage.PAYMENT_NOT_FOUND);
                }

                foreach (var payment in result)
                {
                    payments.Add(GetPaymentObj(payment));
                }
                reconWrapper.paymentList = payments;
                return reconWrapper;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// converts PYL_T_PAYMENT to Payment
        /// </summary>
        /// <param name="dbPayment"></param>
        /// <returns></returns>
        private BankReconciliationPayment GetPaymentObj(PYL_T_PAYMENT dbPayment)
        {
            BankReconciliationPayment payment = new BankReconciliationPayment();
            payment.transactionId = dbPayment.PaymentId;
            payment.pnr = dbPayment.Pnr;
            payment.amount = Convert.ToDecimal(dbPayment.TotalAmountDestination);
            payment.currency = dbPayment.CurrencyDestination;
            payment.branchCode = dbPayment.BankCode.Substring(4);
            payment.bankRef = dbPayment.BankRef;
            payment.customerName = dbPayment.CustomerName;
            payment.responseCode = dbPayment.ResponseCode;            
            payment.transactionDate = dbPayment.TransactionDate;
            return payment;
        }

        public int SaveReconcileData(UlReconciliationClientWrapper reconciliationData)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //PayLaterEntities paymentContext = new PayLaterEntities();
                    PYL_M_RECONCILIATION_PAYMENT reconPayment = new PYL_M_RECONCILIATION_PAYMENT();
                    reconPayment.BankCode = reconciliationData.bankCode;
                    reconPayment.FromDate = reconciliationData.fromDate;
                    reconPayment.ToDate = reconciliationData.toDate;
                    reconPayment.CreatedDate = DateTime.Now;
                    db.PYL_M_RECONCILIATION_PAYMENT.Add(reconPayment);
                    db.SaveChanges();
                    foreach (UlReconciliationPayment paymentDetail in reconciliationData.paymentList)
                    {
                        PYL_T_RECONCILIATION_PAYMENT_DETAIL dbPaymentDetail = new PYL_T_RECONCILIATION_PAYMENT_DETAIL();
                        dbPaymentDetail.ReconciliationPaymentId = reconPayment.ReconciliationPaymentId;
                        dbPaymentDetail.BranchCode = paymentDetail.branchCode;
                        dbPaymentDetail.BankRef = paymentDetail.bankRef;
                        dbPaymentDetail.PaymentId = paymentDetail.transactionId;
                        dbPaymentDetail.Pnr = paymentDetail.pnr;
                        dbPaymentDetail.Currency = paymentDetail.currency;
                        dbPaymentDetail.Amount = paymentDetail.amount;
                        dbPaymentDetail.TransactionDate = paymentDetail.transactionDate;
                        dbPaymentDetail.ResponseCode = paymentDetail.responseCode;
                        dbPaymentDetail.CustomerName = paymentDetail.customerName;
                        dbPaymentDetail.IdNumber = paymentDetail.IdNumber;
                        db.PYL_T_RECONCILIATION_PAYMENT_DETAIL.Add(dbPaymentDetail);
                    }
                    db.SaveChanges();
                    scope.Complete();
                }
                return reconciliationData.paymentList.Count;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                
                var fullErrorMessage = string.Join("; ", errorMessages);
                throw new PaylaterCommonException(6001,fullErrorMessage);
            }
            catch (Exception ex)
            {
                
                if (ex.InnerException!=null &&ex.InnerException.InnerException!=null && ex.InnerException.InnerException.Message.Contains("Violation of PRIMARY KEY constraint 'PK_PYL_T_RECONCILIATION_PAYMENT_DETAIL'"))
                {
                    throw new PaylaterCommonException(6000, StatusMessage.DUPLICATE_ENTRY);
                }
                PaylaterLogger.Error("Failed to save reconciliation data " + ex.Message);
                NotificationSender.SendMailNotification("Paylater - Failed to save reconciliation data", "bank code - " + reconciliationData.bankCode + " from " + reconciliationData.fromDate + " to " + reconciliationData.toDate);
                throw new PaylaterCommonException(2021, StatusMessage.INTERNAL_ERROR);
            }
        }

        public double GetUTCDifference(string station)
        {
            try
            {
                var result = (from obj in db.PYL_M_UTC_TIME
                              where obj.DepartureStation == station
                              select obj).FirstOrDefault();
                if (result==null)
                {
                    throw new PaylaterCommonException(3003, StatusMessage.BOARDPOINT_NOT_SUPPORTED);
                }
                double timeDifference = result.UtcDifferenceMinutes;
                return timeDifference;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error("failed to get UTC time gap - " + ex.Message);
                throw ex;
            }
        }
    }
}
