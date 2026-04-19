using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.FriendModule;

public class Friend : BaseEntity<Friend>
{
    public string Name = string.Empty;
    public string ParentName = string.Empty;
    public string PhoneNumber = string.Empty;
    public HashSet<Loan> Loans = [];
    public bool HasOpenLoan => Loans.Any(l => l.CurrentStatus == LoanStatus.Open || l.CurrentStatus == LoanStatus.Late);
    public bool HasLoan => Loans.Count > 0;
    public static readonly string[] Categories = ["Nome", "Responsável", "Telefone"];
    public Friend(string name, string parentName, string phoneNumber)
    {
        Name = name;
        ParentName = parentName;
        PhoneNumber = phoneNumber;
    }
    public Friend(Friend friend) : this(friend.Name, friend.ParentName, friend.PhoneNumber) { }

    public bool AddLoan(Loan loan)
    {
        if (HasOpenLoan) return false;
        Loans.Add(loan);
        return true;
    }
    public override void UpdateEntity(Friend updatedEntity)
    {
        Name = updatedEntity.Name;
        ParentName = updatedEntity.ParentName;
        PhoneNumber = updatedEntity.PhoneNumber;
    }

    public override bool Equals(Friend entity)
    {
        if (entity.Name != Name
            || entity.ParentName != ParentName
            || entity.PhoneNumber != PhoneNumber)
            return false;
        return true;
    }
}
