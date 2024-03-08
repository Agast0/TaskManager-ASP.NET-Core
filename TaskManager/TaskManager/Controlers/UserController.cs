using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.DTOs;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Controlers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IConfiguration _config;

        public UserController(ApiContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
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
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                };

                var Sectoken = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                return Ok(token);
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
            string salt = _config["Crypto:SecretKey"];
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
