namespace ClubeDaLeitura.ConsoleApp.Shared.Base;

public abstract class BaseUI<T> where T : BaseEntity<T>
{
    protected IRepository<T> Repository;
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
    public abstract T Select(List<T>? entities = null);
    protected List<T> GetAvailable(List<T>? entities = null)
    {
        entities ??= [];
        return Repository.GetAll().Where(e => !entities.Contains(e)).ToList();
    }
}