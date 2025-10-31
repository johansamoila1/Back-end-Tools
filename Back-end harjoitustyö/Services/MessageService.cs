using Back_end_harjoitustyö.Models;
using Back_end_harjoitustyö.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Back_end_harjoitustyö.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly MessageAppContext _context;

        public MessageService(IMessageRepository messageRepository, MessageAppContext context)
        {
            _messageRepository = messageRepository;
            _context = context;
        }

        public async Task<IEnumerable<MessageDTO>> GetAllPublicMessagesAsync()
        {
            var messages = await _messageRepository.GetAllPublicAsync();
            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                Title = m.Title,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                PreviousMessageId = m.PreviousMessageId,
                CreatedAt = m.CreatedAt
            });
        }

        public async Task<IEnumerable<MessageDTO>> GetPrivateMessagesForUserAsync(int userId)
        {
            var messages = await _messageRepository.GetPrivateForUserAsync(userId);
            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                Title = m.Title,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                PreviousMessageId = m.PreviousMessageId,
                CreatedAt = m.CreatedAt
            });
        }

        public async Task<MessageDTO?> GetMessageByIdAsync(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null) return null;
            return new MessageDTO
            {
                Id = message.Id,
                Title = message.Title,
                Content = message.Content,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                PreviousMessageId = message.PreviousMessageId,
                CreatedAt = message.CreatedAt
            };
        }

        public async Task<MessageDTO> CreateMessageAsync(MessageDTO messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Title) || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                throw new ArgumentException("Title and Content cannot be empty or whitespace.");
            }

            var senderExists = await _context.Users.AnyAsync(u => u.Id == messageDto.SenderId);
            if (!senderExists)
                throw new Exception($"Sender with ID {messageDto.SenderId} does not exist.");

            if (messageDto.ReceiverId.HasValue)
            {
                var receiverExists = await _context.Users.AnyAsync(u => u.Id == messageDto.ReceiverId.Value);
                if (!receiverExists)
                    throw new Exception($"Receiver with ID {messageDto.ReceiverId.Value} does not exist.");
            }
            if (messageDto.PreviousMessageId.HasValue)
            {
                var previous = await _messageRepository.GetByIdAsync(messageDto.PreviousMessageId.Value);
                if (previous == null)
                    throw new Exception($"Previous message with ID {messageDto.PreviousMessageId.Value} does not exist.");
            }

            var message = new Message
            {
                Title = messageDto.Title,
                Content = messageDto.Content,
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                PreviousMessageId = messageDto.PreviousMessageId,
                CreatedAt = DateTime.UtcNow
            };
            var createdMessage = await _messageRepository.AddAsync(message);
            return new MessageDTO
            {
                Id = createdMessage.Id,
                Title = createdMessage.Title,
                Content = createdMessage.Content,
                SenderId = createdMessage.SenderId,
                ReceiverId = createdMessage.ReceiverId,
                PreviousMessageId = createdMessage.PreviousMessageId,
                CreatedAt = createdMessage.CreatedAt
            };
        }

        public async Task<MessageDTO?> UpdateMessageAsync(MessageDTO messageDto)
        {
            if (string.IsNullOrWhiteSpace(messageDto.Title) || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                throw new ArgumentException("Title and Content cannot be empty or whitespace.");
            }
            if (messageDto.PreviousMessageId.HasValue)
            {
                var previous = await _messageRepository.GetByIdAsync(messageDto.PreviousMessageId.Value);
                if (previous == null)
                    throw new Exception($"Previous message with ID {messageDto.PreviousMessageId.Value} does not exist.");
            }

            var message = await _messageRepository.GetByIdAsync(messageDto.Id);
            if (message == null) return null;

            message.Title = messageDto.Title;
            message.Content = messageDto.Content;
            var updatedMessage = await _messageRepository.UpdateAsync(message);
            if (updatedMessage == null) return null;

            return new MessageDTO
            {
                Id = updatedMessage.Id,
                Title = updatedMessage.Title,
                Content = updatedMessage.Content,
                SenderId = updatedMessage.SenderId,
                ReceiverId = updatedMessage.ReceiverId,
                PreviousMessageId = updatedMessage.PreviousMessageId,
                CreatedAt = updatedMessage.CreatedAt
            };
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _messageRepository.DeleteAsync(id);
        }
    }
}