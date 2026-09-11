// Exercise 09 — Text tools library (solution)

public static class TextTools
{
    public static string NormalizeName(string name)
    {
        string[] words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join(" ", words);
    }

    public static string Initials(string fullName)
    {
        string[] words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string result = "";
        foreach (string word in words)
        {
            result += char.ToUpper(word[0]);
        }
        return result;
    }

    public static string MaskEmail(string email)
    {
        int at = email.IndexOf('@');
        if (at == -1) return email;
        return email.Substring(0, 1) + "***" + email.Substring(at);
    }

    public static int SafeParseInt(string text, int fallback)
    {
        return int.TryParse(text, out int result) ? result : fallback;
    }

    public static int CountWords(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
