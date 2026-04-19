using ClubeDaLeitura.ConsoleApp.Domain.ComicBookModule;
using ClubeDaLeitura.ConsoleApp.Shared;
using ClubeDaLeitura.ConsoleApp.Shared.Base;

namespace ClubeDaLeitura.ConsoleApp.Display;

public class ComicBookUI : BaseUI<ComicBook>
{
    BoxUI BoxUI;
    public ComicBookUI(BoxUI boxUI, IComicBookRepo comicBookRepo) : base(comicBookRepo)
    {
        BoxUI = boxUI;
    }
    public override void Menu()
    {
        string title = Utils.ColourStringHex("Gerenciar revistas", Colours.Title);
        string[] options = ["Cadastrar revista", "Editar revista", "Remover revista", "Visualizar revistas", "Voltar"];
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
        if (!BoxUI.RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Para adicionar uma revista, primeiro cadastre uma caixa.", type: MessageType.Warning);
            return;
        }
        string title = Utils.ColourStringHex("Cadastrar revista", Colours.Title);
        var (comicBookTitle, edition) = GetValidTitleAndEdition(title);
        ComicBook comicBook = new(comicBookTitle, edition, Utils.DatePromptBox(title, "Data de publicação: "), BoxUI.Select("Selecionar caixa da revista"));
        comicBook.Box.AddComicBook(comicBook);
        Repository.Add(comicBook);
        Utils.MsgBox("Sucesso", "Revista cadastrada com sucesso!", type: MessageType.Success);
    }

    public override void Edit()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhuma revista cadastrada para editar.", type: MessageType.Warning);
            return;
        }
        string title = Utils.ColourStringHex("Editar revista", Colours.Title);
        string[] options = ["Título e Edição", "Data de publicação", "Caixa", "Voltar"];

        ComicBook comicBook = Select("Selecionar revista para editar");
        ComicBook editedComicBook = new(comicBook);

        while (true)
            switch (Utils.Menu(title, options))
            {
                case 0:
                    var (editedTitle, editedEdition) = GetValidTitleAndEdition(title, [comicBook]);
                    editedComicBook.Title = editedTitle;
                    editedComicBook.Edition = editedEdition;
                    break;
                case 1:
                    editedComicBook.ReleaseDate = Utils.DatePromptBox(title, "Data de publicação: ");
                    break;
                case 2:
                    editedComicBook.Box = BoxUI.Select("Selecionar caixa da revista");
                    break;
                case 3:
                    if (!editedComicBook.Equals(comicBook))
                    {
                        Utils.MsgBox("Sucesso", "Revista editada com sucesso!", type: MessageType.Success);
                        comicBook.UpdateEntity(editedComicBook);
                    }
                    return;
            }
    }

    public override void Remove()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Aviso", "Nenhuma revista cadastrada para remover.", type: MessageType.Warning);
            return;
        }
        ComicBook comicBook = Select("Selecionar revista para remover");
        comicBook.Box.RemoveComicBook(comicBook.Id);
        if (Repository.Remove(comicBook.Id)) Utils.MsgBox("Sucesso", "Revista removida com sucesso!", type: MessageType.Success);
        else Utils.MsgBox("Erro", "Erro ao remover a revista. Tente novamente.", type: MessageType.Error);
    }

    public override ComicBook Select(string? title = null, List<ComicBook>? entities = null)
    {
        title ??= "Selecionar revista";
        title = Utils.ColourStringHex(title, Colours.Title);
        var availableComicBooks = GetAvailable(entities);
        string[] options = availableComicBooks.Select(cb => $"{Utils.ColourStringHex(cb.Title, cb.Box.Colour)}, ID: {cb.Id}").ToArray();
        return availableComicBooks[Utils.Menu(title, options)];
    }

    public override void View()
    {
        if (!RepoHasAny)
        {
            Utils.MsgBox("Info", "Nenhuma revista cadastrada.", type: MessageType.Info);
            return;
        }
        string title = Utils.ColourStringHex("Revistas", Colours.Title);
        List<string[]> comicBooks = [];
        foreach (ComicBook cb in Repository.GetAll())
            comicBooks.Add([cb.Title, $"{cb.Edition}", $"{cb.ReleaseDate:dd/MM/yyyy}", $"{Utils.ColourStringHex(cb.Box.Tag, cb.Box.Colour)}"]);
        Utils.GenerateTable(title, ComicBook.Categories, comicBooks.ToArray());
    }

    public (string, int) GetValidTitleAndEdition(string title, List<ComicBook>? ignoredComicBooks = null)
    {
        while (true)
        { 
            // maxLength = 100 n funciona no momento, então vai ser o espaço máximo disponível dentro da PromptBox
            string comicBookTitle = Utils.GetValidString(title, "Título da revista: ", minLength: 2);
            int edition = Utils.GetValidInteger(title, "Edição da revista: ");
            if (GetAvailable(ignoredComicBooks).Any(cb => cb.Title == comicBookTitle && cb.Edition == edition))
                Utils.MsgBox("Aviso", "Já existe uma revista com esse título e edição", type: MessageType.Warning);
            else return (comicBookTitle, edition);
        }
    }
}
