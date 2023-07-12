using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Base.Application.Common.Persistence;
using Base.Domain.Common.Contracts;
using Identity.Infrastructure.Persistence.Context;
using Mapster;

namespace Identity.Infrastructure.Persistence.Repository;

// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public ApplicationDbRepository(IdentityDbContext dbContext)
        : base(dbContext)
    {
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    // This is only done when no Selector is defined, so regular specifications with a selector also still work.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();
    }
}