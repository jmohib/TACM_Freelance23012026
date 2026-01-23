namespace TACM.Entities
{
    public class TestResultItem : Entity<int>
    {        
        public int TestResultId { get; set; }
        public TestResult? TestResult { get; set; }

        public int SessionId { get; set; }
        public Session? Session { get; set; }

        public string Item1 { get; set; } = "";
        public string? Item2 { get; set; } = "";
        public char Answer { get; set; }
        public bool IsCorrect { get; set; } = false;
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
    }
}
