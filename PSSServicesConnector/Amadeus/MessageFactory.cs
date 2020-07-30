using System;
using System.Collections.Generic;
using System.Text;
using PSSServicesConnector.AmadeusWebServices;

namespace com.amadeus.cs
{
    class MessageFactory
    {

        public static PNR_Retrieve BuildPnrRetrieveRequest(string pnr) 
        {
            PNR_Retrieve request = new PNR_Retrieve();
            request.retrievalFacts = new PNR_RetrieveRetrievalFacts();

            request.retrievalFacts.retrieve = new RetrievePNRType();
            request.retrievalFacts.retrieve.type = "2";
            //facts.reservationOrProfileIdentifier = new ReservationControlInformationDetailsType3[]();

            request.retrievalFacts.reservationOrProfileIdentifier = new ReservationControlInformationDetailsType3[1];
            request.retrievalFacts.reservationOrProfileIdentifier[0] = new ReservationControlInformationDetailsType3();
            request.retrievalFacts.reservationOrProfileIdentifier[0].controlNumber = pnr; //"2ZJXEP";
            //controlNumber = "2ZJXEP";//"2ZIEAG";//"2Y6U3V";//5PDAR4";"4A8KO3";4BIGQY; "5PDAR4";
            return request;
        }

        public static Security_SignOut buildSignOutRequest() {
            return new Security_SignOut();
        }
        //Air_FlightInfoGeneralFlightInfo generalInfo = new Air_FlightInfoGeneralFlightInfo();

        //generalInfo.companyDetails = new Air_FlightInfoGeneralFlightInfoCompanyDetails();
        //generalInfo.companyDetails.marketingCompany = "6X";

        //generalInfo.flightIdentification = new Air_FlightInfoGeneralFlightInfoFlightIdentification();
        //generalInfo.flightIdentification.flightNumber = "7725";

        //generalInfo.flightDate = new Air_FlightInfoGeneralFlightInfoFlightDate();
        //generalInfo.flightDate.departureDate = DateTime.Now.AddDays(7).ToString("ddMMyy");

        //Air_FlightInfo flightRequest = new Air_FlightInfo();
        //flightRequest.generalFlightInfo = generalInfo;

        //return flightRequest;

        public static Ticket_RetrieveListOfTSM BuildTicket_RetrieveListOfTSMRequest()
        {
            Ticket_RetrieveListOfTSM request = new Ticket_RetrieveListOfTSM();
            request.deadIndicator = new StatusTypeI7();
            request.deadIndicator.statusDetails = new StatusDetailsTypeI9();
            request.deadIndicator.statusDetails.indicator = "DED";
            request.deadIndicator.statusDetails.action = "0";

            return request;
        }

        public static PNR_Retrieve BuildPnrRetrieveRequestOld()
        {
            PNR_Retrieve request = new PNR_Retrieve();
            request.retrievalFacts = new PNR_RetrieveRetrievalFacts();
            RetrievePNRType RetrievePNRTypeObj = new RetrievePNRType();
            RetrievePNRTypeObj.type = "2";
            request.retrievalFacts.retrieve = RetrievePNRTypeObj;
            //facts.reservationOrProfileIdentifier = new ReservationControlInformationDetailsType3[]();
            request.retrievalFacts.reservationOrProfileIdentifier = null;
            //ReservationControlInformationDetailsType3[] res = new ReservationControlInformationDetailsType3[](); 
            ReservationControlInformationDetailsType3[] res = new ReservationControlInformationDetailsType3[1];
            ReservationControlInformationDetailsType3 obj = new ReservationControlInformationDetailsType3();
            //obj.controlNumber = "4A8KO3";4BIGQY; "5PDAR4"
            obj.controlNumber = "2ZJXEP";//"2ZIEAG";//"2Y6U3V";//5PDAR4";
            res[0] = obj;
            request.retrievalFacts.reservationOrProfileIdentifier = res;
            //request.retrievalFacts.reservationOrProfileIdentifier = res;
            return request;
        }



