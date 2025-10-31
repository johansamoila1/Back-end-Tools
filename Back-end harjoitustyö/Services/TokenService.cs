using System.Collections.Concurrent;

namespace Back_end_harjoitustyö.Services
{
    public static class TokenService
    {
        private static readonly ConcurrentDictionary<string, int> _tokens = new();

        public static string GenerateToken(int userId)
        {
            var token = Guid.NewGuid().ToString();
            _tokens[token] = userId;
            return token;
        }

        public static int? ValidateToken(string token)
        {
            return _tokens.TryGetValue(token, out var userId) ? userId : (int?)null;
        }

        public static void InvalidateToken(string token)
        {
            _tokens.TryRemove(token, out _);
        }
    }
}
