using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using asp_webapi.models;
using asp_webapi.Services;


namespace asp_webapi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IDatabaseFactory _databaseFactory;

    public StudentController(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
    
    
    private QueryFactory? GetDb(string? dbType, out IActionResult? errorResult)
    {
        errorResult = null;
        try
        {
            return _databaseFactory.CreateQueryFactory(dbType);
        }
        catch (ArgumentException ex)
        {
            errorResult = BadRequest(new { Message = ex.Message });
            return null;
        }
    }
    
    [HttpGet]
    public IActionResult GetAll([FromQuery] string db)
    {
        var dbQuery = GetDb(db, out var error);
        if (error != null)
        {
            return error;
        } 
        
        var students = dbQuery!.Query("Student").Get<Student>();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id, [FromQuery] string db)
    {
        var dbQuery = GetDb(db, out var error);
        if (error != null) return error;

        var student = dbQuery!.Query("Student").Where("StudentNumber", id).FirstOrDefault<Student>();

        if (student == null)
        {
            return NotFound(new { Message = $"Student with the requested id {id}  in {db} notfound." });
        }
        
        return Ok(student);
    }
    
    
    [HttpPost]
    public IActionResult Create([FromBody] Student newStudent, [FromQuery] string db)
    {
        var dbQuery = GetDb(db, out var error);
        if (error != null) return error;
    
        dbQuery!.Query("Student").Insert(new
        {
            StudentNumber = newStudent.StudentNumber,
            FirstName = newStudent.FirstName,
            LastName = newStudent.LastName,
            DateOfBirth = newStudent.DateOfBirth,
            Grade = newStudent.Grade,
            IsMale = newStudent.IsMale,
            LeftUnitsCount = newStudent.LeftUnitsCount
        });

        return CreatedAtAction(nameof(GetById), new { id = newStudent.StudentNumber, db = db }, newStudent);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Student updatedStudent, [FromQuery] string db)
    {
        var dbQuery = GetDb(db, out var error);
        if (error != null) return error;

        var exists = dbQuery!.Query("Student").Where("StudentNumber", id).Exists();
        if (!exists)
        {
            return NotFound(new { Message = $" student with the requested id {id}  in {db} notfound. for update." });
        }

        dbQuery.Query("Student").Where("StudentNumber", id).Update(new
        {
            FirstName = updatedStudent.FirstName,
            LastName = updatedStudent.LastName,
            DateOfBirth = updatedStudent.DateOfBirth,
            Grade = updatedStudent.Grade,
            IsMale = updatedStudent.IsMale,
            LeftUnitsCount = updatedStudent.LeftUnitsCount
        });

        return NoContent(); 
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id, [FromQuery] string db)
    {
        var dbQuery = GetDb(db, out var error);
        if (error != null) return error;

        var affectedRows = dbQuery!.Query("Student").Where("StudentNumber", id).Delete();

        if (affectedRows == 0)
        {
            return NotFound(new { Message = $"there is no student with this id {id} for delete " });
        }
        return NoContent();
    }
}