        public static Fare_ConvertCurrency BuildFareConvertRequest(string origCur, string destCur, double baseAmount)
        {
            Fare_ConvertCurrency request = new Fare_ConvertCurrency();
            request.message = new Fare_ConvertCurrencyMessage();
            request.message.messageFunctionDetails = new Fare_ConvertCurrencyMessageMessageFunctionDetails();
            request.message.messageFunctionDetails.messageFunction = "726";

            request.conversionDetails = new Fare_ConvertCurrencyConversionDetails[2];
            request.conversionDetails[0] = new Fare_ConvertCurrencyConversionDetails();

            request.conversionDetails[0].conversionDirection = new Fare_ConvertCurrencyConversionDetailsSelectionDetails[1];
            request.conversionDetails[0].conversionDirection[0] = new Fare_ConvertCurrencyConversionDetailsSelectionDetails();
            request.conversionDetails[0].conversionDirection[0].option = "706"; // From Direction

            request.conversionDetails[0].currencyInfo = new Fare_ConvertCurrencyConversionDetailsCurrencyInfo();
            request.conversionDetails[0].currencyInfo.conversionRateDetails = new Fare_ConvertCurrencyConversionDetailsCurrencyInfoConversionRateDetails();
            request.conversionDetails[0].currencyInfo.conversionRateDetails.currency = origCur; // "EUR";

            request.conversionDetails[0].amountInfo = new Fare_ConvertCurrencyConversionDetailsMonetaryDetails[1];
            request.conversionDetails[0].amountInfo[0] = new Fare_ConvertCurrencyConversionDetailsMonetaryDetails();
            request.conversionDetails[0].amountInfo[0].typeQualifier = "B";
            request.conversionDetails[0].amountInfo[0].amount = baseAmount.ToString(); // Base Amount

            request.conversionDetails[1] = new Fare_ConvertCurrencyConversionDetails();
            request.conversionDetails[1].conversionDirection = new Fare_ConvertCurrencyConversionDetailsSelectionDetails[1];
            request.conversionDetails[1].conversionDirection[0] = new Fare_ConvertCurrencyConversionDetailsSelectionDetails();
            request.conversionDetails[1].conversionDirection[0].option = "707"; // To Direction

            request.conversionDetails[1].currencyInfo = new Fare_ConvertCurrencyConversionDetailsCurrencyInfo();
            request.conversionDetails[1].currencyInfo.conversionRateDetails = new Fare_ConvertCurrencyConversionDetailsCurrencyInfoConversionRateDetails();
            request.conversionDetails[1].currencyInfo.conversionRateDetails.currency = destCur;//"AED";

            return request;
        }

        internal static FOP_CreateFormOfPayment BuildCreateFormOfPaymentRequest()
        {
            FOP_CreateFormOfPayment request = new FOP_CreateFormOfPayment();
            request.transactionContext = new TransactionInformationForTicketingType_174416S();
            request.transactionContext.transactionDetails = new TransactionInformationsType_245700C();
            request.transactionContext.transactionDetails.code = "FP";

            request.fopGroup = new FOP_CreateFormOfPaymentFopGroup[1];
            request.fopGroup[0] = new FOP_CreateFormOfPaymentFopGroup();
            request.fopGroup[0].fopReference = new ElementManagementSegmentType();
            request.fopGroup[0].mopDescription = new FOP_CreateFormOfPaymentFopGroupMopDescription[1];
            request.fopGroup[0].mopDescription[0] = new FOP_CreateFormOfPaymentFopGroupMopDescription();
            request.fopGroup[0].mopDescription[0].fopSequenceNumber = new SequenceDetailsTypeU();
            request.fopGroup[0].mopDescription[0].fopSequenceNumber.sequenceDetails = new SequenceInformationTypeU();
            request.fopGroup[0].mopDescription[0].fopSequenceNumber.sequenceDetails.number = "1";

            request.fopGroup[0].mopDescription[0].mopDetails = new FOP_CreateFormOfPaymentFopGroupMopDescriptionMopDetails();
            request.fopGroup[0].mopDescription[0].mopDetails.fopPNRDetails = new FormOfPaymentInformationType[1];
            request.fopGroup[0].mopDescription[0].mopDetails.fopPNRDetails[0] = new FormOfPaymentInformationType();
            request.fopGroup[0].mopDescription[0].mopDetails.fopPNRDetails[0].fopCode = "CASH";

            return request;
        }

