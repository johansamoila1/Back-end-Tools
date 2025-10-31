using Back_end_harjoitustyö.Models;

namespace Back_end_harjoitustyö.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllPublicAsync();
        Task<IEnumerable<Message>> GetPrivateForUserAsync(int userId);
        Task<Message?> GetByIdAsync(int id);
        Task<Message> AddAsync(Message message);
        Task<Message?> UpdateAsync(Message message);
        Task<bool> DeleteAsync(int id);
    }
}