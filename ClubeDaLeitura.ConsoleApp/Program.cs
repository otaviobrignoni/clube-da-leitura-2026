using System.Text;
using ClubeDaLeitura.ConsoleApp.Display;
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
