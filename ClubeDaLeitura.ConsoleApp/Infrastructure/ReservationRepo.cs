using ClubeDaLeitura.ConsoleApp.Domain.ReservationModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Infrastructure;

public class ReservationRepo : BaseRepository<Reservation>, IReservationRepo { }
