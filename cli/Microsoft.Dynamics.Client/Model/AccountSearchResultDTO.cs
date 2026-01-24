using System.Text.Json.Serialization;

namespace Microsoft.Dynamics.Client.Model
{
    public class AccountSearchResultDTO
    {
        [JsonPropertyName("value")]
        public List<AccountDTO> Value { get; set; } = new List<AccountDTO>();
    }
}
