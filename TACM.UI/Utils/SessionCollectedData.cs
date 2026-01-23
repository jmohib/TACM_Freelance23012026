namespace TACM.UI.Utils;

public struct AnswersCount
{
    public ushort CorrectAnswersCount;
    public ushort WrongAnswersCount;
}


public static class SessionCollectedData
{
    private static int SessionId=0;
    private static int TestResultId=0;
    private static int TestAttempt = 0;
    private static int NonVerbalTestAttempt = 0;
    private static Dictionary<int, AnswersCount> TestResultAnswersCount = [];

    public static void CollectSessionId(int sessionId) => SessionId = sessionId;
    public static ref int GetCollectedSessionId() => ref SessionId;

    public static void CollectTestAttemp(int testAttempt) => TestAttempt = testAttempt;
    public static ref int GetCollectedTestAttempt() => ref TestAttempt;

    public static void CollectNonVerbalTestAttemp(int nonVerbalTestAttempt) => NonVerbalTestAttempt = nonVerbalTestAttempt;
    public static ref int GetCollectedNonVerbalTestAttempt() => ref NonVerbalTestAttempt;

    public static void CollectTestResultId(int testResultId) => TestResultId = testResultId;
    public static ref int GetCollectedTestResultId() => ref TestResultId;

    public static void CollectAnswersCount(ushort correctAnswers, ushort wrongAnswers)
    {
        if (TestResultId == 0 || TestResultId == 0)
            return;

        TestResultAnswersCount ??= [];
        
        if (!TestResultAnswersCount.TryGetValue(TestResultId, out AnswersCount item))
            TestResultAnswersCount.TryAdd(TestResultId, item);

        item.CorrectAnswersCount = correctAnswers;
        item.WrongAnswersCount = wrongAnswers;
    }

    public static void ClearCollectedData()
    {
        SessionId = 0;
        TestResultId = 0;
        TestResultAnswersCount.Clear();
    }
}
