public record Exam(string Student, string Subject, int Score);

public static class ExamStats
{
    public static Dictionary<string, double> AverageBySubject(List<Exam> exams)
    {
        throw new NotImplementedException();
    }

    public static string? TopScorer(List<Exam> exams)
    {
        throw new NotImplementedException();
    }

    public static bool EveryonePassed(List<Exam> exams, int passMark)
    {
        throw new NotImplementedException();
    }

    public static List<string> NamesPage(List<Exam> exams, int page, int size)
    {
        throw new NotImplementedException();
    }
}
