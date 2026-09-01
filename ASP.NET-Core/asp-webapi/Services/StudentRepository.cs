using asp_webapi.models;
using SqlKata.Execution;
using asp_webapi.Services.Abstractions;

namespace asp_webapi.Services;

public class StudentRepository : IStudentRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public StudentRepository(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public IEnumerable<Student> GetAll(string dbType) =>
        Db(dbType).Query("Student").Get<Student>();

    public Student? GetByStudentNumber(string dbType, string studentNumber) =>
        Db(dbType).Query("Student").Where("StudentNumber", studentNumber).FirstOrDefault<Student>();

    public void Create(string dbType, Student student)
    {
        Db(dbType).Query("Student").Insert(new
        {
            student.StudentNumber,
            student.FirstName,
            student.LastName,
            student.DateOfBirth,
            student.Grade,
            student.IsMale,
            student.LeftUnitsCount
        });
    }

    public bool Update(string dbType, string studentNumber, Student student)
    {
        var db = Db(dbType);
        if (!db.Query("Student").Where("StudentNumber", studentNumber).Exists())
        {
            return false;
        }

        db.Query("Student").Where("StudentNumber", studentNumber).Update(new
        {
            student.FirstName,
            student.LastName,
            student.DateOfBirth,
            student.Grade,
            student.IsMale,
            student.LeftUnitsCount
        });
        return true;
    }

    public bool Delete(string dbType, string studentNumber) =>
        Db(dbType).Query("Student").Where("StudentNumber", studentNumber).Delete() > 0;

    private QueryFactory Db(string dbType) => _databaseFactory.CreateQueryFactory(dbType);
}