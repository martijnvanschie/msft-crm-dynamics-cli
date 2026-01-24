using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Client.Model
{
    public class AccountDTO
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("_ownerid_value@OData.Community.Display.V1.FormattedValue")]
        public string? OwnerName { get; set; }

        [JsonPropertyName("_ownerid_value")]
        public string? OwnerId { get; set; }

        [JsonPropertyName("accountid")]
        public string? AccountId { get; set; }
    }
}
