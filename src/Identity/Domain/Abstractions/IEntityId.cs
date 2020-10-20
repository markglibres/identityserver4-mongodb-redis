namespace Identity.Domain.Abstractions
{
    public interface IEntityId<out T> : IEntityId
    {
        
    }

    public interface IEntityId
    {
        string ToString();
    }
}