using Back_end_harjoitustyö.Models;
using Back_end_harjoitustyö.Services;
using Microsoft.AspNetCore.Mvc;

namespace Back_end_harjoitustyö.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAllPublic()
        {
            return Ok(await _messageService.GetAllPublicMessagesAsync());
        }

        [HttpGet("private/{userId}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetPrivateForUser(int userId, [FromHeader] int currentUserId)
        {
            if (userId != currentUserId) return Unauthorized("You can only view your own private messages");
            return Ok(await _messageService.GetPrivateMessagesForUserAsync(userId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDTO>> GetById(int id, [FromHeader] int currentUserId)
        {
            var message = await _messageService.GetMessageByIdAsync(id);
            if (message == null) return NotFound();
            if (message.ReceiverId != null && message.SenderId != currentUserId && message.ReceiverId != currentUserId)
                return Unauthorized("You can only view your own messages or public messages");
            return Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> Create(MessageDTO message, [FromHeader] int currentUserId)
        {
            if (message.SenderId != currentUserId) return Unauthorized("You can only create messages as yourself");
            var createdMessage = await _messageService.CreateMessageAsync(message);
            return CreatedAtAction(nameof(GetById), new { id = createdMessage.Id }, createdMessage);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MessageDTO>> Update(int id, MessageDTO message, [FromHeader] int currentUserId)
        {
            if (id != message.Id) return BadRequest();
            var existingMessage = await _messageService.GetMessageByIdAsync(id);
            if (existingMessage == null || existingMessage.SenderId != currentUserId)
                return Unauthorized("You can only edit your own messages");
            var updatedMessage = await _messageService.UpdateMessageAsync(message);
            if (updatedMessage == null) return NotFound();
            return Ok(updatedMessage);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] int currentUserId)
        {
            var existingMessage = await _messageService.GetMessageByIdAsync(id);
            if (existingMessage == null || existingMessage.SenderId != currentUserId)
                return Unauthorized("You can only delete your own messages");
            var success = await _messageService.DeleteMessageAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}