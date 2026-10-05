using entra_auth_lab.Api.Dtos;
using entra_auth_lab.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace entra_auth_lab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await userService.GetByIdAsync(id);
            return Ok(user);
        }
        [HttpGet("{externalId:Guid}")]
        public async Task<ActionResult<UserDto>> GetByExternalId(Guid externalId)
        {
            var user = await userService.GetByExternalIdAsync(externalId);
            return Ok(user);
        }
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await userService.GetAsync();
            return Ok(users);
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest user)
        {
            var createdUser = await userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] CreateUserRequest user)
        {
            await userService.UpdateAsync(id, user);
            return NoContent();
        }
    }
}