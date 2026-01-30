namespace Microsoft.Dynamics.Client
{
    /// <summary>
    /// Search for opportunities by name
    /// </summary>
    /// <param name="Top">Maximum number of opportunities to return</param>
    /// <param name="UseStartsWith">If true, uses 'startswith' filter; otherwise uses 'contains' filter</param>
    /// <param name="IncludeClosed">If true, includes closed opportunities; otherwise only open opportunities (statecode = 0)</param>
    /// <returns>OpportunitiesRequestParameters containing the request parameters</returns>
    public class OpportunitiesRequestParameters
    {
        public int Top { get; set; } = 20;

        public bool UseStartsWith { get; set; } = true;

        public bool IncludeClosed { get; set; } = false;

        public string[]? SelectFields { get; set; }

        public string? Filter { get; set; }

        public string? Expand { get; set; }
    }
}
