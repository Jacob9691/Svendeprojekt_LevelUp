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

    /// <summary>
    /// Opretter en opgave. Med "repeat" og "repeatUntil" oprettes én opgave pr. gentagelse i en serie,
    /// og den første returneres.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request)
    {
        var task = await taskService.CreateTaskAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    /// <summary>Redigerer én opgave – også hvis den er del af en serie.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, SaveTaskRequest request) =>
        await taskService.UpdateTaskAsync(id, request);

    /// <summary>Sletter opgaven. Med ?following=true slettes også senere, ikke-gennemførte opgaver i serien.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id, [FromQuery] bool following = false)
    {
        await taskService.DeleteTaskAsync(id, following);
        return NoContent();
    }

    /// <summary>Markerer opgaven som gennemført og giver XP og points.</summary>
    [HttpPost("{id:int}/complete")]
    public async Task<ActionResult<CompleteTaskResponse>> CompleteTask(int id) =>
        await taskService.CompleteTaskAsync(id);
}
