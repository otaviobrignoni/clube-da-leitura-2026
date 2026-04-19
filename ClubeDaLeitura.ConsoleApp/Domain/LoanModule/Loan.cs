using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.LoanModule;

public class Loan : BaseEntity<Loan>
{
    public Friend Friend;
    public ComicBook ComicBook;
    public DateTime OpenedDate;
    public DateTime ReturnDate;
    public DateTime? ReturnedDate = null;
    public LoanStatus CurrentStatus;
    public bool IsLate => DateTime.Now > ReturnDate && CurrentStatus != LoanStatus.Done && CurrentStatus != LoanStatus.DoneLate;
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
        LoanStatus.Open => "#8ea9f1",
        LoanStatus.Done => "#7eee91",
        LoanStatus.Late => "#ff7272",
        LoanStatus.DoneLate => "#ffcd82",
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
    public static readonly string[] Categories = ["Amigo", "Revista", "Emprestada", "Data de devolução", "Devolvida", "Status"];
    public Loan(Friend friend, ComicBook comicBook)
    {
        Friend = friend;
        ComicBook = comicBook;
        OpenedDate = DateTime.Now;
        ReturnDate = DateTime.Now.AddDays(comicBook.Box.LoanDays);
        CurrentStatus = LoanStatus.Open;
    }
    public void ReturnComicBook()
    {
        if (!IsLate) CurrentStatus = LoanStatus.Done;
        else CurrentStatus = LoanStatus.DoneLate;
        ReturnedDate = DateTime.Now;
        ComicBook.ChangeStatus();
    }

    public override bool Equals(Loan entity)
    {
        if (entity.Friend != Friend
            || entity.ComicBook != ComicBook
            || entity.CurrentStatus != CurrentStatus)
            return false;
        return true;
    }
    public override void UpdateEntity(Loan updatedEntity)
    {
        Friend = updatedEntity.Friend;
        ComicBook = updatedEntity.ComicBook;
        CurrentStatus = updatedEntity.CurrentStatus;
    }
}
