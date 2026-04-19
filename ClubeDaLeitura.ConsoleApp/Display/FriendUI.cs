using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class FriendUI : BaseUI<Friend>
{
    public FriendUI(IFriendRepo friendRepo) : base(friendRepo) { }
    public override void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar amigos", Colours.Title);
        string[] options = ["Cadastrar amigo", "Editar amigo", "Remover amigo", "Visualizar amigos", "Visualizar empréstimos de um amigo", "Voltar"];
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
                    Remove();
                    break;
                case 3:
                    View();
                    break;
                case 4:
                    ViewLoans();
                    break;
                case 5:
                    return;
            }
    }
    public override void Add()
    {
        string title = Utils.ColourStringHex("Cadastrar amigo", Colours.Title);
        var (name, phoneNumber) = GetValidNameAndPhoneNumber(title);
        Repository.Add(new Friend(name, Utils.GetValidString(title, "Nome do responsável: ", minLength: 3), phoneNumber));
        Utils.MsgBox("Sucesso", "Amigo cadastrado com sucesso!", type: MessageType.Success);
    }

    public override void Edit()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhum amigo cadastrado para editar.", type: MessageType.Warning);
            return;
        }

        string title = Utils.ColourStringHex("Editar amigo", Colours.Title);
        string[] options = ["Nome e Telefone", "Responsável", "Voltar"];

        Friend friend = Select("Selecionar amigo para editar");
        Friend editedFriend = new(friend);

        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    var (name, phoneNumber) = GetValidNameAndPhoneNumber(title);
                    editedFriend.Name = name;
                    editedFriend.PhoneNumber = phoneNumber;
                    break;
                case 1:
                    editedFriend.ParentName = Utils.GetValidString(title, "Nome do responsável: ", minLength: 3);
                    break;
                case 2:
                    if (!editedFriend.Equals(friend))
                    {
                        Utils.MsgBox("Sucesso", "Amigo editado com sucesso!", type: MessageType.Success);
                        friend.UpdateEntity(editedFriend);
                    }
                    return;
            }
    }

    public override void Remove()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhum amigo cadastrado para remover.", type: MessageType.Warning);
            return;
        }
        Friend friend = Select("Selecionar amigo para remover");
        if (friend.HasLoan)
        {
            Utils.MsgBox("Aviso", "Não é possível remover amigo com empréstimos vinculados.", type: MessageType.Warning);
            return;
        }
        if (Repository.Remove(friend.Id)) Utils.MsgBox("Sucesso", "Amigo removido com sucesso!", type: MessageType.Success);
        else Utils.MsgBox("Erro", "Erro ao remover amigo. Tente novamente.", type: MessageType.Error);
    }
    public override Friend Select(string? title = null, List<Friend>? entities = null)
    {
        title ??= "Selecionar amigo";
        title = Utils.ColourStringHex(title, Colours.Title);
        var availableFriends = GetAvailable(entities);
        string[] options = availableFriends.Select(f => $"{f.Name}, ID: {f.Id}").ToArray();
        return availableFriends[Utils.Menu(title, options)];
    }
    public override void View()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Info", "Nenhum amigo cadastrado.", type: MessageType.Info);
            return;
        }
        string title = Utils.ColourStringHex("Amigos", Colours.Title);
        List<string[]> friends = [];
        foreach (Friend f in Repository.GetAll())
            friends.Add([f.Name, f.ParentName, f.PhoneNumber]);
        Utils.GenerateTable(title, Friend.Categories, friends.ToArray());
    }
    public void ViewLoans()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Info", "Nenhum amigo cadastrado.", type: MessageType.Info);
            return;
        }
        if (Repository.GetAll().Where(f => f.HasLoan).ToList().Count < 1)
        {
            Utils.MsgBox("Info", "Nenhum amigo fez empréstimos.", type: MessageType.Info);
            return;
        }
        Friend friend = Select(entities: Repository.GetAll().Where(f => !f.HasLoan).ToList());
        string title = Utils.ColourStringHex($"Empréstimos de {friend.Name}", Colours.Title);
        List<string[]> loans = [];
        var orderedLoans = friend.Loans.OrderBy(l => l.StatusOrder)
                                       .ThenBy(l => l.OpenedDate);
        foreach (Loan l in orderedLoans)
            loans.Add([$"{l.ComicBook.Title} N°{l.ComicBook.Edition}", $"{l.OpenedDate}", $"{l.ReturnDate}", l.ReturnedDate == null ? "Não" : $"{l.ReturnedDate}", Utils.ColourStringHex(l.StatusString, l.StatusColour)]);
        Utils.GenerateTable(title, Loan.Categories[1..], loans.ToArray());
    }

    public (string, string) GetValidNameAndPhoneNumber(string title, List<Friend>? ignoredFriends = null)
    {
        while (true)
        {
            // maxLength = 100 n funciona no momento, então vai ser o espaço máximo disponível dentro da PromptBox
            string name = Utils.GetValidString(title, "Nome do amigo: ", minLength: 3);
            string phoneNumber = Utils.PhoneNumberPromptBox(title, "Telefone do amigo: ");
            if (GetAvailable(ignoredFriends).Any(f => f.Name == name && f.PhoneNumber == phoneNumber))
                Utils.MsgBox("Aviso", "Já existe um amigo com esse nome e telefone.", type: MessageType.Warning);
            else return (name, phoneNumber);
        }

    }

}
