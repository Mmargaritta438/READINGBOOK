namespace AuthAi.Models.Authentication
{
    public class ConfirmationCode
    {
        public int ConfirmationCodeId { get; set; }
        public required string UserEmail { get; set; }
        public required string Code { get; set; }
        public required DateTime ExpiryDate { get; set; }
    }
}
