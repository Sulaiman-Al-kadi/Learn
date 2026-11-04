public static class TextCleanup
{
    public static List<string> CleanLines(List<string> lines)
    {
        return lines.Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
    }

    public static List<int> WordLengths(string sentence)
    {
        return sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w.Length).ToList();
    }

    public static List<string> LongestFirst(List<string> words)
    {
        return words.OrderByDescending(w => w.Length).ThenBy(w => w).ToList();
    }
}
