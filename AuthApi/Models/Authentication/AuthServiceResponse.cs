namespace AuthAi.Models.Authentication
{
    public class AuthServiceResponse
    {
        public bool IsSuccess { get; set; }
        public ResponseStatus Status { get; set; }
        public object? Data { get; set; }
    }
}
