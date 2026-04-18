using ClubeDaLeitura.ConsoleApp.Shared.Base;
using ClubeDaLeitura.ConsoleApp.Domain.BoxModule;

namespace ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;

public class ComicBook : BaseEntity<ComicBook>
{
    public string Title = string.Empty;
    public int Edition;
    public DateOnly ReleaseDate;
    public Box Box;
    public static readonly string[] Categories = ["Título", "Edição", "Data de publicação", "Caixa"];
    public ComicBook(string title, int edition, DateOnly releaseDate, Box box)
    {
        Title = title;
        Edition = edition;
        ReleaseDate = releaseDate;
        Box = box;
    }
    public ComicBook(ComicBook comicBook) : this(comicBook.Title, comicBook.Edition, comicBook.ReleaseDate, comicBook.Box) { }
    public override void UpdateEntity(ComicBook updatedEntity)
    {
        if (Box != updatedEntity.Box)
        {
            Box.RemoveComicBook(Id);
            updatedEntity.Box.AddComicBook(this);
            Box = updatedEntity.Box;
        }
        Title = updatedEntity.Title;
        Edition = updatedEntity.Edition;
        ReleaseDate = updatedEntity.ReleaseDate;
    }
    public override bool Equals(ComicBook entity)
    {
        if (entity.Title != Title
            || entity.Edition != Edition
            || entity.ReleaseDate != ReleaseDate
            || entity.Box != Box)
            return false;
        return true;
    }
}
