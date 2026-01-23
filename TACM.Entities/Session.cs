namespace TACM.Entities;

public class Session : Entity<int>
{
    public required string SubjectID { get; set; }
    public required ushort Age {  get; set; }
    public required string Sex {  get; set; }
}
