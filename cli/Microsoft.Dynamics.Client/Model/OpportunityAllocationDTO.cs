using System.Text.Json.Serialization;

namespace Microsoft.Dynamics.Client.Model
{
    public class OpportunityAllocationDTO
    {
        [JsonPropertyName("mac_opportunityallocationid")]
        public string? AllocationId { get; set; }

        [JsonPropertyName("mac_name")]
        public string? Name { get; set; }

        [JsonPropertyName("_mac_opportunity_value")]
        public string? OpportunityId { get; set; }

        [JsonPropertyName("_mac_serviceline_value")]
        public string? ServiceLineId { get; set; }

        [JsonPropertyName("_mac_serviceline_value@OData.Community.Display.V1.FormattedValue")]
        public string? ServiceLine { get; set; }

        [JsonPropertyName("mac_revenue")]
        public decimal? Revenue { get; set; }

        [JsonPropertyName("mac_revenue@OData.Community.Display.V1.FormattedValue")]
        public string? RevenueFormatted { get; set; }
    }

    public class OpportunityAllocationsResponseDTO
    {
        [JsonPropertyName("value")]
        public List<OpportunityAllocationDTO> Value { get; set; } = new();
    }
}
