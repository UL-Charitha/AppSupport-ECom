using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace PayLater.Domain.Util
{
    public class Helper
    {
        public PnrClientWrapper CreatePnrClientResponse(Pnr response)
        {
            PnrClientWrapper clientResponse = new PnrClientWrapper();
            clientResponse.pnr = response.pnrID;
            clientResponse.pnrTotalValue = response.totalAmountDestination;
            clientResponse.currency = response.DestinationCurrency;
            clientResponse.expiry = response.expDate;
            clientResponse.responseCode = response.responseCode;
            clientResponse.responseMessage = response.msg;
            if (response.paxList != null)
            {
                clientResponse.paxList = string.Join(",", response.paxList.ToArray());
            }

            return clientResponse;
        }

        public PaymentClientWrapper CreatePaymentClientResponse(Payment response)
        {
            PaymentClientWrapper clientResponse = new PaymentClientWrapper();
            clientResponse.transactionId = response.paymentId;
            if (response.paymentId == 0)
            {
                clientResponse.transactionId = null;
            }
            clientResponse.bankCode = response.bankCode;
            clientResponse.bankRef = response.bankRef;
            clientResponse.pnr = response.pnrID;
            clientResponse.currency = response.currencyDest;
            clientResponse.amount = response.totalAmountDest;
            clientResponse.customerName = response.customerName;
            clientResponse.responseCode = response.responseCode;
            clientResponse.responseMessage = response.msg;
            clientResponse.transactionDate = response.transactionDate;
            return clientResponse;
        }

        public SearchClientWrapper CreateSearchClientResponse(Payment response)
        {
            SearchClientWrapper clientResponse = new SearchClientWrapper();
            clientResponse.transactionId = response.paymentId;
            if (response.paymentId == 0)
            {
                clientResponse.transactionId = null;
            }            
            clientResponse.bankRef = response.bankRef;
            clientResponse.pnr = response.pnrID;
            clientResponse.currency = response.currencyDest;
            clientResponse.amount = response.totalAmountDest;
            clientResponse.customerName = response.customerName;
            clientResponse.responseCode = response.responseCode;
            clientResponse.responseMessage = response.msg;
            clientResponse.transactionDate = response.transactionDate;
            return clientResponse;
        }

        public Payment CreatePaymentDomainRequest(PaymentClientWrapper clientResponse)
        {
            Payment response = new Payment();
            response.bankCode = clientResponse.bankCode;
            response.bankRef = clientResponse.bankRef;
            response.pnrID = clientResponse.pnr;
            response.currencyDest = clientResponse.currency;
            response.totalAmountDest = clientResponse.amount;
            response.customerName = clientResponse.customerName;
            return response;
        }

        public StringBuilder GetSavePaymentFailSB(Payment paymentRes)
        {
            StringBuilder sb = new StringBuilder("save payment failed/n");
            sb.Append(Environment.NewLine);
            sb.Append("PNR - " + paymentRes.pnrID);
            sb.Append(Environment.NewLine);
            sb.Append("Amount Orig - " + paymentRes.totalAmountOrig);
            sb.Append(Environment.NewLine);
            sb.Append("Currency Orig - " + paymentRes.currencyOrig);
            sb.Append(Environment.NewLine);
            sb.Append("Amount Dest" + paymentRes.totalAmountDest);
            sb.Append(Environment.NewLine);
            sb.Append("Currency Orig" + paymentRes.currencyDest);
            sb.Append(Environment.NewLine);
            sb.Append("Bank Ref" + paymentRes.bankRef);
            sb.Append(Environment.NewLine);
            sb.Append("Bank Code" + paymentRes.bankCode);
            sb.Append(Environment.NewLine);
            sb.Append("Customer name" + paymentRes.customerName);
            sb.Append(Environment.NewLine);
            sb.Append("Transaction date" + paymentRes.transactionDate);
            sb.Append(Environment.NewLine);
            sb.Append("Expiry Date" + paymentRes.expDate);
            sb.Append(Environment.NewLine);
            sb.Append("Status code" + paymentRes.statusCode);
            sb.Append(Environment.NewLine);
            sb.Append("Response code" + paymentRes.responseCode);
            sb.Append(Environment.NewLine);
            sb.Append("Message" + paymentRes.msg);
            sb.Append(Environment.NewLine);

            return sb;
        }



        public TicketInfoClientWrapper CreateTicketInfoClientResponse(TicketInfo ticketInfo)
        {
            TicketInfoClientWrapper response = new TicketInfoClientWrapper();

            response.transactionId = ticketInfo.paymentId;
            response.bankRef = ticketInfo.bankRef;
            response.pnr = ticketInfo.pnrID;
            response.responseCode = ticketInfo.responseCode;
            response.responseMessage = ticketInfo.msg;
            response.amount = ticketInfo.TotalPrice;
            response.currency = ticketInfo.TotalPriceCurrency;
            if (ticketInfo.ticketInformation!=null)
            {
                response.ticketInformation = ticketInfo.ticketInformation.Trim().Replace("\r\n", "");
            }
            return response;
        }

        public string GenerateHtmlTicketInformation(TicketInfo ticketInfo)
        {
            string paxTickets = "";
            string itineraryInfo = "";

            string template = @" 
<html>
<body>
<h4>Booking Reference(PNR)</h4>
<p>xxxpnrID</p>
<h4>Passenger Information</h4>
<table style=' border:1px black; border-collapse: collapse; ' border='1'>
<tr>
<th>Passenger</th>
<th>Ticket No</th>
</tr>
xxxpaxTickets
</table>

<h4>Itinerary Information</h4>
<table style=' border:1px black; border-collapse: collapse; text-align:center' border='1'>
<tr>
<th>Flight</th>
<th>Departure Station</th>
<th>Departure Time</th>
<th>Arrival Station</th>
<th>Arrival Time</th>
</tr>
xxxitineraryInfo
</table>
<h4>Price</h4>
<table>
<tr>
<td>
Total Amount : <b>xxxTotalPrice</b>
</td>
</tr>
</table>
</body>
</html>";
            // Replace item places
            template = template.Replace("xxxpnrID", ticketInfo.pnrID);

            //Set paxTickets
            foreach (var ticketItem in ticketInfo.ticketList)
            {
                paxTickets += string.Format("<tr> <td>{0}</td><td>{1}</td></tr>", ticketItem.paxLName + " " + ticketItem.paxFName, ticketItem.ticket);
            }
            template = template.Replace("xxxpaxTickets", paxTickets);

            //Set Itinerary Information
            foreach (var itineraryInfoItem in ticketInfo.itineraryInfoList)
            {
                itineraryInfo += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", itineraryInfoItem.flightID, itineraryInfoItem.stationDep, itineraryInfoItem.dateTimeDep.ToString(), itineraryInfoItem.stationArr, itineraryInfoItem.dateTimeArr.ToString());
            }
            template = template.Replace("xxxitineraryInfo", itineraryInfo);

            //Set Price
            template = template.Replace("xxxTotalPrice", ticketInfo.TotalPriceCurrency + " " + ticketInfo.TotalPrice);


            // return final Set
            return template;
        }

        
    }
}
