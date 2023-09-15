namespace RWAProjectApi.DTOs
{
    public class NotificationDTO
    {
        public string ReceiverEmail { get; set; } = null!;

        public string? Subject { get; set; }

        public string Body { get; set; } = null!;
    }
}
