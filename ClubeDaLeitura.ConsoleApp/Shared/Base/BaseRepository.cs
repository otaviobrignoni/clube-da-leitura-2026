namespace ClubeDaLeitura.ConsoleApp.Shared.Base;

public class BaseRepository<T> where T : BaseEntity<T>
{
    protected readonly Dictionary<Guid, T> Entities = [];
    public void Add(T entity)
    {
        Guid id;
        do id = Guid.NewGuid();
        while (Entities.Any(e => e.Key == id));
        entity.Id = id;
        Entities.Add(entity.Id, entity);
    }
    public bool Edit(Guid id, T updatedEntity)
    {
        if (!TryGetEntity(id, out T? entity))
            return false;

        entity!.UpdateEntity(updatedEntity);
        return true;
    }
    public bool Remove(Guid id)
    {
        return Entities.Remove(id);
    }
    public bool TryGetEntity(Guid id, out T? entity)
    {
        return Entities.TryGetValue(id, out entity);
    }
    public IEnumerable<T> GetAll() => Entities.Values;
    public int Count() => Entities.Count;
}
