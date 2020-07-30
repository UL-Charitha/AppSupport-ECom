using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLater.Domain.Util
{
    public class StatusMessage
    {
        public static string SUCCESS = "Success";
        public static string BANK_CODE_NOT_FOUND = "Bank Code Not Found";
        public static string INTERNAL_ERROR = "Internal Error";
        public static string PNR_NOT_FOUND = "PNR Not Found";
        public static string INCORRECT_AMOUNT = "Incorrect Amount For PNR";
        public static string TICKETING_FAILED_AFTER_PAYMENT_SUCCESS = "Ticketing Failed After Payment Success";
        public static string MISSING_EXPIRY_DATE = "Expiry date is missing in the PNR";
        public static string QUEUE_PROCESS_FAILED = "Queue process Failed";
        public static string PAYMENT_FAILED = "Payment Failed";
        public static string PNR_ALREADY_PAID = "PNR already Paid";
        public static string PNR_FARE_CONVERSION_FAILED = "PNR Fare conversion Failed";
        public static string WRONG_REQUEST_INPUTS = "Wrong Request Parameters Found";
        public static string PNR_NOT_PRICED = "PNR is not Priced";
        public static string PNR_EXPIRED = "PNR has expired";
        public static string PAYMENT_NOT_FOUND = "Payment not found";
        public static string DUPLICATE_ENTRY = "Duplicate entry found";
        public static string BOARDPOINT_NOT_SUPPORTED = "Board point not supported";
        

        public static string INCORRECT_PNR_INPUT = "Incorrect PNR";
        public static string INCORRECT_PNR_DESTINATION_CURRENCY_INPUT = "Incorrect PNR Destination Currency";
        public static string INCORRECT_PNR_DESTINATION_AMOUNT_INPUT = "Incorrect PNR Destination Amount";
        public static string INCORRECT_BANK_CODE_INPUT = "Incorrect Bank Code";
        public static string TOO_LONG_BANK_REFERENCE_CODE_INPUT = "Too Long Bank Reference Code";
        public static string TOO_LONG_CUSTOMER_NAME_INPUT = "Too Long Customer Name";


    }
}
