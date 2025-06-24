using System.Collections.Generic;
using System.Linq; // Added for good measure, though not directly used in QuizQuestion itself

namespace CyberSecurityPoeWPF // << IMPORTANT: Ensure this matches your project's root namespace
{
    public class QuizQuestion
    {
        // All non-nullable reference type properties are initialized to string.Empty
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // E.g., "MCQ", "TF"
        public List<string> Options { get; set; } = new List<string>(); // Initialize with empty list
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;

        // Constructor for creating quiz questions
        public QuizQuestion(string question, string type, string correctAnswer, string explanation, List<string>? options = null)
        {
            Question = question ?? string.Empty;
            Type = type ?? string.Empty;
            CorrectAnswer = correctAnswer ?? string.Empty;
            Explanation = explanation ?? string.Empty;
            // If options are provided, use them; otherwise, initialize an empty list
            Options = options ?? new List<string>();
        }

        // Parameterless constructor is useful for deserialization (e.g., from JSON)
        // It must ensure all non-nullable properties are initialized.
        public QuizQuestion()
        {
            Question = string.Empty;
            Type = string.Empty;
            Options = new List<string>();
            CorrectAnswer = string.Empty;
            Explanation = string.Empty;
        }
    }
}