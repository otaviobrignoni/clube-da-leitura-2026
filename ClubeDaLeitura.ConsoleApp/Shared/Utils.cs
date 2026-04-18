using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubeDaLeitura.ConsoleApp.Shared;

public static partial class Utils
{
    public const int MenuWidth = 80;
    public const string PhoneNumberTemplate = "(__) _____-____";
    public const string DateTemplate = "__/__/____";
    public const string DateFormat = "dd/MM/yyyy";
    public const int BoxH = 2;
    public const int BoxV = 1;
    public const int UnderBox = 3;
    public static int Menu(string title, string[] options)
    {
        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            DrawBoxTop(title);
            for (int i = 0; i < options.Length; i++)
                DrawBoxMiddle(i == selectedIndex ? $"{ColourStringHex("❯", Colours.Info)} {options[i]}" : $"  {options[i]}");
            DrawBoxBottom();
            Console.WriteLine("⇅ selecionar | ↲ Confirmar");
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;

                case ConsoleKey.Enter:
                    return selectedIndex;
            }
        }
    }
    public static string PromptBox(string title, string msg)
    {
        MsgBox(title, msg, false);

        string userInput = string.Empty;
        int x = msg.VisibleLength() + BoxH;
        int y = BoxV;
        int maxLength = MenuWidth - msg.VisibleLength() - BoxH;
        Console.SetCursorPosition(x, y);

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (!char.IsControl(keyInfo.KeyChar) && userInput.Length < maxLength)
            {
                userInput += keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
                x += 1;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && userInput.Length > 0)
            {
                userInput = userInput[..^1];
                x -= 1;
                Console.SetCursorPosition(x, y);
                Console.Write(' ');
                Console.SetCursorPosition(x, y);
            }
            else if (keyInfo.Key == ConsoleKey.Enter && userInput.Length > 0)
            {
                Console.SetCursorPosition(0, 0);
                return userInput;
            }
        }
    }
    public static DateOnly DatePromptBox(string title, string msg)
    {
        MsgBox(title, msg + DateTemplate, false);

        const int dateLength = 8;
        char[] userInput = new char[dateLength];
        int[] slashPos = [2, 4];
        int index = 0;
        int x = msg.VisibleLength() + BoxH;
        int y = BoxV;
        Console.SetCursorPosition(x, y);

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (char.IsDigit(keyInfo.KeyChar) && index < dateLength)
            {
                if (slashPos.Contains(index))
                {
                    x += 1;
                    Console.SetCursorPosition(x, y);
                }
                userInput[index++] = keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
                x += 1;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && index > 0)
            {
                index--;
                x -= 1;
                Console.SetCursorPosition(x, y);
                Console.Write('_');
                if (slashPos.Contains(index)) x -= 1;
                Console.SetCursorPosition(x, y);

            }
            else if (keyInfo.Key == ConsoleKey.Enter && index == dateLength)
            {
                string dateString = new(userInput);
                string formattedDate = $"{dateString[..2]}/{dateString[2..4]}/{dateString[4..8]}";
                if (DateOnly.TryParseExact(formattedDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
                {
                    Console.SetCursorPosition(0, 0);
                    return date;
                }
                else
                {
                    MsgBox(ColourStringHex("Aviso", Colours.Warning), "Data inválida. Tente novamente no formato DD/MM/YYYY.");
                    index = 0;
                    Console.SetCursorPosition(0, 0);
                    MsgBox(title, msg + DateTemplate, false);
                    x = msg.VisibleLength() + BoxH;
                    Console.SetCursorPosition(x, y);
                }
            }
        }
    }
    public static string PhoneNumberPromptBox(string title, string msg)
    {
        MsgBox(title, msg + PhoneNumberTemplate, false);
        const int phoneNumberLength = 11;
        char[] userInput = new char[phoneNumberLength];
        int[] templatePos = [0, 2, 7];
        int index = 0;
        int x = msg.Length + BoxH;
        int y = BoxV;
        Console.SetCursorPosition(x, y);
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (char.IsDigit(keyInfo.KeyChar) && index < phoneNumberLength)
            {
                if (templatePos.Contains(index))
                {
                    x += index == templatePos[1] ? 2 : 1;
                    Console.SetCursorPosition(x, y);
                }
                userInput[index++] = keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
                x += 1;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && index > 0)
            {
                index--;
                x -= 1;
                Console.SetCursorPosition(x, y);
                Console.Write('_');
                if (templatePos.Contains(index)) x -= index == templatePos[1] ? 2 : 1;
                Console.SetCursorPosition(x, y);
            }
            else if (keyInfo.Key == ConsoleKey.Enter && index == phoneNumberLength)
            {
                string phoneString = new(userInput);
                Console.SetCursorPosition(0, 0);
                return $"({phoneString[..2]}) {phoneString[2..7]}-{phoneString[7..11]}";
            }
        }

    }
    public static void MsgBox(string title, string msg, bool askPrompt = true)
    {
        string[] msgLines = msg.Split('\0');
        Console.Clear();
        DrawBoxTop(title);
        foreach (string s in msgLines)
            Console.WriteLine("│ " + s.FitToWidth(MenuWidth - 1) + "│");
        DrawBoxBottom();
        if (askPrompt) EnterPrompt();
    }
    public static void DrawBoxTop(string title)
    {
        title = "─ " + title + ' ';
        Console.WriteLine("╭" + title + new string('─', MenuWidth - title.VisibleLength()) + "╮ ");
    }
    public static void DrawBoxMiddle(string text)
    {
        text = ' ' + text;
        Console.WriteLine("│" + text.FitToWidth(MenuWidth) + "│");
    }
    public static void DrawBoxBottom()
    {
        Console.WriteLine("╰" + new string('─', MenuWidth) + "╯");
    }
    public static void GenerateTable(string title, string[] headers, string[][] rows)
    {
        int[] columnWidths = headers.Select(h => h.VisibleLength()).ToArray();

        for (int i = 0; i < headers.Length; i++)
        {
            foreach (var row in rows)
            {
                if (row != null && i < row.Length)
                    columnWidths[i] = Math.Max(columnWidths[i], row[i]?.VisibleLength() ?? 0);
            }
        }

        int contentWidth = columnWidths.Sum() + (headers.Length * 3) + 1;
        int titleWidth = Math.Max(contentWidth, title.VisibleLength() + 3);

        Console.Clear();

        Console.WriteLine("╭─ " + title + " " + new string('─', titleWidth - title.VisibleLength() - 5) + "╮");

        Console.Write("│");
        for (int i = 0; i < headers.Length; i++)
            Console.Write(" " + headers[i].Center(columnWidths[i]) + " │");
        Console.WriteLine();

        Console.WriteLine("├" + string.Join("┼", columnWidths.Select(w => new string('─', w + 2))) + "┤");

        foreach (var row in rows)
        {
            Console.Write("│");
            for (int i = 0; i < headers.Length; i++)
            {
                string value = i < row.Length ? row[i] : "";
                Console.Write(" " + value.FitToWidth(columnWidths[i]) + " │");
            }
            Console.WriteLine();
        }

        Console.WriteLine("╰" + string.Join("┴", columnWidths.Select(w => new string('─', w + 2))) + "╯");
        EnterPrompt();
    }
    public static void EnterPrompt(string? msg = null)
    {
        Console.WriteLine(msg ?? "Pressione ENTER para continuar…");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;
    }
    public static string GetValidString(string title, string msg, int minLength = 3, int? maxLength = null, string pattern = "^.*$", string invalidFormatMsg = "Formato inválido. Tente novamente.")
    {
        int availableSpace = MenuWidth - msg.VisibleLength() - BoxH;
        maxLength ??= availableSpace;
        if (minLength < 0)
            throw new ArgumentOutOfRangeException(nameof(minLength), "minLength cannot be negative");
        if (minLength > maxLength)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength cannot be less than minLength");
        if (maxLength > availableSpace)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength cannot be larger than availableSpace");

        while (true)
        {
            string input = PromptBox(title, msg).Trim();
            string wordMin = minLength == 1 ? "letra" : "letras";
            string wordMax = maxLength == 1 ? "letra" : "letras";

            if (input.Length == 0 && minLength > 0)
                MsgBox(ColourStringHex("Aviso", Colours.Warning), "Entrada inválida. Insira algum texto.");
            else if (input.Length < minLength)
                MsgBox(ColourStringHex("Aviso", Colours.Warning), $"Este campo deve conter no mínimo {minLength} {wordMin}.");
            else if (input.Length > maxLength)
                MsgBox(ColourStringHex("Aviso", Colours.Warning), $"Este campo deve conter no máximo {maxLength} {wordMax}.");
            else if (!Regex.IsMatch(input, pattern))
                MsgBox(ColourStringHex("Aviso", Colours.Warning), invalidFormatMsg);
            else
                return input;
        }
    }
    public static int GetValidInteger(string title, string msg)
    {
        while (true)
        {
            if (int.TryParse(PromptBox(title, msg), out int value) && value > 0)
                return value;

            MsgBox(ColourStringHex("Aviso", Colours.Warning), value <= 0 ? "O valor deve ser maior que zero. Tente novamente." : "Entrada inválida. Insira um valor numérico válido.");
        }
    }

    public static string ColourStringHex(string text, string hex)
    {
        hex = hex.TrimStart('#');

        int r = Convert.ToInt32(hex[..2], 16);
        int g = Convert.ToInt32(hex[2..4], 16);
        int b = Convert.ToInt32(hex[4..6], 16);

        return $"\x1b[38;2;{r};{g};{b}m{text}\x1b[0m";
    }
}
