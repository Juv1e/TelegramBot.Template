namespace TelegramBot.Template;

public static class AntiFlood
{
    private static readonly Dictionary<long, List<DateTimeOffset>> _userMessageTimes = new();

    private const int MaxMessagesPerInterval = 3;
    private const int MessageIntervalSeconds = 10;

    public static bool IsFlood(long userId)
    {
        if (!_userMessageTimes.TryGetValue(userId, out var messageTimes))
        {
            messageTimes = new List<DateTimeOffset>();
            _userMessageTimes[userId] = messageTimes;
        }
        messageTimes.Add(DateTimeOffset.Now);
        messageTimes.RemoveAll(t => (DateTime.UtcNow - t).TotalSeconds > MessageIntervalSeconds);
        return messageTimes.Count > MaxMessagesPerInterval;
    }
}