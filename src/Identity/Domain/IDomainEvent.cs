namespace Identity.Domain
{
    public interface IDomainEvent
    {
        public string Id { get; }
        public string EntityId { get; }
    }
}