using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Infrastructure;

public class LoanRepo : BaseRepository<Loan>, ILoanRepo { }
