using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Domain.BoxModule;

public class Box : BaseEntity<Box>
{
    public string Tag = string.Empty;
    public string Colour = string.Empty;
    public int LoanDays = 7;
    public Box(string tag, string colour, int loanDays)
    {
        Tag = tag;
        Colour = colour;
        LoanDays = loanDays;
    }
    public Box(Box box) : this(box.Tag, box.Colour, box.LoanDays) { }
    public override bool Equals(Box entity)
    {
        if (entity.Tag != Tag
            || entity.Colour != Colour
            || entity.LoanDays != LoanDays)
            return false;
        return true;
    }

    public override void UpdateEntity(Box updatedEntity)
    {
        Tag = updatedEntity.Tag;
        Colour = updatedEntity.Colour;
        LoanDays = updatedEntity.LoanDays;
    }
}
