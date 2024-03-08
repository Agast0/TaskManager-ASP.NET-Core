using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.DTOs;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace TaskManager.Controlers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApiContext _context;
        public UserController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("/get/user/{username}")]
        public ActionResult<GetUserResponseDTO> GetUser([FromRoute] string username)
        {
            var user = _context.Users.Find(username);
            if (user == null)
            {
                return NotFound();
            }

            var userResponseDTO = new GetUserResponseDTO
            {
                Username = user.Username
            };

            return Ok(userResponseDTO);
        }

        [HttpPost("/user/register")]
        public ActionResult<User> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _context.Users.Find(user.Username);
            if (existingUser != null)
            {
                return Conflict("User with this username already exists.");
            }

            string hashedPassword = HashPassword(user.Password);
            user.Password = hashedPassword;

            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUser), new { username = user.Username }, user);
        }

        [HttpPost("/user/login")]
        public ActionResult Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _context.Users.Find(user.Username);
            if (existingUser == null)
            {
                return NotFound();
            }

            string hashedPassword = HashPassword(user.Password);

            if (existingUser.Password == hashedPassword)
            {
                return Ok("Login successful");
            }
            else
            {
                return Unauthorized("Incorrect credentials");
            }
        }

        [HttpDelete("/user/delete/{username}")]
        public ActionResult DeleteTask(string username)
        {
            var existingUser = _context.Users.Find(username);
            if (existingUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(existingUser);
            _context.SaveChanges();

            return Ok();
        }

        private string HashPassword(string password)
        {
            string salt = "7buVtajQ0UjY6MedSXtwFisNugLPjYZZ";
            byte[] saltBytes = Convert.FromBase64String(salt);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));
            return hashedPassword;
        }
    }
}
