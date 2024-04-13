using AuthAi.Models.Authentication;

namespace AuthAi.Models
{
    public class ServerResponse
    {
        public ServerResponse(ResponseStatus status)
        {
            Status = status;
        }

        public ServerResponse(ResponseStatus status, object? data = null)
        {
            Status = status;
            Data = data;
        }
        public ResponseStatus Status { get; set; }
        public object? Data { get; set; }
    }
}
