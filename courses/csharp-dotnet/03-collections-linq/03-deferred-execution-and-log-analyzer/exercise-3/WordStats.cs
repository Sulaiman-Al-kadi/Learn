public static class WordStats
{
    private static IEnumerable<string> Words(string text) =>
        text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w.ToLowerInvariant());

    public static List<(string Word, int Count)> TopWords(string text, int n)
    {
        throw new NotImplementedException();
    }

    public static int UniqueWords(string text)
    {
        throw new NotImplementedException();
    }
}
