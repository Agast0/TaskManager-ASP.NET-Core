using TaskManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TaskManager.Data;

namespace TaskManager.Controlers
{
    [Route("/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApiContext _context;
        public TaskController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("/get/{id}")]
        public ActionResult<Models.Task> GetTask([FromRoute] int id)
        {
            var task = _context.Task.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpGet("/get")]
        public ActionResult<IEnumerable<Models.Task>> GetAllTasks()
        {
            var tasks = _context.Task.ToList();
            return Ok(tasks);
        }

        [HttpPost("/create")]
        public ActionResult<Models.Task> CreateTask(Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Task.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("/update/{id}")]
        public ActionResult<Models.Task> UpdateTask(int id, Models.Task updatedTask)
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

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.DueDate = updatedTask.DueDate;
            existingTask.IsCompleted = updatedTask.IsCompleted;

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

            _context.Task.Remove(existingTask);
            _context.SaveChanges();

            return Ok();
        }
    }
}
