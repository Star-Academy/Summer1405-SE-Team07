using Microsoft.AspNetCore.Mvc;
using asp_webapi.Exceptions;
using asp_webapi.models;
using asp_webapi.Services.Abstractions;

namespace asp_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;

    public StudentController(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string db) =>
        Ok(_studentRepository.GetAll(db));

    [HttpGet("{id}")]
    public IActionResult GetById(string id, [FromQuery] string db)
    {
        var student = _studentRepository.GetByStudentNumber(db, id)
                      ?? throw new EntityNotFoundException($"Student with id {id} in {db} not found.");
        return Ok(student);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Student newStudent, [FromQuery] string db)
    {
        _studentRepository.Create(db, newStudent);
        return CreatedAtAction(nameof(GetById), new { id = newStudent.StudentNumber, db }, newStudent);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Student updatedStudent, [FromQuery] string db)
    {
        if (!_studentRepository.Update(db, id, updatedStudent))
        {
            throw new EntityNotFoundException($"Student with id {id} in {db} not found for update.");
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id, [FromQuery] string db)
    {
        if (!_studentRepository.Delete(db, id))
        {
            throw new EntityNotFoundException($"Student with id {id} not found for delete.");
        }
        return NoContent();
    }
}