using TaskManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TaskManager.Data;
using Microsoft.AspNetCore.Authorization;
using TaskManager.DTOs;
using System.Security.Claims;

namespace TaskManager.Controlers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ApiContext _context;
        public TaskController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("/get/{id}")]
        [AllowAnonymous]
        public ActionResult<TaskDTO> GetTask([FromRoute] int id)
        {
            var task = _context.Task.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpGet("/get")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Models.Task>> GetAllTasks()
        {
            var tasks = _context.Task.ToList();
            return Ok(tasks);
        }

        [HttpGet("/getWithUsername/{username}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Models.Task>> GetAllTasksWithUsername([FromRoute] string username)
        {
            var tasks = _context.Task.Where(t => t.Username == username).ToList();

            return Ok(tasks);
        }

        [HttpPost("/create")]
        public ActionResult<Models.Task> CreateTask(TaskDTO taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var task = new Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                IsCompleted = taskDto.IsCompleted,
                Username = username
            };

            _context.Task.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("/update/{id}")]
        public ActionResult<Models.Task> UpdateTask(int id, TaskDTO taskDTO)
        {
            var existingTask = _context.Task.Find(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (existingTask.Username != username)
            {
                return Unauthorized("You are not authorized to update this task.");
            }

            existingTask.Title = taskDTO.Title;
            existingTask.Description = taskDTO.Description;
            existingTask.DueDate = taskDTO.DueDate;
            existingTask.IsCompleted = taskDTO.IsCompleted;

            _context.Task.Update(existingTask);
            _context.SaveChanges();

            return Ok(existingTask);
        }

        [HttpDelete("/delete/{id}")]
        public ActionResult DeleteTask(int id)
        {
            var existingTask = _context.Task.Find(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (existingTask.Username != username)
            {
                return Unauthorized("You are not authorized to delete this task.");
            }

            _context.Task.Remove(existingTask);
            _context.SaveChanges();

            return Ok();
        }
    }
}
