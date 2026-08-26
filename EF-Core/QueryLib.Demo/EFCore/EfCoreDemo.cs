using Microsoft.EntityFrameworkCore;

namespace QueryLib.Demo.EFCore;

public static class EfCoreDemo
{
    public static async Task RunAsync()
    {
        await using var db = new AppDbContext();

        var students = await db.Students
            .Where(s => s.IsMale)
            .Where(s => new[]
            {
                "98100203",
                "98100207",
                "98100205"
            }.Contains(s.StudentNumber))
            .OrderBy(s => s.LastName)
            .Take(10)
            .Select(s => new
            {
                s.StudentNumber,
                s.FirstName,
                s.LastName
            })
            .ToListAsync();

        foreach (var student in students)
        {
            Console.WriteLine(
                $"{student.StudentNumber} | {student.FirstName} | {student.LastName}");
        }
    }
}