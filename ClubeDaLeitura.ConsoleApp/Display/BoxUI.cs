using ClubeDaLeitura.ConsoleApp.Domain.BoxModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class BoxUI : BaseUI<Box>
{
    public BoxUI(IBoxRepo boxRepo) : base(boxRepo) { }

    public override void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar caixas", Colours.Title);
        string[] options = ["Cadastrar caixa", "Editar caixa", "Remover caixa", "Visualizar caixas", "Voltar"];
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
        string title = Utils.ColourStringHex("Cadastrar caixa", Colours.Title);
        Repository.Add(new Box(
            GetValidTag(title),
            Utils.GetValidString(title, "Cor da caixa: ", pattern: @"^#?[0-9A-Fa-f]{6}$"),
            Utils.GetValidInteger(title, "Dias de empréstimo (padrão = 7): ")));
        Utils.MsgBox("Sucesso", "Caixa cadastrada com sucesso!", type: MessageType.Success);
    }
    public override void Edit()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhuma caixa cadastrada para editar.", type: MessageType.Warning);
            return;
        }
        string title = Utils.ColourStringHex("Editar caixa", Colours.Title);
        string[] options = ["Etiqueta", "Cor", "Dias de empréstimo", "Voltar"];

        Box box = Select("Selecionar caixa para editar");
        Box editedBox = new(box);

        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    editedBox.Tag = GetValidTag(title, [box]);
                    break;
                case 1:
                    editedBox.Colour = Utils.GetValidString(title, "Cor da caixa: ", pattern: @"^#?[0-9A-Fa-f]{6}$");
                    break;
                case 2:
                    editedBox.LoanDays = Utils.GetValidInteger(title, "Dias de empréstimo (padrão = 7): ");
                    break;
                case 3:
                    if (!editedBox.Equals(box))
                    {
                        Utils.MsgBox("Sucesso", "Caixa editada com sucesso!", type: MessageType.Success);
                        box.UpdateEntity(editedBox);
                    }
                    return;
            }
    }
    public override void Remove()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhuma caixa cadastrada para remover.", type: MessageType.Warning);
            return;
        }
        Box box = Select("Selecionar caixa para remover");
        if (box.HasComicBook)
        {
            Utils.MsgBox("Aviso", "Não é possível remover esta caixa porque contém revistas. Remova as revistas primeiro.", type: MessageType.Warning);
            return;
        }
        if (Repository.Remove(box.Id)) Utils.MsgBox("Sucesso", "Caixa removida com sucesso!", type: MessageType.Success);
        else Utils.MsgBox("Erro", "Erro ao remover o caixa. Tente novamente.", type: MessageType.Error);
    }

    public override Box Select(string? title = null, List<Box>? entities = null)
    {
        title ??= "Selecionar caixa";
        title = Utils.ColourStringHex(title, Colours.Title);
        var availableBoxes = GetAvailable(entities);
        string[] options = availableBoxes.Select(b => $"{Utils.ColourStringHex(b.Tag, b.Colour)} ID: {b.Id}").ToArray();
        return availableBoxes[Utils.Menu(title, options)];
    }

    public override void View()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Info", "Nenhuma caixa cadastrada.", type: MessageType.Info);
            return;
        }
        string title = Utils.ColourStringHex("Caixas", Colours.Title);
        List<string[]> boxes = [];
        foreach (Box b in Repository.GetAll())
            boxes.Add([$"{Utils.ColourStringHex(b.Tag, b.Colour)}", $"{b.LoanDays}"]);
        Utils.GenerateTable(title, Box.Categories, boxes.ToArray());
    }
    public string GetValidTag(string title, List<Box>? ignoredBoxes = null)
    {
        while (true)
        {
            string tag = Utils.GetValidString(title, "Etiqueta da caixa: ", maxLength: 50);
            if (GetAvailable(ignoredBoxes).Any(b => b.Tag == tag))
                Utils.MsgBox("Aviso", "Já existe uma caixa com essa etiqueta.", type: MessageType.Warning);
            else return tag;
        }
    }
}
