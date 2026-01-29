using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Dynamics.Client.Model
{
    public class OpportunityDTO
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("opportunityid")]
        public string? OpportunityId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("mac_opportunitynumber")]
        public string? OpportunityNumber { get; set; }

        [JsonPropertyName("mac_autonumberopportunity")]
        public string? AutoNumberOpportunity { get; set; }

        [JsonPropertyName("estimatedvalue")]
        public decimal? EstimatedValue { get; set; }

        [JsonPropertyName("estimatedvalue_base")]
        public decimal? EstimatedValueBase { get; set; }

        [JsonPropertyName("actualvalue")]
        public decimal? ActualValue { get; set; }

        [JsonPropertyName("actualvalue_base")]
        public decimal? ActualValueBase { get; set; }

        [JsonPropertyName("totalamount")]
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("totalamount_base")]
        public decimal? TotalAmountBase { get; set; }

        [JsonPropertyName("mac_weightedrevenuedecimal")]
        public decimal? WeightedRevenueDecimal { get; set; }

        [JsonPropertyName("closeprobability")]
        public int? CloseProbability { get; set; }

        [JsonPropertyName("estimatedclosedate")]
        public string? EstimatedCloseDate { get; set; }

        [JsonPropertyName("actualclosedate")]
        public string? ActualCloseDate { get; set; }

        [JsonPropertyName("createdon")]
        public string? CreatedOn { get; set; }

        [JsonPropertyName("modifiedon")]
        public string? ModifiedOn { get; set; }

        [JsonPropertyName("statecode@OData.Community.Display.V1.FormattedValue")]
        public string? StateCodeFormattedValue { get; set; }

        [JsonPropertyName("statecode")]
        public int? StateCode { get; set; }

        [JsonPropertyName("statuscode@OData.Community.Display.V1.FormattedValue")]
        public string? StatusCodeFormattedValue { get; set; }

        [JsonPropertyName("statuscode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("_customerid_value")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("_parentaccountid_value")]
        public string? ParentAccountId { get; set; }

        [JsonPropertyName("_parentcontactid_value")]
        public string? ParentContactId { get; set; }

        [JsonPropertyName("_ownerid_value@OData.Community.Display.V1.FormattedValue")]
        public string? OwnerFormattedValue { get; set; }

        [JsonPropertyName("_ownerid_value")]
        public string? OwnerId { get; set; }

        [JsonPropertyName("_owninguser_value")]
        public string? OwningUser { get; set; }

        [JsonPropertyName("_owningteam_value")]
        public string? OwningTeam { get; set; }

        [JsonPropertyName("_owningbusinessunit_value")]
        public string? OwningBusinessUnit { get; set; }

        [JsonPropertyName("_createdby_value")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("_modifiedby_value")]
        public string? ModifiedBy { get; set; }

        [JsonPropertyName("_createdonbehalfby_value")]
        public string? CreatedOnBehalfBy { get; set; }

        [JsonPropertyName("_modifiedonbehalfby_value")]
        public string? ModifiedOnBehalfBy { get; set; }

        [JsonPropertyName("_mac_servicelineid_value")]
        public string? ServiceLineId { get; set; }

        [JsonPropertyName("_mac_deliveryid_value")]
        public string? DeliveryId { get; set; }

        [JsonPropertyName("_mac_scid_value")]
        public string? ScId { get; set; }

        [JsonPropertyName("_mac_signeeid_value")]
        public string? SigneeId { get; set; }

        [JsonPropertyName("_mac_parentopportunityid_value")]
        public string? ParentOpportunityId { get; set; }

        [JsonPropertyName("_transactioncurrencyid_value")]
        public string? TransactionCurrencyId { get; set; }

        [JsonPropertyName("mac_opportunitytype")]
        public int? OpportunityType { get; set; }

        [JsonPropertyName("mac_probability@OData.Community.Display.V1.FormattedValue")]
        public string ProbabilityFormattedValue { get; set; }

        [JsonPropertyName("mac_probability")]
        public int? Probability { get; set; }

        [JsonPropertyName("mac_projectduration")]
        public int? ProjectDuration { get; set; }

        [JsonPropertyName("mac_projectstartdate")]
        public string? ProjectStartDate { get; set; }

        [JsonPropertyName("mac_strategic")]
        public bool? Strategic { get; set; }

        [JsonPropertyName("mac_winningstrategy")]
        public bool? WinningStrategy { get; set; }

        [JsonPropertyName("mac_delivery")]
        public bool? Delivery { get; set; }

        [JsonPropertyName("mac_commercial")]
        public bool? Commercial { get; set; }

        [JsonPropertyName("mac_contractcheck")]
        public bool? ContractCheck { get; set; }

        [JsonPropertyName("mac_signedorderconfirmation")]
        public bool? SignedOrderConfirmation { get; set; }

        [JsonPropertyName("mac_signedcontractporeceived")]
        public bool? SignedContractPoReceived { get; set; }

        [JsonPropertyName("mac_reviewbriefing")]
        public bool? ReviewBriefing { get; set; }

        [JsonPropertyName("mac_downselected")]
        public bool? Downselected { get; set; }

        [JsonPropertyName("mac_microsoftcosell")]
        public int? MicrosoftCosell { get; set; }

        [JsonPropertyName("mac_azurefunding")]
        public int? AzureFunding { get; set; }

        [JsonPropertyName("mac_azurerevenuemonthly")]
        public decimal? AzureRevenueMonthly { get; set; }

        [JsonPropertyName("mac_azurerevenuemonthly_base")]
        public decimal? AzureRevenueMonthlyBase { get; set; }

        [JsonPropertyName("mac_airelatedppportunity")]
        public bool? AiRelatedOpportunity { get; set; }

        [JsonPropertyName("mac_airelateddescription")]
        public string? AiRelatedDescription { get; set; }

        [JsonPropertyName("mac_openactivities")]
        public int? OpenActivities { get; set; }

        [JsonPropertyName("mac_openactivities_date")]
        public string? OpenActivitiesDate { get; set; }

        [JsonPropertyName("mac_datenextaction")]
        public string? DateNextAction { get; set; }

        [JsonPropertyName("mac_nexaction")]
        public string? NextAction { get; set; }

        [JsonPropertyName("stepname")]
        public string? StepName { get; set; }

        [JsonPropertyName("salesstage")]
        public int? SalesStage { get; set; }

        [JsonPropertyName("salesstagecode")]
        public int? SalesStageCode { get; set; }

        [JsonPropertyName("opportunityratingcode")]
        public int? OpportunityRatingCode { get; set; }

        [JsonPropertyName("prioritycode")]
        public int? PriorityCode { get; set; }

        [JsonPropertyName("msdyn_forecastcategory")]
        public int? ForecastCategory { get; set; }

        [JsonPropertyName("exchangerate")]
        public decimal? ExchangeRate { get; set; }

        [JsonPropertyName("versionnumber")]
        public long? VersionNumber { get; set; }

        [JsonPropertyName("isrevenuesystemcalculated")]
        public bool? IsRevenueSystemCalculated { get; set; }

        [JsonPropertyName("processid")]
        public string? ProcessId { get; set; }

        [JsonPropertyName("stageid")]
        public string? StageId { get; set; }

        [JsonPropertyName("traversedpath")]
        public string? TraversedPath { get; set; }
    }
}