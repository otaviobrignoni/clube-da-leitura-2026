using System.Text;
using System.Text.RegularExpressions;

namespace ClubeDaLeitura.ConsoleApp.Shared;

public static class StringExtensions
{
    private static readonly Regex AnsiRegex = new(@"\x1B\[[0-9;]*m", RegexOptions.Compiled);

    public static string FitToWidth(this string text, int width)
    {
        int visibleLength = text.VisibleLength();

        if (visibleLength > width)
            return text.Truncate(width);

        return text.PadRight(text.Length + (width - visibleLength));
    }

    public static string Center(this string text, int width)
    {
        int visibleLength = text.VisibleLength();

        if (visibleLength > width)
            return text.Truncate(width);

        int totalPadding = width - visibleLength;
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;

        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }

    public static string Truncate(this string text, int width)
    {
        if (width <= 0) return string.Empty;
        if (width == 1) return "…";

        StringBuilder result = new();
        int visibleCount = 0;
        bool hadAnsi = false;

        for (int i = 0; i < text.Length;)
        {
            if (text[i] == '\x1B')
            {
                Match match = AnsiRegex.Match(text, i);

                if (match.Success && match.Index == i)
                {
                    result.Append(match.Value);
                    i += match.Length;
                    hadAnsi = true;
                    continue;
                }
            }

            if (visibleCount >= width - 1) break;

            result.Append(text[i]);
            visibleCount++;
            i++;
        }

        result.Append('…');

        if (hadAnsi)
            result.Append("\x1b[0m");

        return result.ToString();
    }

    public static string StripAnsi(this string text)
    {
        return AnsiRegex.Replace(text, "");
    }

    public static int VisibleLength(this string text)
    {
        return text.StripAnsi().Length;
    }
}
