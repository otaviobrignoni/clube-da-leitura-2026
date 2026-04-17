namespace ClubeDaLeitura.ConsoleApp.Shared.Base;

public interface IRepository<T> where T : BaseEntity<T>
{
    public void Add(T entity);
    public bool Edit(Guid id, T updatedEntity);
    public bool Remove(Guid id);
    public bool TryGetEntity(Guid id, out T? entity);
    public IEnumerable<T> GetAll();
    public int Count();
}
