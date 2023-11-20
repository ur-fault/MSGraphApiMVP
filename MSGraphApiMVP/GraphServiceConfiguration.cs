using Microsoft.IdentityModel.Protocols.Configuration;

namespace MSGraphApiMVP;
internal class GraphServiceConfiguration
{
    public float DelayBetweenRequests { get; set; } = 1f;

    public string[]? Departments { get; set; }
    public string[]? Fields { get; set; }
    public string[]? OrderBy { get; set; }

    public LogObject LogObject { get; set; } = LogObject.Count;

    public GraphServiceConfiguration Validate() {
        if (OrderBy is not null && IsFiltering) {
            throw new InvalidConfigurationException(
                @"Cannot set both OrderBy and Departments, based on this restriction from Graph Api
                  https://learn.microsoft.com/en-us/graph/api/user-list-messages#using-filter-and-orderby-in-the-same-query");
        }

        return this;
    }

    public bool IsFiltering => (Departments?.Length ?? 0) > 0;
}

internal enum LogObject
{
    Count,
    Json,
    DisplayName,
}
