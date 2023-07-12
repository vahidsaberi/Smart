namespace Base.Application.Multitenancy;

public class ActivateTenantRequest : IRequest<string>
{
    public ActivateTenantRequest(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; } = default!;
}

public class ActivateTenantRequestValidator : CustomValidator<ActivateTenantRequest>
{
    public ActivateTenantRequestValidator()
    {
        RuleFor(t => t.TenantId)
            .NotEmpty();
    }
}

public class ActivateTenantRequestHandler : IRequestHandler<ActivateTenantRequest, string>
{
    private readonly ITenantService _tenantService;

    public ActivateTenantRequestHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public Task<string> Handle(ActivateTenantRequest request, CancellationToken cancellationToken)
    {
        return _tenantService.ActivateAsync(request.TenantId);
    }
}