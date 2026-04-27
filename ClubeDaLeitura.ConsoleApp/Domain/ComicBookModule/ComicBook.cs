using ClubeDaLeitura.ConsoleApp.Shared.Base;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Domain.BoxModule;

namespace ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;

public class ComicBook : BaseEntity<ComicBook>
{
    public string Title { get; internal set; } = string.Empty;
    public int Edition { get; internal set; }
    public DateOnly ReleaseDate { get; internal set; }
    public Box Box { get; internal set; }
    public ComicBookStatus CurrentStatus { get; internal set; } = ComicBookStatus.Available;
    public bool IsAvailable => CurrentStatus == ComicBookStatus.Available;
    public string StatusString => CurrentStatus switch
    {
        ComicBookStatus.Available => "Disponível",
        ComicBookStatus.Loaned => "Emprestada",
        ComicBookStatus.Reserved => "Reservada",
        _ => string.Empty
    };
    public string StatusColour => CurrentStatus switch
    {
        ComicBookStatus.Available => Colours.LightGreen,
        ComicBookStatus.Loaned => Colours.LightRed,
        ComicBookStatus.Reserved => Colours.LightYellow,
        _ => Colours.White
    };
    public static readonly string[] Categories = ["Título", "Edição", "Data de publicação", "Caixa", "Status"];
    public ComicBook(string title, int edition, DateOnly releaseDate, Box box)
    {
        Title = title;
        Edition = edition;
        ReleaseDate = releaseDate;
        Box = box;
    }
    public ComicBook(ComicBook comicBook) : this(comicBook.Title, comicBook.Edition, comicBook.ReleaseDate, comicBook.Box) { }
    public void SetLoaned() => CurrentStatus = ComicBookStatus.Loaned;
    public void SetReserved() => CurrentStatus = ComicBookStatus.Reserved;
    public void SetAvailable() => CurrentStatus = ComicBookStatus.Available;
    public override void UpdateEntity(ComicBook updatedComicBook)
    {
        if (Box != updatedComicBook.Box)
        {
            Box.RemoveComicBook(Id);
            updatedComicBook.Box.AddComicBook(this);
            Box = updatedComicBook.Box;
        }
        Title = updatedComicBook.Title;
        Edition = updatedComicBook.Edition;
        ReleaseDate = updatedComicBook.ReleaseDate;
    }
    public override bool Equals(ComicBook comicBook)
    {
        if (comicBook.Title != Title
            || comicBook.Edition != Edition
            || comicBook.ReleaseDate != ReleaseDate
            || comicBook.Box != Box)
            return false;
        return true;
    }
}
