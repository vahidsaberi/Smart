namespace Base.Application.Auditing;

public class GetMyAuditLogsRequest : IRequest<List<AuditDto>>
{
}

public class GetMyAuditLogsRequestHandler : IRequestHandler<GetMyAuditLogsRequest, List<AuditDto>>
{
    private readonly IAuditService _auditService;
    private readonly ICurrentUser _currentUser;

    public GetMyAuditLogsRequestHandler(ICurrentUser currentUser, IAuditService auditService)
    {
        (_currentUser, _auditService) = (currentUser, auditService);
    }

    public Task<List<AuditDto>> Handle(GetMyAuditLogsRequest request, CancellationToken cancellationToken)
    {
        return _auditService.GetUserTrailsAsync(_currentUser.GetUserId());
    }
}