using SqlKata.Compilers;

namespace QueryLib.Demo;

public static class SqlKataDemo2
{
    public static void Run()
    {
        var query = new SqlKata.Query("Student")
            .Select(
                "StudentNumber",
                "FirstName",
                "LastName")
            .Where("IsMale", true)
            .WhereIn("StudentNumber", new[]
            {
                "98100203",
                "98100207",
                "98100205"
            })
            .OrderBy("LastName")
            .Limit(10);
        
        // var query = new SqlKata.Query("Student")
        //     .Join(
        //         "Enrollment",
        //         "Student.StudentNumber",
        //         "Enrollment.StudentNumber")
        //     .Join(
        //         "Course",
        //         "Enrollment.CourseId",
        //         "Course.Id")
        //     .Select(
        //         "Student.StudentNumber",
        //         "Student.FirstName",
        //         "Student.LastName",
        //         "Course.Name")
        //     .Where("Student.IsMale", true)
        //     .WhereIn("Course.Id", new[] { 1, 2, 3 })
        //     .OrderBy("Student.LastName")
        //     .Limit(10)
        //     .Offset(0);

        var compiler = new PostgresCompiler();

        var result = compiler.Compile(query);

        Console.WriteLine("===== SQL =====");
        Console.WriteLine(result.Sql);

        Console.WriteLine();
        Console.WriteLine("===== Bindings =====");

        foreach (var binding in result.Bindings)
        {
            Console.WriteLine(binding);
        }
    }
}