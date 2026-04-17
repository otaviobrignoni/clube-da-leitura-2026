namespace ClubeDaLeitura.ConsoleApp.Shared;

public static class StringExtensions
{
    public static string FitToWidth(this string text, int width)
    {
        if (text.Length > width) return text.Truncate(width);
        return text.PadRight(width);
    }
    public static string Center(this string text, int width)
    {
        if (text.Length > width) return text.Truncate(width);
        return text.PadLeft((width + text.Length) / 2).PadRight(width);
    }
    public static string Truncate(this string text, int width)
    {
        if (width <= 0) return string.Empty;
        if (width == 1) return "…";
        return string.Concat(text.AsSpan(0, width - 1), "…");
    }
}
