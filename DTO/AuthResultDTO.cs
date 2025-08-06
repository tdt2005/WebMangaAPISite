namespace MangaAPI.DTO
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ReaderID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
