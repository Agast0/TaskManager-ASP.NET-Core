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

        [HttpGet]
        public ActionResult<Models.Task> GetTask(int id)
        {
            var task = _context.Task.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public ActionResult<Models.Task> CreateTask(Models.Task task)
        {
            if (task == null)
            {
                return BadRequest();
            }
            _context.Task.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTask), new { id =  task.Id }, task);
        }
    }
}
