namespace Base.Domain.Common.Events;

public class EntityUpdatedEvent
{
    public static EntityUpdatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
        where TEntity : IEntity
    {
        return new EntityUpdatedEvent<TEntity>(entity);
    }
}

public class EntityUpdatedEvent<TEntity> : DomainEvent
    where TEntity : IEntity
{
    internal EntityUpdatedEvent(TEntity entity)
    {
        Entity = entity;
    }

    public TEntity Entity { get; }
}