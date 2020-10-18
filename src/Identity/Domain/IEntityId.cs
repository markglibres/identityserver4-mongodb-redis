namespace Identity.Domain
{
    public interface IEntityId<out T> : IEntityId
    {
        
    }

    public interface IEntityId
    {
        string ToString();
    }
}