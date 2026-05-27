using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models.Domain;
using TaskTracker.Api.Models.Dtos;
using TaskTracker.Api.Services;
using Xunit;

namespace TaskTracker.Tests;

public class TaskServiceTests
{
	private readonly TaskDbContext _context;
	private readonly ITaskService _service;

	public TaskServiceTests()
	{
		// Sets up a clean, isolated database in memory for each test run
		var options = new DbContextOptionsBuilder<TaskDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new TaskDbContext(options);
		_service = new TaskService(_context);
	}

	[Fact]
	public async Task CreateAsync_WithValidData_ReturnsSuccessPath()
	{
		// Arrange
		var dto = new CreateTaskDto { Title = "Valid Task", Description = "Test Details", Status = Api.Models.Domain.TaskStatus.Todo };

		// Act
		var (result, error) = await _service.CreateAsync(dto);

		// Assert
		Assert.Null(error);
		Assert.NotNull(result);
		Assert.Equal("Valid Task", result.Title);
		Assert.Equal("Todo", result.Status);
	}

	[Fact]
	public async Task CreateAsync_WhenStatusIsDoneAndTitleIsEmpty_ReturnsValidationError()
	{
		// Arrange & Act
		var dto = new CreateTaskDto { Title = "   ", Status = Api.Models.Domain.TaskStatus.Done };
		var (result, error) = await _service.CreateAsync(dto);

		// Assert
		Assert.Null(result);
		Assert.NotNull(error);
		Assert.Contains("cannot be marked as Done", error);
	}

	[Fact]
	public async Task UpdateAsync_WithValidData_ReturnsSuccessfulPath()
	{
		// Arrange
		var existingId = Guid.NewGuid();
		_context.Tasks.Add(new TaskItem { Id = existingId, Title = "Old Title", Status = Api.Models.Domain.TaskStatus.Todo });
		await _context.SaveChangesAsync();

		var updateDto = new UpdateTaskDto { Title = "Updated Title", Status = Api.Models.Domain.TaskStatus.InProgress };

		// Act
		var (success, error) = await _service.UpdateAsync(existingId, updateDto);

		// Assert
		Assert.True(success);
		Assert.Null(error);

		var updatedTask = await _context.Tasks.FindAsync(existingId);
		Assert.Equal("Updated Title", updatedTask!.Title);
		Assert.Equal(Api.Models.Domain.TaskStatus.InProgress, updatedTask.Status);
	}

	[Fact]
	public async Task UpdateAsync_WhenStatusTransitionToDoneWithEmptyTitle_ReturnsValidationError()
	{
		// Arrange
		var existingId = Guid.NewGuid();
		_context.Tasks.Add(new TaskItem { Id = existingId, Title = "Old Title", Status = Api.Models.Domain.TaskStatus.Todo });
		await _context.SaveChangesAsync();

		var updateDto = new UpdateTaskDto { Title = "", Status = Api.Models.Domain.TaskStatus.Done };

		// Act
		var (success, error) = await _service.UpdateAsync(existingId, updateDto);

		// Assert
		Assert.False(success);
		Assert.Equal("A task cannot be marked as Done if the Title is empty or whitespace.", error);
	}
}