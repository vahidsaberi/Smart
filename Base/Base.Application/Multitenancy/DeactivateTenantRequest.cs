namespace Base.Application.Multitenancy;

public class DeactivateTenantRequest : IRequest<string>
{
    public DeactivateTenantRequest(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; } = default!;
}

public class DeactivateTenantRequestValidator : CustomValidator<DeactivateTenantRequest>
{
    public DeactivateTenantRequestValidator()
    {
        RuleFor(t => t.TenantId)
            .NotEmpty();
    }
}

public class DeactivateTenantRequestHandler : IRequestHandler<DeactivateTenantRequest, string>
{
    private readonly ITenantService _tenantService;

    public DeactivateTenantRequestHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public Task<string> Handle(DeactivateTenantRequest request, CancellationToken cancellationToken)
    {
        return _tenantService.DeactivateAsync(request.TenantId);
    }
}