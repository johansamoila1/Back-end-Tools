using Back_end_harjoitustyö.Models;

namespace Back_end_harjoitustyö.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetAllPublicMessagesAsync();
        Task<IEnumerable<MessageDTO>> GetPrivateMessagesForUserAsync(int userId);
        Task<MessageDTO?> GetMessageByIdAsync(int id);
        Task<MessageDTO> CreateMessageAsync(MessageDTO message);
        Task<MessageDTO?> UpdateMessageAsync(MessageDTO message);
        Task<bool> DeleteMessageAsync(int id);
    }
}