namespace Shared.DTO.User
{
    public record LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserInformation User { get; set; } = new UserInformation();
    }
}
