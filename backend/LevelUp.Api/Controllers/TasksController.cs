using LevelUp.Api.Dtos;
using LevelUp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    /// <summary>Henter brugerens opgaver. Brug from/to til at hente én uge ad gangen.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetTasks([FromQuery] DateTime? from, [FromQuery] DateTime? to) =>
        await taskService.GetTasksAsync(from, to);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id) =>
        await taskService.GetTaskAsync(id);

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(SaveTaskRequest request)
    {
        var task = await taskService.CreateTaskAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, SaveTaskRequest request) =>
        await taskService.UpdateTaskAsync(id, request);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    /// <summary>Markerer opgaven som gennemført og giver XP og points.</summary>
    [HttpPost("{id:int}/complete")]
    public async Task<ActionResult<CompleteTaskResponse>> CompleteTask(int id) =>
        await taskService.CompleteTaskAsync(id);
}
