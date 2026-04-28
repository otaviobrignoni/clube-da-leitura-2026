using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Domain.FriendModule;
using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.ReservationModule;

public class Reservation : BaseEntity<Reservation>
{
    public Friend Friend { get; internal set; }
    public ComicBook ComicBook { get; internal set; }
    public DateOnly ReservedDate { get; private set; }
    public ReservationStatus ReservationStatus;
    public string StatusString => ReservationStatus switch
    {
        ReservationStatus.Active => "Ativa",
        ReservationStatus.Done => "Concluída",
        ReservationStatus.Canceled => "Cancelada",
        _ => string.Empty
    };
    public string StatusColour => ReservationStatus switch
    {
        ReservationStatus.Active => Colours.LightBlue,
        ReservationStatus.Done => Colours.LightGreen,
        ReservationStatus.Canceled => Colours.LightYellow,
        _ => Colours.White
    };
    public int StatusOrder => ReservationStatus switch
    {
        ReservationStatus.Active => 0,
        ReservationStatus.Done => 1,
        ReservationStatus.Canceled => 2,
        _ => 3
    };
    public static readonly string[] Categories = ["Amigo", "Revista", "Reservada", "Status"];
    public Reservation(Friend friend, ComicBook comicBook)
    {
        Friend = friend;
        ComicBook = comicBook;
        ReservedDate = DateOnly.FromDateTime(DateTime.Now);
        ComicBook.CurrentStatus = ComicBookStatus.Reserved;
        ReservationStatus = ReservationStatus.Active;
    }
    public void CancelReservation()
    {
        ComicBook.SetAvailable();
        ReservationStatus = ReservationStatus.Canceled;
    }
    public Loan ConvertToLoan()
    {
        ReservationStatus = ReservationStatus.Done;
        ComicBook.SetLoaned();
        return new Loan(Friend, ComicBook);
    }
    public override bool Equals(Reservation reservation)
    {
        if (reservation.Friend != Friend
            || reservation.ComicBook != ComicBook
            || reservation.ReservationStatus != ReservationStatus)
            return false;
        return true;
    }

    public override void UpdateEntity(Reservation updatedReservation)
    {
        Friend = updatedReservation.Friend;
        ComicBook = updatedReservation.ComicBook;
        ReservationStatus = updatedReservation.ReservationStatus;
    }
}