        public static PNR_AddMultiElements BuildPNRAddMultiElementsRequest()
        {
            PNR_AddMultiElements request = new PNR_AddMultiElements();
            request.pnrActions = new string[1];
            request.pnrActions[0] = "11";

            request.dataElementsMaster = new PNR_AddMultiElementsDataElementsMaster();
            request.dataElementsMaster.marker1 = new DummySegmentTypeI2();
            request.dataElementsMaster.dataElementsIndiv = new PNR_AddMultiElementsDataElementsMasterDataElementsIndiv[1];
            request.dataElementsMaster.dataElementsIndiv[0] = new PNR_AddMultiElementsDataElementsMasterDataElementsIndiv();

            request.dataElementsMaster.dataElementsIndiv[0].elementManagementData = new ElementManagementSegmentType2();
            request.dataElementsMaster.dataElementsIndiv[0].elementManagementData.reference = new ReferencingDetailsType5();
            request.dataElementsMaster.dataElementsIndiv[0].elementManagementData.reference.qualifier = "OT";
            request.dataElementsMaster.dataElementsIndiv[0].elementManagementData.reference.number = "3";
            request.dataElementsMaster.dataElementsIndiv[0].elementManagementData.segmentName = "RF";

            request.dataElementsMaster.dataElementsIndiv[0].freetextData = new LongFreeTextType();
            request.dataElementsMaster.dataElementsIndiv[0].freetextData.freetextDetail = new FreeTextQualificationType();
            request.dataElementsMaster.dataElementsIndiv[0].freetextData.freetextDetail.subjectQualifier = "3";
            request.dataElementsMaster.dataElementsIndiv[0].freetextData.freetextDetail.type = "P22";
            request.dataElementsMaster.dataElementsIndiv[0].freetextData.longFreetext = "Internalchecks";

            return request;
        }

        internal static DocIssuance_IssueTicket BuildDocIssuance_IssueTicketRequest()
        {
            DocIssuance_IssueTicket request = new DocIssuance_IssueTicket();
            request.optionGroup = new DocIssuance_IssueTicketOptionGroup[2];
            request.optionGroup[0] = new DocIssuance_IssueTicketOptionGroup();
            request.optionGroup[0].switches = new StatusTypeI2();
            request.optionGroup[0].switches.statusDetails = new StatusDetailsTypeI2();
            request.optionGroup[0].switches.statusDetails.indicator = "ET";

            request.optionGroup[1] = new DocIssuance_IssueTicketOptionGroup();
            request.optionGroup[1].switches = new StatusTypeI2();
            request.optionGroup[1].switches.statusDetails = new StatusDetailsTypeI2();
            request.optionGroup[1].switches.statusDetails.indicator = "ITR";

            request.optionGroup[1].subCompoundOptions = new AttributeType2[1];
            request.optionGroup[1].subCompoundOptions[0] = new AttributeType2();
            request.optionGroup[1].subCompoundOptions[0].criteriaDetails = new AttributeInformationTypeU2();
            request.optionGroup[1].subCompoundOptions[0].criteriaDetails.attributeType = "EMPRA";// "EMPRA"=to:Customized.EMPRN removed

            return request;
        }

