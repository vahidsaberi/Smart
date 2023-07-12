namespace Identity.Infrastructure.Authentication;

public class SecuritySettings
{
    public string? Provider { get; set; }
    public bool RequireConfirmedAccount { get; set; }
}