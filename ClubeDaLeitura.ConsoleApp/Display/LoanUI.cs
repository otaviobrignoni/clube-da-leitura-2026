using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class LoanUI : BaseUI<Loan>, ILoanUI
{
    private readonly IComicBookUI ComicBookUI;
    private readonly IFriendUI FriendUI;
    public LoanUI(IComicBookUI comicBookUI, IFriendUI friendUI, ILoanRepo loanRepo) : base(loanRepo)
    {
        ComicBookUI = comicBookUI;
        FriendUI = friendUI;
    }
    public override void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar empréstimos", Colours.Title);
        string[] options = ["Emprestar revista", "Devolver revista", "Visualizar empréstimos", "Voltar"];
        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    Add();
                    break;
                case 1:
                    Edit();
                    break;
                case 2:
                    View();
                    break;
                case 3:
                    return;
            }
    }

    public override void Add()
    {
        Open();
    }

    public override void Edit()
    {
        Return();
    }

    public override void Remove()
    {
        Utils.MsgBox("Info", "Operação de remoção não se aplica a empréstimos.", type: MessageType.Info);
    }

    public void Open()
    {
        if (!ComicBookUI.RepoHasAny())
        {
            Utils.MsgBox("Aviso", "Nenhuma revista cadastrada para emprestar.", type: MessageType.Warning);
            return;
        }
        if (!FriendUI.RepoHasAny())
        {
            Utils.MsgBox("Aviso", "Nenhum amigo cadastrado para emprestar uma revista.", type: MessageType.Warning);
            return;
        }
        var validComicBooks = ComicBookUI.GetAll().Where(cb => cb.IsAvailable).ToList();
        if (validComicBooks.Count < 1)
        {
            Utils.MsgBox("Aviso", "Nenhuma revista está disponível.", type: MessageType.Warning);
            return;
        }
        var validFriends = FriendUI.GetAll().Where(f => !f.HasOpenLoan).ToList();
        if (validFriends.Count < 1)
        {
            Utils.MsgBox("Aviso", "Todos os amigos cadastrados já têm um empréstimo aberto.", type: MessageType.Warning);
            return;
        }
        var ignoredFriends = FriendUI.GetAll().Except(validFriends).ToList();
        var ignoredComicBooks = ComicBookUI.GetAll().Except(validComicBooks).ToList();
        Loan loan = new(SelectValidFriend(ignoredFriends), SelectValidComicBook(ignoredComicBooks));
        if (!loan.Friend.AddLoan(loan))
        {
            Utils.MsgBox("Erro", "Ocorreu um erro na abertura do empréstimo.", type: MessageType.Error);
            return;
        }
        loan.ComicBook.ChangeStatus();
        Repository.Add(loan);
    }
    public void Return()
    {
        var openLoans = Repository.GetAll().Where(l => l.CurrentStatus == LoanStatus.Open || l.CurrentStatus == LoanStatus.Late).ToList();
        var ignoredLoans = Repository.GetAll().Except(openLoans).ToList();
        if (openLoans.Count == 0)
        {
            Utils.MsgBox("Info", "Não há revistas para retornar.", type: MessageType.Info);
            return;
        }
        Loan loan = Select("Concluir empréstimo", ignoredLoans);
        loan.ReturnComicBook();
    }

    public override Loan Select(string? title = null, List<Loan>? ignoredLoans = null)
    {
        title ??= "Selecionar empréstimo";
        title = Utils.ColourStringHex(title, Colours.Title);
        var availableLoans = GetAvailable(ignoredLoans);
        string[] options = availableLoans.Select(l => $"{l.Friend.Name}, {l.ComicBook.Title} N°{l.ComicBook.Edition}, Status: {Utils.ColourStringHex(l.StatusString, l.StatusColour)}").ToArray();
        return availableLoans[Utils.Menu(title, options)];
    }

    public override void View()
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
    public Friend SelectValidFriend(List<Friend> ignoredFriends)
    {
        return FriendUI.Select("Selecionar amigo fazendo empréstimo", ignoredFriends);
    }
    public ComicBook SelectValidComicBook(List<ComicBook> ignoredComicBooks)
    {
        return ComicBookUI.Select("Selecionar revista sendo emprestada", ignoredComicBooks);
    }

}
