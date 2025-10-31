using Back_end_harjoitustyö.Models;
using Back_end_harjoitustyö.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Back_end_harjoitustyö.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly MessageAppContext _context;

        public UserService(IUserRepository userRepository, IMessageRepository messageRepository, MessageAppContext context)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            return await _userRepository.AddAsync(user);
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null) return null;

            existingUser.Username = user.Username;
            if (!string.IsNullOrEmpty(user.Password) && user.Password.Length >= 6)
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            else if (!string.IsNullOrEmpty(user.Password))
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.LastLogin = user.LastLogin;

            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sentMessages = await _context.Messages
                    .Where(m => m.SenderId == id)
                    .ToListAsync();
                _context.Messages.RemoveRange(sentMessages);

                var receivedMessages = await _context.Messages
                    .Where(m => m.ReceiverId == id)
                    .ToListAsync();
                foreach (var message in receivedMessages)
                {
                    message.ReceiverId = null;
                }

                var messagesWithPrevious = await _context.Messages
                    .Where(m => m.PreviousMessageId != null)
                    .ToListAsync();

                foreach (var message in messagesWithPrevious)
                {
                    if (sentMessages.Any(m => m.Id == message.PreviousMessageId))
                    {
                        message.PreviousMessageId = null;
                    }
                }

                await _context.SaveChangesAsync();

                var result = await _userRepository.DeleteAsync(id);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}