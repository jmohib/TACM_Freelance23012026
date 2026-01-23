namespace TACM.Entities
{
    public class TestResult : Entity<int>
    {
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public string TestType { get; set; } = "";
        public ushort ItemsCount {  get; set; }
        public bool Preview {  get; set; }
        public DateTime? End {  get; set; }

        public ICollection<TestResultItem>? Items { get; set; }
    }
}
