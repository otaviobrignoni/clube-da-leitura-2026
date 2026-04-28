using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Domain.ReservationModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class ReservationUI : BaseUI<Reservation>, IReservationUI
{
    private readonly IComicBookUI ComicBookUI;
    private readonly IFriendUI FriendUI;
    private readonly ILoanRepo LoanRepo;
    public ReservationUI(IComicBookUI comicBookUI, IFriendUI friendUI, ILoanRepo loanRepo, IReservationRepo repository) : base(repository)
    {
        ComicBookUI = comicBookUI;
        FriendUI = friendUI;
        LoanRepo = loanRepo;
    }
    public override void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar reservas", Colours.Title);
        string[] options = ["Reservar revista", "Converter em empréstimo", "Cancelar reserva", "Visualizar reservas", "Voltar"];

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
                    return;
            }
    }
    public override void Add()
    {
        Open();
    }

    public override void Edit()
    {
        Convert();
    }
    public override void Remove()
    {
        Cancel();
    }
    public void Open()
    {
        if (!ComicBookUI.RepoHasAny())
        {
            Utils.MsgBox("Aviso", "Nenhuma revista cadastrada para reservar.", type: MessageType.Warning);
            return;
        }
        if (!FriendUI.RepoHasAny())
        {
            Utils.MsgBox("Aviso", "Nenhum amigo cadastrado para reservar uma revista.", type: MessageType.Warning);
            return;
        }
        var validComicBooks = ComicBookUI.GetAll().Where(cb => cb.IsAvailable).ToList();
        if (validComicBooks.Count < 1)
        {
            Utils.MsgBox("Aviso", "Nenhuma revista está disponível para reserva.", type: MessageType.Warning);
            return;
        }
        var validFriends = FriendUI.GetAll().Where(f => !f.HasPendingFine).ToList();
        if (validFriends.Count < 1)
        {
            Utils.MsgBox("Aviso", "Todos os amigos cadastrados possuem multas em aberto.", type: MessageType.Warning);
            return;
        }
        var ignoredFriends = FriendUI.GetAll().Except(validFriends).ToList();
        var ignoredComicBooks = ComicBookUI.GetAll().Except(validComicBooks).ToList();
        Reservation reservation = new(SelectValidFriend(ignoredFriends), SelectValidComicBook(ignoredComicBooks));
        Repository.Add(reservation);
        Utils.MsgBox("Sucesso", "Reserva criada com sucesso!", type: MessageType.Success);
    }
    public void Convert()
    {
        var activeReservations = Repository.GetAll().Where(r => r.ReservationStatus == ReservationStatus.Active).ToList();
        var ignoredReservations = Repository.GetAll().Except(activeReservations).ToList();

        if (activeReservations.Count == 0)
        {
            Utils.MsgBox("Info", "Não há reservas ativas para converter.", type: MessageType.Info);
            return;
        }

        Reservation reservation = Select("Converter reserva em empréstimo", ignoredReservations);

        if (reservation.Friend.HasOpenLoan)
        {
            Utils.MsgBox("Aviso", "O amigo já possui um empréstimo aberto.", type: MessageType.Warning);
            return;
        }
        if (reservation.Friend.HasPendingFine)
        {
            Utils.MsgBox("Aviso", "O amigo possui multa pendente e não pode reservar revistas.", type: MessageType.Warning);
            return;
        }
        Loan loan = reservation.ConvertToLoan();
        loan.Friend.AddLoan(loan);
        LoanRepo.Add(loan);
        Utils.MsgBox("Sucesso", "Reserva convertida em empréstimo com sucesso!", type: MessageType.Success);
    }

    public void Cancel()
    {
        var activeReservations = Repository.GetAll().Where(r => r.ReservationStatus == ReservationStatus.Active).ToList();
        var ignoredReservations = Repository.GetAll().Except(activeReservations).ToList();

        if (activeReservations.Count == 0)
        {
            Utils.MsgBox("Info", "Não há reservas ativas para cancelar.", type: MessageType.Info);
            return;
        }

        Reservation reservation = Select("Cancelar reserva", ignoredReservations);
        reservation.CancelReservation();
        Utils.MsgBox("Sucesso", "Reserva cancelada com sucesso!", type: MessageType.Success);
    }

    public override Reservation Select(string? title = null, List<Reservation>? ignoredEntities = null)
    {
        title ??= "Selecionar reserva";
        title = Utils.ColourStringHex(title, Colours.Title);

        var availableReservations = GetAvailable(ignoredEntities);
        string[] options = availableReservations.Select(r => $"{r.Friend.Name}, {r.ComicBook.Title} N°{r.ComicBook.Edition}, Status: {Utils.ColourStringHex(r.StatusString, r.StatusColour)}").ToArray();

        return availableReservations[Utils.Menu(title, options)];
    }
    public override void View()
    {
        if (!RepoHasAny())
        {
            Utils.MsgBox("Info", "Não há reservas.", type: MessageType.Info);
            return;
        }
        string title = Utils.ColourStringHex("Reservas", Colours.Title);
        List<string[]> reservations = [];
        var orderedReservations = Repository.GetAll()
                                            .OrderBy(r => r.StatusOrder)
                                            .ThenBy(r => r.ReservedDate);
        foreach (Reservation r in orderedReservations)
            reservations.Add([r.Friend.Name, $"{r.ComicBook.Title} N°{r.ComicBook.Edition}", $"{r.ReservedDate}", Utils.ColourStringHex(r.StatusString, r.StatusColour)]);

        Utils.GenerateTable(title, Reservation.Categories, reservations.ToArray());
    }
    public Friend SelectValidFriend(List<Friend> ignoredFriends)
    {
        return FriendUI.Select("Selecionar amigo fazendo a reserva", ignoredFriends);
    }
    public ComicBook SelectValidComicBook(List<ComicBook> ignoredComicBooks)
    {
        return ComicBookUI.Select("Selecionar revista sendo reservada", ignoredComicBooks);
    }
}
