using System.Text;
using ClubeDaLeitura.ConsoleApp.Display;
using ClubeDaLeitura.ConsoleApp.Domain.BoxModule;
using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Infrastructure;
using ClubeDaLeitura.ConsoleApp.Shared;

namespace ClubeDaLeitura.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string title = Utils.ColourStringHex("Clube da Leitura", Colours.Title);
        string[] options = ["Caixas", "Revistas", "Amigos", "Empréstimos", "Sair"];

        BoxRepo boxRepo = new();
        ComicBookRepo comicBookRepo = new();
        FriendRepo friendRepo = new();
        LoanRepo loanRepo = new();

        BoxUI bUI = new(boxRepo);
        ComicBookUI cUI = new(bUI, comicBookRepo);
        FriendUI fUI = new(friendRepo);
        LoanUI lUI = new(cUI, fUI, loanRepo);

        boxRepo.Add(new Box("bx1", "#ff0000", 5));
        boxRepo.Add(new Box("bx2", "#00ff00", 7));
        boxRepo.Add(new Box("cb3", "#0000ff", 2));

        friendRepo.Add(new Friend("fr1", "pa1", "(00) 00000-0000"));
        friendRepo.Add(new Friend("fr2", "pa1", "(00) 00000-0000"));
        friendRepo.Add(new Friend("fr3", "pa1", "(00) 00000-0000"));
        friendRepo.Add(new Friend("fr4", "pa1", "(00) 00000-0000"));

        comicBookRepo.Add(new ComicBook("rev1", 1, DateOnly.Parse("11/11/1111"), boxRepo.GetAll().First(b => !b.HasComicBook)));
        comicBookRepo.Add(new ComicBook("rev2", 12, DateOnly.Parse("12/11/1111"), boxRepo.GetAll().First(b => !b.HasComicBook)));
        comicBookRepo.Add(new ComicBook("rev3", 123, DateOnly.Parse("13/11/1111"), boxRepo.GetAll().First(b => !b.HasComicBook)));

        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    bUI.Menu();
                    break;
                case 1:
                    cUI.Menu();
                    break;
                case 2:
                    fUI.Menu();
                    break;
                case 3:
                    lUI.Menu();
                    break;
                case 4:
                    return;
            }
    }
}
