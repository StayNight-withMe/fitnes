namespace fitnes.Domain.Abstraction.Repositories;

public interface IEntity<out TKey>
{
    TKey Id { get; }
}
