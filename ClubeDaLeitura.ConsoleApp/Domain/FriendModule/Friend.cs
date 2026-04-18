using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.FriendModule;

public class Friend : BaseEntity<Friend>
{
    public string Name = string.Empty;
    public string ParentName = string.Empty;
    public string PhoneNumber = string.Empty;
    public static readonly string[] Categories = ["Nome", "Responsável", "Telefone"];
    public Friend(string name, string parentName, string phoneNumber)
    {
        Name = name;
        ParentName = parentName;
        PhoneNumber = phoneNumber;
    }
    public Friend(Friend friend) : this(friend.Name, friend.ParentName, friend.PhoneNumber) { }

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
