using System.Text.Json.Serialization;

namespace Microsoft.Dynamics.Client.Model
{
    public class OpportunitiesResponseDTO
    {
        [JsonPropertyName("@odata.context")]
        public string? ODataContext { get; set; }

        [JsonPropertyName("value")]
        public List<OpportunityDTO> Value { get; set; } = [];
    }
}
