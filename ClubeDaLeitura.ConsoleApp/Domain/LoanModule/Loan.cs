using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.LoanModule;

public class Loan : BaseEntity<Loan>
{

    public Friend Friend { get; internal set; }
    public ComicBook ComicBook { get; internal set; }
    public DateOnly OpenedDate { get; private set; }
    public DateOnly ReturnDate { get; private set; }
    public DateOnly? ReturnedDate { get; private set; }
    private LoanStatus Status;
    public LoanStatus CurrentStatus
    {
        get
        {
            if (Status == LoanStatus.Open && DateOnly.FromDateTime(DateTime.Now) > ReturnDate)
                return LoanStatus.Late;
            return Status;
        }
        set => Status = value;
    }
    public string StatusString => CurrentStatus switch
    {
        LoanStatus.Open => "Aberto",
        LoanStatus.Done => "Concluído",
        LoanStatus.Late => "Atrasado",
        LoanStatus.DoneLate => "Concluído",
        _ => string.Empty
    };
    public string StatusColour => CurrentStatus switch
    {
        LoanStatus.Open => Colours.LightBlue,
        LoanStatus.Done => Colours.LightGreen,
        LoanStatus.Late => Colours.LightRed,
        LoanStatus.DoneLate => Colours.LightYellow,
        _ => Colours.White
    };
    public int StatusOrder => CurrentStatus switch
    {
        LoanStatus.Open => 0,
        LoanStatus.Late => 1,
        LoanStatus.Done => 2,
        LoanStatus.DoneLate => 2,
        _ => 3
    };
    private bool isFinePaid = false;
    public FineStatus FineStatus
    {
        get
        {
            if (isFinePaid)
                return FineStatus.Paid;
            else if (CurrentStatus == LoanStatus.Late || CurrentStatus == LoanStatus.DoneLate)
                return FineStatus.Pending;
            else
                return FineStatus.NoFine;
        }
    }
    public string FineStatusString => FineStatus switch
    {
        FineStatus.NoFine => "Sem Multa",
        FineStatus.Pending => "Pendente",
        FineStatus.Paid => "Quitada",
        _ => string.Empty
    };
    public string FineStatusColour => FineStatus switch
    {
        FineStatus.NoFine => Colours.LightBlue,
        FineStatus.Pending => Colours.LightRed,
        FineStatus.Paid => Colours.LightGreen,
        _ => string.Empty
    };

    public decimal FineValue => CalculateFine();
    public static readonly string[] Categories = ["Amigo", "Revista", "Emprestada", "Data de devolução", "Devolvida", "Status", "Multa"];
    public Loan(Friend friend, ComicBook comicBook)
    {
        Friend = friend;
        ComicBook = comicBook;
        OpenedDate = DateOnly.FromDateTime(DateTime.Now);
        ReturnDate = OpenedDate.AddDays(comicBook.Box.LoanDays);
        CurrentStatus = LoanStatus.Open;
        isFinePaid = false;
    }
    public void ReturnComicBook()
    {
        CurrentStatus = CurrentStatus == LoanStatus.Late ? LoanStatus.DoneLate : LoanStatus.Done;
        ReturnedDate = DateOnly.FromDateTime(DateTime.Now);
        ComicBook.SetAvailable();
    }
    public decimal CalculateFine()
    {
        DateOnly referenceDate = ReturnedDate ?? DateOnly.FromDateTime(DateTime.Now);
        int days = referenceDate.DayNumber - ReturnDate.DayNumber;
        return days > 0 ? days * 2.0m : 0;
    }
    public void PayFine()
    {
        isFinePaid = true;
    }
    public override void UpdateEntity(Loan updatedLoan)
    {
        Friend = updatedLoan.Friend;
        ComicBook = updatedLoan.ComicBook;
        CurrentStatus = updatedLoan.CurrentStatus;
    }
    public override bool Equals(Loan loan)
    {
        if (loan.Friend != Friend
            || loan.ComicBook != ComicBook
            || loan.CurrentStatus != CurrentStatus)
            return false;
        return true;
    }
}