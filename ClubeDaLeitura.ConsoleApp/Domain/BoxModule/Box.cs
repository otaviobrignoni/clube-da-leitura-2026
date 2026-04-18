using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.BoxModule;

public class Box : BaseEntity<Box>
{
    public string Tag = string.Empty;
    public string Colour = string.Empty;
    public int LoanDays = 7;
    public HashSet<ComicBook> ComicBooks = [];
    public static readonly string[] Categories = ["Etiqueta", "Dias de empréstimo"];
    public Box(string tag, string colour, int loanDays)
    {
        Tag = tag;
        Colour = colour;
        LoanDays = loanDays;
    }
    public Box(Box box) : this(box.Tag, box.Colour, box.LoanDays) { }

    public bool HasComicBook => ComicBooks.Count > 0;
    public void AddComicBook(ComicBook comicBook)
    {
        ComicBooks.Add(comicBook);
    }
    public bool RemoveComicBook(Guid comicBookId)
    {
        ComicBook? comicBook = ComicBooks.FirstOrDefault(cb => cb.Id == comicBookId);
        if (comicBook is null) return false;
        return ComicBooks.Remove(comicBook);
    }
    public override bool Equals(Box entity)
    {
        if (entity.Tag != Tag
            || entity.Colour != Colour
            || entity.LoanDays != LoanDays)
            return false;
        return true;
    }

    public override void UpdateEntity(Box updatedEntity)
    {
        Tag = updatedEntity.Tag;
        Colour = updatedEntity.Colour;
        LoanDays = updatedEntity.LoanDays;
    }
}
