using Back_end_harjoitustyö.Models;
using Microsoft.EntityFrameworkCore;

namespace Back_end_harjoitustyö.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageAppContext _context;

        public MessageRepository(MessageAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllPublicAsync()
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == null)
                .Include(m => m.Sender)
                .Include(m => m.PreviousMessage)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetPrivateForUserAsync(int userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.PreviousMessage)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message?> UpdateAsync(Message message)
        {
            var existingMessage = await _context.Messages.FindAsync(message.Id);
            if (existingMessage == null) return null;

            existingMessage.Title = message.Title;
            existingMessage.Content = message.Content;

            await _context.SaveChangesAsync();
            return existingMessage;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null) return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}