using asp_webapi.models;

namespace asp_webapi.Services.Abstractions;

public interface IStudentRepository
{
    IEnumerable<Student> GetAll(string dbType);
    Student? GetByStudentNumber(string dbType, string studentNumber);
    void Create(string dbType, Student student);
    bool Update(string dbType, string studentNumber, Student student);
    bool Delete(string dbType, string studentNumber);
}