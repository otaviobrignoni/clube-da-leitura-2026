using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class LoanUI
{
    ComicBookUI ComicBookUI;
    FriendUI FriendUI;
    public ILoanRepo Repository;
    public LoanUI(ComicBookUI comicBookUI, FriendUI friendUI, ILoanRepo loanRepo)
    {
        ComicBookUI = comicBookUI;
        FriendUI = friendUI;
        Repository = loanRepo;
    }
    public void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar empréstimos", Colours.Title);
        string[] options = ["Emprestar revista", "Devolver revista", "Visualizar empréstimos", "Voltar"];
        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    Open();
                    break;
                case 1:
                    Return();
                    break;
                case 2:
                    View();
                    break;
                case 3:
                    return;
            }
    }
    public void Open()
    {
        if (!ComicBookUI.RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhuma revista cadastrada para emprestar", type: MessageType.Warning);
            return;
        }
        if (!FriendUI.RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhum amigo cadastrado para emprestar uma revista", type: MessageType.Warning);
            return;
        }
        Loan loan = new(SelectValidFriend(), SelectValidComicBook());
        loan.Friend.AddLoan(loan);
        loan.ComicBook.ChangeStatus();
        Repository.Add(loan);
    }
    public void Return()
    {
        var openLoans = Repository.GetAll().Where(l => l.CurrentStatus == LoanStatus.Open || l.CurrentStatus == LoanStatus.Late).ToList();
        if (openLoans.Count == 0)
        {
            Utils.MsgBox("Info", "Não há revistas para retornar.", type: MessageType.Info);
            return;
        }
        Loan loan = Select("Concluir empréstimo", openLoans);
        loan.ReturnComicBook();
    }

    public Loan Select(string? title = null, List<Loan>? availableLoans = null)
    {
        title ??= "Selecionar empréstimo";
        title = Utils.ColourStringHex(title, Colours.Title);
        availableLoans ??= Repository.GetAll().ToList();
        string[] options = availableLoans.Select(l => $"{l.Friend.Name}, {l.ComicBook.Title} N°{l.ComicBook.Edition}, Status: {Utils.ColourStringHex(l.StatusString, l.StatusColour)}").ToArray();
        return availableLoans[Utils.Menu(title, options)];
    }
    public void View()
    {
        if (Repository.Count() < 1)
        {
            Utils.MsgBox("Info", "Não há empréstimos.", type: MessageType.Info);
            return;
        }
        string title = Utils.ColourStringHex("Empréstimos", Colours.Title);
        List<string[]> loans = [];
        var orderedLoans = Repository.GetAll()
                                     .OrderBy(l => l.StatusOrder)
                                     .ThenBy(l => l.OpenedDate);
        foreach (Loan l in orderedLoans)
            loans.Add([l.Friend.Name, $"{l.ComicBook.Title} N°{l.ComicBook.Edition}", $"{l.OpenedDate}", $"{l.ReturnDate}", l.ReturnedDate == null ? "Não" : $"{l.ReturnedDate}", Utils.ColourStringHex(l.StatusString, l.StatusColour)]);
        Utils.GenerateTable(title, Loan.Categories, loans.ToArray());
    }
    public Friend SelectValidFriend()
    {
        while (true)
        {
            Friend friend = FriendUI.Select("Selecionar amigo fazendo empréstimo");
            if (friend.HasOpenLoan)
            {
                Utils.MsgBox("Aviso", "Esse amigo tem um empréstimo aberto. Conclua-o para fazer outro.", type: MessageType.Warning);
                continue;
            }
            return friend;
        }
    }
    public ComicBook SelectValidComicBook()
    {
        while (true)
        {
            ComicBook comicBook = ComicBookUI.Select("Selecionar revista sendo emprestada");
            if (!comicBook.IsAvailable)
            {
                Utils.MsgBox("Aviso", "Essa revista não está disponível.", type: MessageType.Warning);
                continue;
            }
            return comicBook;
        }
    }
}
