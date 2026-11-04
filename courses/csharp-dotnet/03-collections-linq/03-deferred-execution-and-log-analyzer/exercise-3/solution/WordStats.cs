public static class WordStats
{
    private static IEnumerable<string> Words(string text) =>
        text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w.ToLowerInvariant());

    public static List<(string Word, int Count)> TopWords(string text, int n)
    {
        return Words(text).GroupBy(w => w).Select(g => (g.Key, g.Count()))
            .OrderByDescending(x => x.Item2).ThenBy(x => x.Key).Take(n).ToList();
    }

    public static int UniqueWords(string text)
    {
        return Words(text).Distinct().Count();
    }
}
