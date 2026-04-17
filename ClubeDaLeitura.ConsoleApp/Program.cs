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
        BoxUI bUI = new(boxRepo);

        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    bUI.Menu();
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    return;
            }
    }
}
