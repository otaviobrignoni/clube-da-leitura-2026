using ClubeDaLeitura.ConsoleApp.Domain.LoanModule;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.FriendModule;

public class Friend : BaseEntity<Friend>
{
    public string Name { get; internal set; } = string.Empty;
    public string ParentName { get; internal set; } = string.Empty;
    public string PhoneNumber { get; internal set; } = string.Empty;
    public HashSet<Loan> Loans { get; } = [];
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
    public override void UpdateEntity(Friend updatedFriend)
    {
        Name = updatedFriend.Name;
        ParentName = updatedFriend.ParentName;
        PhoneNumber = updatedFriend.PhoneNumber;
    }

    public override bool Equals(Friend friend)
    {
        if (friend.Name != Name
            || friend.ParentName != ParentName
            || friend.PhoneNumber != PhoneNumber)
            return false;
        return true;
    }
}
