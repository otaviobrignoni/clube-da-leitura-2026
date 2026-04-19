using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.BoxModule;

public class Box : BaseEntity<Box>
{
    public string Tag { get; internal set; } = string.Empty;
    public string Colour { get; internal set; } = string.Empty;
    public int LoanDays { get; internal set; } = 7;
    public HashSet<ComicBook> ComicBooks { get; } = [];
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
    public override void UpdateEntity(Box updatedBox)
    {
        Tag = updatedBox.Tag;
        Colour = updatedBox.Colour;
        LoanDays = updatedBox.LoanDays;
    }
    public override bool Equals(Box box)
    {
        if (box.Tag != Tag
            || box.Colour != Colour
            || box.LoanDays != LoanDays)
            return false;
        return true;
    }
}
