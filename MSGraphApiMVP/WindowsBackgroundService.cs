using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace MSGraphApiMVP;

class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly GraphServiceClient _graphServiceClient;
    private readonly GraphServiceConfiguration _configuration;

    public TimeSpan DelayBetweenRequests { get; set; } = TimeSpan.FromSeconds(1);

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger,
        GraphServiceClient graphServiceClient,
        GraphServiceConfiguration configuration) {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        string[] departments = {
            "Marketing",
            "Sales",
            "Retail",
            "HR",
            "Operations",
            "Finance",
            "Executive Management",
            "Engineering",
        };

        string[] fields = {
            "id",
            "displayName",
            "department",
            "jobTitle",
            "mail",
            "mobilePhone",
            "officeLocation",
            "preferredLanguage",
            "surname",
            "userPrincipalName",
        };

        while (true) {
            // **Data acquisition**

            // Get ids of users in Marketing department 
            //var users = await GetUsersAsync(cancellationToken,
            //    department: new[] { "Marketing" },
            //    select: new[] { "id" });

            // Get random selected fields of random users in random departments
            //var users = await GetUsersAsync(cancellationToken,
            //    department: RandomSelection(departments),
            //    select: RandomSelection(fields));

            // Get users filtered and selected by configuration
            var users = await GetUsersAsync(cancellationToken,
                department: _configuration.Departments,
                select: _configuration.Fields,
                orderBy: _configuration.OrderBy);

            // **Logging**

            // Log only display names
            //_logger.LogInformation("Returned users: {Users}",
            //    users is null ? "Null" : string.Join(", ", users!.Select(u => u.DisplayName)));

            // Log all fields - json formatted
            //var stringed = DumpObject(users);
            //_logger.LogInformation($"Returned users: {{{nameof(users)}}}", stringed);

            // Log only count of users
            //_logger.LogInformation("Returned users: {Count}", users?.Count ?? 0);

            // Log based on configuration
            switch (_configuration.LogObject) {
                case LogObject.Count:
                    _logger.LogInformation("Returned users: {Count}", users?.Count ?? 0);
                    break;
                case LogObject.Json:
                    _logger.LogInformation("Returned {Count} users : {Users}", users?.Count ?? 0, DumpObject(users));
                    break;
                case LogObject.DisplayName:
                    _logger.LogInformation("Returned users: {Users}",
                        users is null ? "Null" : string.Join(", ", users!.Select(u => u.DisplayName)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(DelayBetweenRequests, cancellationToken);
        }
    }

    public async Task<ICollection<User>?> GetUsersAsync(CancellationToken cancellationToken,
        ICollection<string>? ids = null,
        ICollection<string>? department = null,
        ICollection<string>? select = null,
        ICollection<string>? orderBy = null) {

        var service = _graphServiceClient.Users;

        List<string> filters = new();
        if (!department.IsNullOrEmpty()) {
            filters.Add(department.Count == 1
                ? $"department eq '{department.First()}'"
                : $"department in ({StringifyParamsList(department)})");
        }

        if (!ids.IsNullOrEmpty()) {
            filters.Add(ids.Count == 1
                ? $"id eq '{ids.First()}'"
                : $"id in ({StringifyParamsList(ids)})");
        }

        var filter = string.Join(" and ", filters);
        _logger.LogDebug(@"Getting users with filter: {filter}", filter);

        _logger.LogDebug("Getting fields: {fields}", string.Join(", ", select ?? new string[] { }));

        var users = await service.GetAsync(configuration => {
            var parameters = configuration.QueryParameters = new UsersRequestBuilder.UsersRequestBuilderGetQueryParameters();
            if (filter.Length > 0)
                parameters.Filter = filter;

            parameters.Select = select?.ToArray();

            parameters.Orderby = orderBy?.ToArray();

            configuration.Options.WithAppOnly();
        }, cancellationToken: cancellationToken);

        return users?.Value;
    }

    private string StringifyParamsList(ICollection<string> list) {
        if (list.IsNullOrEmpty())
            throw new ArgumentException("collection must not be `null` or empty", nameof(list));

        return string.Join(',', list.Select(item => $"'{item}'"));
    }

    private string DumpObject(object? obj) {
        var options = new JsonSerializerOptions {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(obj, options);
    }

    private T[] RandomSelection<T>(ICollection<T> collection) {
        return collection.OrderBy(_ => Random.Shared.NextInt64())
            .Take(Random.Shared.Next(collection.Count))
            .ToArray();
    }
}