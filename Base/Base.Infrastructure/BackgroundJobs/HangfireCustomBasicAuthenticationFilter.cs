using System.Net.Http.Headers;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

namespace Base.Infrastructure.BackgroundJobs;

public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
{
    private const string _AuthenticationScheme = "Basic";
    private readonly ILogger _logger;

    public HangfireCustomBasicAuthenticationFilter()
        : this(new NullLogger<HangfireCustomBasicAuthenticationFilter>())
    {
    }

    public HangfireCustomBasicAuthenticationFilter(ILogger logger)
    {
        _logger = logger;
    }

    public string User { get; set; } = default!;
    public string Pass { get; set; } = default!;

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var header = httpContext.Request.Headers["Authorization"];

        if (MissingAuthorizationHeader(header))
        {
            _logger.LogInformation("Request is missing Authorization Header");
            SetChallengeResponse(httpContext);
            return false;
        }

        var authValues = AuthenticationHeaderValue.Parse(header);

        if (NotBasicAuthentication(authValues))
        {
            _logger.LogInformation("Request is NOT BASIC authentication");
            SetChallengeResponse(httpContext);
            return false;
        }

        var tokens = ExtractAuthenticationTokens(authValues);

        if (tokens.AreInvalid())
        {
            _logger.LogInformation("Authentication tokens are invalid (empty, null, whitespace)");
            SetChallengeResponse(httpContext);
            return false;
        }

        if (tokens.CredentialsMatch(User, Pass))
        {
            _logger.LogInformation("Awesome, authentication tokens match configuration!");
            return true;
        }

        _logger.LogInformation(
            $"Boo! Authentication tokens [{tokens.Username}] [{tokens.Password}] do not match configuration");

        SetChallengeResponse(httpContext);
        return false;
    }

    private static bool MissingAuthorizationHeader(StringValues header)
    {
        return string.IsNullOrWhiteSpace(header);
    }

    private static BasicAuthenticationTokens ExtractAuthenticationTokens(AuthenticationHeaderValue authValues)
    {
        var parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter!));
        var parts = parameter.Split(':');
        return new BasicAuthenticationTokens(parts);
    }

    private static bool NotBasicAuthentication(AuthenticationHeaderValue authValues)
    {
        return !_AuthenticationScheme.Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase);
    }

    private static void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}

public class BasicAuthenticationTokens
{
    private readonly string[] _tokens;

    public BasicAuthenticationTokens(string[] tokens)
    {
        _tokens = tokens;
    }

    public string Username => _tokens[0];
    public string Password => _tokens[1];

    public bool AreInvalid()
    {
        return ContainsTwoTokens() && ValidTokenValue(Username) && ValidTokenValue(Password);
    }

    public bool CredentialsMatch(string user, string pass)
    {
        return Username.Equals(user) && Password.Equals(pass);
    }

    private static bool ValidTokenValue(string token)
    {
        return string.IsNullOrWhiteSpace(token);
    }

    private bool ContainsTwoTokens()
    {
        return _tokens.Length == 2;
    }
}