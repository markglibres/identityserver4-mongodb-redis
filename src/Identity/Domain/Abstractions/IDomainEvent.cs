namespace Identity.Domain.Abstractions
{
    public interface IDomainEvent
    {
        public string Id { get; }
        public string EntityId { get; }
    }
}