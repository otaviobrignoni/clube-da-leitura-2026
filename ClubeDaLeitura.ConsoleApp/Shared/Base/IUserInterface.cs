namespace ClubeDaLeitura.ConsoleApp.Shared.Base;

public interface IUserInterface<T> where T : BaseEntity<T>
{
    public int RepoCount();
    public bool RepoHasAny();
    public IEnumerable<T> GetAll();
    public abstract T Select(string? title = null, List<T>? ignoredEntities = null);
}
