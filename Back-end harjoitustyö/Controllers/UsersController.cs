using Back_end_harjoitustyö.Models;
using Back_end_harjoitustyö.Services;
using Microsoft.AspNetCore.Mvc;

namespace Back_end_harjoitustyö.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] int currentUserId)
        {
            if (currentUserId <= 0) return Unauthorized();

            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                JoinDate = u.JoinDate,
                LastLogin = u.LastLogin
            });
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromHeader] int currentUserId)
        {
            if (id != currentUserId) return Unauthorized("You can only view your own profile");
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JoinDate = user.JoinDate,
                LastLogin = user.LastLogin
            };
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Username = userDto.Username,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName
            };

            var created = await _userService.CreateUserAsync(user);
            var resultDto = new UserDTO
            {
                Id = created.Id,
                Username = created.Username,
                FirstName = created.FirstName,
                LastName = created.LastName,
                JoinDate = created.JoinDate,
                LastLogin = created.LastLogin
            };

            return CreatedAtAction(nameof(Get), new { id = created.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDTO userDto, [FromHeader] int currentUserId)
        {
            if (id != userDto.Id || id != currentUserId)
                return Unauthorized("You can only update your own profile");

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Username = userDto.Username;
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;

            var updated = await _userService.UpdateUserAsync(existingUser);
            if (updated == null) return NotFound();

            var updatedDto = new UserDTO
            {
                Id = updated.Id,
                Username = updated.Username,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                JoinDate = updated.JoinDate,
                LastLogin = updated.LastLogin
            };

            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromHeader] int currentUserId)
        {
            if (id != currentUserId) return Unauthorized("You can only delete your own profile");
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.GetByUsernameAsync(model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }
            user.LastLogin = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);
            return Ok(new { UserId = user.Id, Username = user.Username });
        }
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}