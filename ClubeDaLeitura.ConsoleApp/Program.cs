using System.Text;
using ClubeDaLeitura.ConsoleApp.Shared;

namespace ClubeDaLeitura.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        string title = "Clube da Leitura";
        string[] options = ["Caixas", "Revistas", "Amigos", "Empréstimos", "Sair"];
        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
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
