namespace NotificationService.Domain.Notifications
{
    public class InAppNotification
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime SentAt { get; private set; }

        private InAppNotification()
        {

        }
        public InAppNotification(
        int userId,
        string title,
        string message,
        NotificationType type)
        {

            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than 0.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.");

            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            SentAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            if (IsRead)
                return;
           IsRead = true;
        }

    }
}