namespace TACM.Data.Queries
{
    public  class GetSessionsQuery
    {
        public int? SessionId { get; set; }
        public DateTime? SessionDate { get; set; }
        public string? SubjectID { get; set; }
        public ushort Age { get; set; }
        public string? Sex { get; set; }
    }
}
