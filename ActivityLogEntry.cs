using System;

namespace CyberSecurityPoeWPF // << IMPORTANT: Ensure this matches your project's root namespace
{
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }

        public ActivityLogEntry(string action, string details)
        {
            Timestamp = DateTime.Now;
            // Ensure properties are initialized to non-null values
            Action = action ?? string.Empty;
            Details = details ?? string.Empty;
        }
    }
}