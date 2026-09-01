namespace asp_webapi.models;

public class Student
{
    public string StudentNumber { get; set; } = null!;
    public double? Grade { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsMale { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int LeftUnitsCount { get; set; }
}