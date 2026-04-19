namespace ClubeDaLeitura.ConsoleApp.Shared.Base;

public abstract class BaseUI<T> where T : BaseEntity<T>
{
    public IRepository<T> Repository { get; }
    protected BaseUI(IRepository<T> repository)
    {
        Repository = repository;
    }
    public int RepoCount => Repository.Count();
    public bool RepoHasAny => RepoCount > 0;
    public abstract void Menu();
    public abstract void Add();
    public abstract void Edit();
    public abstract void Remove();
    public abstract void View();
    public abstract T Select(string? title = null, List<T>? ignoredEntities = null);
    protected List<T> GetAvailable(IEnumerable<T>? ignoredEntities = null)
    {
        ignoredEntities ??= [];
        return Repository.GetAll().Where(e => !ignoredEntities.Contains(e)).ToList();
    }
}