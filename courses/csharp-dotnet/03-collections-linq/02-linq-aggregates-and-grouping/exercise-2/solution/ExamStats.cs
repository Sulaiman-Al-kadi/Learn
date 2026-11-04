public record Exam(string Student, string Subject, int Score);

public static class ExamStats
{
    public static Dictionary<string, double> AverageBySubject(List<Exam> exams)
    {
        return exams.GroupBy(e => e.Subject).ToDictionary(g => g.Key, g => g.Average(e => e.Score));
    }

    public static string? TopScorer(List<Exam> exams)
    {
        return exams.OrderByDescending(e => e.Score).FirstOrDefault()?.Student;
    }

    public static bool EveryonePassed(List<Exam> exams, int passMark)
    {
        return exams.All(e => e.Score >= passMark);
    }

    public static List<string> NamesPage(List<Exam> exams, int page, int size)
    {
        return exams.Select(e => e.Student).OrderBy(n => n).Skip((page - 1) * size).Take(size).ToList();
    }
}
