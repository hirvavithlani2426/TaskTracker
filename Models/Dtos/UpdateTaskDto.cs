using System.ComponentModel.DataAnnotations;
using TaskTracker.Api.Models.Domain;

namespace TaskTracker.Api.Models.Dtos;

public class UpdateTaskDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public Domain.TaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}