        public static DocIssuance_IssueCombined BuildDocIssuance_IssueCombinedRequest()
        {
            DocIssuance_IssueCombined request = new DocIssuance_IssueCombined();
            request.optionGroup = new DocIssuance_IssueCombinedOptionGroup[2];

            //request.optionGroup[0] = new DocIssuance_IssueCombinedOptionGroup();
            //request.optionGroup[0].switches = new StatusTypeI();
            //request.optionGroup[0].switches.statusDetails = new StatusDetailsTypeI();
            //request.optionGroup[0].switches.statusDetails.indicator = "ED"; // TKT >>> ED (by Chetan)
            // new //

            request.optionGroup[0] = new DocIssuance_IssueCombinedOptionGroup();
            request.optionGroup[0].switches = new StatusTypeI();
            request.optionGroup[0].switches.statusDetails = new StatusDetailsTypeI();
            request.optionGroup[0].switches.statusDetails.indicator = "ITR";
                                
            request.optionGroup[0].subCompoundOptions = new AttributeType[1];
            request.optionGroup[0].subCompoundOptions[0] = new AttributeType();
            request.optionGroup[0].subCompoundOptions[0].criteriaDetails = new AttributeInformationTypeU();
            request.optionGroup[0].subCompoundOptions[0].criteriaDetails.attributeType = "EMPRA"; // "EMPRN"=to:APE

            request.optionGroup[1] = new DocIssuance_IssueCombinedOptionGroup();
            request.optionGroup[1].switches = new StatusTypeI();
            request.optionGroup[1].switches.statusDetails = new StatusDetailsTypeI();
            request.optionGroup[1].switches.statusDetails.indicator = "EPR"; //changed from 'ITR' to solve EMD not issued case
                                
                                
            request.optionGroup[1].subCompoundOptions = new AttributeType[1];
            request.optionGroup[1].subCompoundOptions[0] = new AttributeType();
            request.optionGroup[1].subCompoundOptions[0].criteriaDetails = new AttributeInformationTypeU();
            request.optionGroup[1].subCompoundOptions[0].criteriaDetails.attributeType = "EMPRA"; // "EMPRN"=to:APE

            return request;
        }

        public static Queue_PlacePNR BuildCreateQueuePlacePNRRequest(string pnr, string queueId, string queueOfficeId)
        {
            Queue_PlacePNR request = new Queue_PlacePNR();
            request.placementOption = new SelectionDetailsTypeI2();
            request.placementOption.selectionDetails = new SelectionDetailsInformationTypeI3();
            request.placementOption.selectionDetails.option = "QEQ";

            request.targetDetails = new Queue_PlacePNRTargetDetails[1];
            request.targetDetails[0] = new Queue_PlacePNRTargetDetails();
            request.targetDetails[0].targetOffice = new AdditionalBusinessSourceInformationType1();
            request.targetDetails[0].targetOffice.sourceType = new SourceTypeDetailsTypeI();
            request.targetDetails[0].targetOffice.sourceType.sourceQualifier1 = "4"; // 3 => Get default Office ID Info

            request.targetDetails[0].targetOffice.originatorDetails = new OriginatorIdentificationDetailsTypeI3();
            request.targetDetails[0].targetOffice.originatorDetails.inHouseIdentification1 = queueOfficeId; // CMBUL08P1 / CMBUL07AE;

            request.targetDetails[0].queueNumber = new QueueInformationTypeI();
            request.targetDetails[0].queueNumber.queueDetails = new QueueInformationDetailsTypeI();
            request.targetDetails[0].queueNumber.queueDetails.number = queueId; // "2"; // Queue ID

            request.targetDetails[0].categoryDetails = new SubQueueInformationTypeI();
            request.targetDetails[0].categoryDetails.subQueueInfoDetails = new SubQueueInformationDetailsTypeI();
            request.targetDetails[0].categoryDetails.subQueueInfoDetails.identificationType = "C";
            request.targetDetails[0].categoryDetails.subQueueInfoDetails.itemNumber = "0";

            request.recordLocator = new ReservationControlInformationTypeI3();
            request.recordLocator.reservation = new ReservationControlInformationDetailsTypeI4();
            request.recordLocator.reservation.controlNumber = pnr; //"4A58AB"; // PNR to be queued.

            return request;
        }
    }
}
