using System.Text.Json.Serialization;

namespace Microsoft.Dynamics.Client.Model
{
    public class AccountsResponseDTO
    {
        [JsonPropertyName("@odata.context")]
        public string? ODataContext { get; set; }

        [JsonPropertyName("value")]
        public List<AccountDTO> Value { get; set; } = new List<AccountDTO>();
    }
}
