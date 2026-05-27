using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Models.Dtos;
using TaskTracker.Api.Services;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll()
    {
        return Ok(await _taskService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create([FromBody] CreateTaskDto dto)
    {
        var (result, error) = await _taskService.CreateAsync(dto);
        if (error != null) return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var (success, error) = await _taskService.UpdateAsync(id, dto);

        if (!success)
        {
            if (error == "NotFound") return NotFound();
            return BadRequest(new { message = error });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _taskService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}