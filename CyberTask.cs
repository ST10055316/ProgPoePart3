using System;

namespace CyberSecurityPoeWPF // << IMPORTANT: Ensure this matches your project's root namespace
{
    public class CyberTask
    {
        public string Description { get; set; }
        public DateTime? DueDate { get; set; } // Nullable DateTime
        public bool IsCompleted { get; set; }

        public CyberTask(string description, DateTime? dueDate)
        {
            // Ensure properties are initialized
            Description = description ?? string.Empty;
            DueDate = dueDate; // DueDate can be null
            IsCompleted = false; // Default value
        }
    }
}