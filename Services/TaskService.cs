using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using Domain = TaskTracker.Api.Models.Domain;
using TaskTracker.Api.Models.Dtos;

namespace TaskTracker.Api.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllAsync();
    Task<TaskResponseDto?> GetByIdAsync(Guid id);
    Task<(TaskResponseDto? Result, string? Error)> CreateAsync(CreateTaskDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class TaskService : ITaskService
{
    private readonly TaskDbContext _context;

    public TaskService(TaskDbContext context) => _context = context;

    public async Task<IEnumerable<TaskResponseDto>> GetAllAsync()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return tasks.Select(MapToResponseDto);
    }

    public async Task<TaskResponseDto?> GetByIdAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        return task == null ? null : MapToResponseDto(task);
    }

    public async Task<(TaskResponseDto? Result, string? Error)> CreateAsync(CreateTaskDto dto)
    {
        // Assignment constraint verification rule
        if (dto.Status == Domain.TaskStatus.Done && string.IsNullOrWhiteSpace(dto.Title))
        {
            return (null, "A task cannot be marked as Done if the Title is empty or whitespace.");
        }

        var task = new Domain.TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Status = dto.Status,
            DueDate = dto.DueDate
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return (MapToResponseDto(task), null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return (false, "NotFound");

        // Assignment constraint verification rule
        if (dto.Status == Domain.TaskStatus.Done && string.IsNullOrWhiteSpace(dto.Title))
        {
            return (false, "A task cannot be marked as Done if the Title is empty or whitespace.");
        }

        task.Title = dto.Title.Trim();
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TaskResponseDto MapToResponseDto(Domain.TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status.ToString(),
        DueDate = task.DueDate
    };
}