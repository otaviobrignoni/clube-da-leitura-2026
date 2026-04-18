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
        Utils.MsgBox(Utils.ColourStringHex("Info", Colours.Info), Utils.ColourStringHex("✓", Colours.Success) + " Caixa cadastrada com sucesso!");
    }
    public override void Edit()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox(Utils.ColourStringHex("Aviso", Colours.Warning), "Nenhuma caixa cadastrada para editar.");
            return;
        }
        string title = Utils.ColourStringHex("Editar caixa", Colours.Title);
        string[] options = ["Etiqueta", "Cor", "Dias de empréstimo", "Voltar"];

        Box box = Select();
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
                        Utils.MsgBox(
                            Utils.ColourStringHex("Info", Colours.Info),
                            $"{Utils.ColourStringHex("✓", Colours.Success)} Caixa editada com sucesso!");
                        box.UpdateEntity(editedBox);
                    }
                    return;
            }
    }
    public override void Remove()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox(Utils.ColourStringHex("Aviso", Colours.Warning), "Nenhuma caixa cadastrada para remover.");
            return;
        }
        Box box = Select();
        if (Repository.Remove(box.Id))
            Utils.MsgBox(
                Utils.ColourStringHex("Info", Colours.Info),
                $"{Utils.ColourStringHex("✓", Colours.Success)} Caixa removida com sucesso!");
        else Utils.MsgBox(
            Utils.ColourStringHex("Erro", Colours.Error),
            $"{Utils.ColourStringHex("✗", Colours.Error)}) Erro ao remover o caixa. Tente novamente.");
    }

    public override Box Select(List<Box>? entities = null)
    {
        string title = Utils.ColourStringHex("Selecionar caixa", Colours.Title);
        var availableBoxes = GetAvailable(entities);
        string[] options = availableBoxes.Select(b => $"{Utils.ColourStringHex(b.Tag, b.Colour)} ID: {b.Id}").ToArray();
        return availableBoxes[Utils.Menu(title, options)];
    }

    public override void View()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox(Utils.ColourStringHex("Info", Colours.Info), "Nenhuma caixa cadastrada.");
            return;
        }
        string title = Utils.ColourStringHex("Caixas", Colours.Title);
        string[] categories = ["Etiqueta", "Dias de empréstimo"];
        List<string[]> boxes = [];
        foreach (Box b in Repository.GetAll())
            boxes.Add([$"{Utils.ColourStringHex(b.Tag, b.Colour)}", $"{b.LoanDays}"]);
        Utils.GenerateTable(title, categories, boxes.ToArray());
    }
    public string GetValidTag(string title, List<Box>? ignoredBoxes = null)
    {
        while (true)
        {
            string tag = Utils.GetValidString(title, "Etiqueta da caixa: ", maxLength: 50);
            if (GetAvailable(ignoredBoxes).Any(b => b.Tag == tag))
                Utils.MsgBox(Utils.ColourStringHex("Aviso", Colours.Warning), "Já existe uma caixa com essa etiqueta.");
            else return tag;
        }
    }
}
