using System;
using System.Collections.Generic;
using System.Linq; // Crucial for extension methods like .Any(), .OrderBy(), .Reverse()
using System.Text;
using System.Windows;         // For Application, Window, MessageBox
using System.Windows.Controls; // For RichTextBox, TextBox, Button
using System.Windows.Documents; // For FlowDocument, Paragraph, Run
using System.Windows.Media;   // For Brushes, Color
using System.Windows.Input;   // For Key, KeyEventArgs
// System.Media is needed indirectly via AudioPlayer.
using System.IO;              // For Path.Combine, File.Exists
using System.Globalization;   // For CultureInfo
using System.Threading.Tasks; // For Task.Delay

namespace CyberSecurityPoeWPF // IMPORTANT: Ensure this matches your project's root namespace
{
    public partial class MainWindow : Window
    {
        private CyberAwarenessChatbot chatbot;
        private bool isQuizActive = false;
        private Queue<QuizQuestion>? currentQuizQuestions; // Marked as nullable
        private QuizQuestion? currentQuestion;            // Marked as nullable
        private int quizScore = 0;
        private int questionsAttempted = 0;
        private string? userName; // Marked as nullable as it's set after initialization

        // State variables for multi-step inputs
        private Action<string>? currentInputProcessor; // Delegate to hold the next step's processing method

        // Instance of the AudioPlayer
        private AudioPlayer _audioPlayer;

        public MainWindow()
        {
            InitializeComponent();
            chatbot = new CyberAwarenessChatbot();
            // Initialize AudioPlayer, passing a method to log messages to the UI.
            // This allows AudioPlayer to display messages/errors in rtbConversation.
            _audioPlayer = new AudioPlayer(AppendChatbotMessage);
            InitializeChatbotUI();
        }

        private void InitializeChatbotUI()
        {
            // Initializing currentInputProcessor to handle the user name input first
            currentInputProcessor = ProcessNameInput;

            // Event handlers for buttons and text box
            btnSend.Click += BtnSend_Click;
            txtInput.KeyDown += TxtInput_KeyDown; // Using KeyDown for WPF

            btnMenu.Click += BtnMenu_Click;
            btnStartQuiz.Click += BtnStartQuiz_Click;
            btnAddTask.Click += BtnAddTask_Click;
            btnShowTasks.Click += BtnShowTasks_Click;
            btnCompleteTask.Click += BtnCompleteTask_Click;
            btnShowLog.Click += BtnShowLog_Click;
            btnExit.Click += BtnExit_Click;

            // Set focus to input box
            txtInput.Focus();

            // Call the AudioPlayer instance to play the greeting.
            // All audio-related logic and error handling is now encapsulated in AudioPlayer.
            _audioPlayer.PlayGreeting();

            AppendChatbotMessage("Initializing Cyber Awareness Hub...", Brushes.Cyan);
            // Simulate typing delay without freezing UI thread
            Task.Delay(1000).ContinueWith((t) =>
            {
                // Use Dispatcher.Invoke for UI updates from a background thread
                Dispatcher.Invoke(() =>
                {
                    GetUserNameAndStore(); // This will now prompt within the UI
                });
            });

            // Display the ASCII art
            AppendAsciiArt();
        }

        private void AppendAsciiArt()
        {
            string asciiArt = @"
  _____         _          _____
 / ____|       | |        / ____|
| |  _  _ | |__  ___ _ __| (___  ___ __ _ _ __  _ __   ___ _ __
| | | | | | '_ \/ _ \ '__|\___ \/ __/ _` | '_ \| '_ \ / _ \ '__|
| |___| |_| |_) | __/ |  ____) | (_| (_| | | | | | | | __/ |
 \_____\__,_|_.__/ \___|_| |_____/ \___\__,_|_| |_|_|\___|_|

";
            AppendText(rtbConversation, asciiArt + "\n", Brushes.DarkCyan, isBold: false);
        }

        // WPF equivalent of WinForms AppendText
        private void AppendText(RichTextBox rtb, string text, Brush color, bool isBold = false, TextAlignment alignment = TextAlignment.Left)
        {
            // Always use Dispatcher.Invoke for UI updates from any thread
            Dispatcher.Invoke(() =>
            {
                Paragraph p = new Paragraph();
                p.TextAlignment = alignment;

                Run messageRun = new Run(text);
                messageRun.Foreground = color;
                if (isBold)
                {
                    messageRun.FontWeight = FontWeights.Bold;
                }

                p.Inlines.Add(messageRun);
                rtb.Document.Blocks.Add(p);
                rtb.ScrollToEnd(); // Auto-scroll to the end
            });
        }

        private void AppendUserMessage(string message)
        {
            // Use null-conditional operator or null check for userName
            AppendText(rtbConversation, $"{userName ?? "User"}: {message}\n", Brushes.DarkBlue, isBold: true, alignment: TextAlignment.Right);
        }

        private void AppendChatbotMessage(string message, Brush color)
        {
            AppendText(rtbConversation, $"Chatbot: {message}\n", color, isBold: false, alignment: TextAlignment.Left);
        }

        private void GetUserNameAndStore()
        {
            AppendChatbotMessage("\n🔒 Welcome to Cyber Awareness Hub!\nBefore we begin, what should I call you? ", Brushes.DarkGoldenrod);
            txtInput.Clear();
            txtInput.Focus();
            // The currentInputProcessor is already set to ProcessNameInput in InitializeChatbotUI
        }

        private void ProcessNameInput(string inputName)
        {
            inputName = inputName.Trim();
            if (string.IsNullOrWhiteSpace(inputName))
            {
                AppendChatbotMessage("⚠️ I didn't catch that. Please enter your name: ", Brushes.Red);
            }
            else
            {
                // Capitalize first letter if name is not empty
                userName = inputName.Length > 0 ? char.ToUpper(inputName[0]) + inputName.Substring(1) : inputName;
                chatbot.SetUserName(userName);
                AppendUserMessage(inputName);
                AppendChatbotMessage($"🛡️ Welcome, {userName}! I'm your Cyber Awareness Assistant.\nI'm here to help you stay safe in the digital world.\n\n", Brushes.Green);
                AppendChatbotMessage("Type 'menu' to see topics, or ask me anything about cybersecurity!", Brushes.DeepPink);

                // Reset input handler to general processing
                currentInputProcessor = ProcessGeneralUserInput;
                txtInput.Clear();
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            // Use the currently assigned input processor
            currentInputProcessor?.Invoke(txtInput.Text);
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent default Enter key behavior (like new line)
                currentInputProcessor?.Invoke(txtInput.Text);
            }
        }

        // --- Core User Input Processing ---
        private void ProcessGeneralUserInput(string userInput)
        {
            userInput = userInput.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            AppendUserMessage(userInput);
            txtInput.Clear();

            if (isQuizActive)
            {
                HandleQuizAnswer(userInput);
                return; // Do not process as general chatbot input if quiz is active
            }

            string response = "";
            string lowerInput = userInput.ToLower();

            // Handle special commands first
            if (lowerInput == "exit")
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    AppendChatbotMessage("Goodbye! Remember to stay cyber aware.", Brushes.Cyan);
                    Application.Current.Shutdown(); // Properly shuts down the WPF application
                    return;
                }
                else
                {
                    response = "Okay, let's continue!";
                }
            }
            else if (lowerInput == "menu")
            {
                BtnMenu_Click(null, null); // Simulate menu button click
                return;
            }
            else if (lowerInput == "start quiz" || lowerInput == "take quiz")
            {
                BtnStartQuiz_Click(null, null); // Simulate quiz button click
                return;
            }
            else if (lowerInput == "add a task" || lowerInput == "create task")
            {
                HandleAddTaskUI();
                return;
            }
            else if (lowerInput == "show my tasks" || lowerInput == "list tasks")
            {
                response = chatbot.DisplayTasks();
            }
            else if (lowerInput.StartsWith("complete task"))
            {
                // If the command includes a number, try to complete directly
                if (int.TryParse(lowerInput.Replace("complete task", "").Trim(), out int taskNum))
                {
                    response = chatbot.CompleteTask(taskNum);
                }
                else
                {
                    // If no number, prompt for it
                    HandleCompleteTaskUI();
                    return;
                }
            }
            else if (lowerInput.StartsWith("set a reminder") || lowerInput.StartsWith("remind me"))
            {
                HandleSetReminderUI();
                return;
            }
            else if (lowerInput.Contains("show activity log") || lowerInput.Contains("what have you done") || lowerInput.Contains("my history"))
            {
                response = chatbot.DisplayActivityLog();
            }
            else if (lowerInput == "show full log")
            {
                response = chatbot.DisplayFullActivityLog();
            }
            else
            {
                // Let the chatbot logic handle the response
                response = chatbot.GetChatbotResponse(userInput);
            }

            AppendChatbotMessage(response, Brushes.Green);
            txtInput.Focus();
        }

        // --- Event Handlers for Buttons (Standard WPF signature) ---
        // Sender and EventArgs are now the standard WPF types: object, RoutedEventArgs
        // Using `object? sender, RoutedEventArgs? e` to allow `null, null` calls from other methods.
        private void BtnMenu_Click(object? sender, RoutedEventArgs? e)
        {
            AppendChatbotMessage(chatbot.DisplayMainMenuText(), Brushes.DarkGoldenrod);
        }

        private void BtnStartQuiz_Click(object? sender, RoutedEventArgs? e)
        {
            StartQuizUI();
        }

        private void BtnAddTask_Click(object? sender, RoutedEventArgs? e)
        {
            HandleAddTaskUI();
        }

        private void BtnShowTasks_Click(object? sender, RoutedEventArgs? e)
        {
            AppendChatbotMessage(chatbot.DisplayTasks(), Brushes.Green);
        }

        private void BtnCompleteTask_Click(object? sender, RoutedEventArgs? e)
        {
            HandleCompleteTaskUI();
        }

        private void BtnShowLog_Click(object? sender, RoutedEventArgs? e)
        {
            AppendChatbotMessage(chatbot.DisplayActivityLog(), Brushes.Green);
        }

        private void BtnExit_Click(object? sender, RoutedEventArgs? e)
        {
            ProcessGeneralUserInput("exit"); // Use existing exit logic
        }

        // --- UI-driven Task/Reminder Methods (Multi-step input management) ---
        private void HandleAddTaskUI()
        {
            AppendChatbotMessage("What task would you like to add? (e.g., 'Enable two-factor authentication')", Brushes.Black);
            txtInput.Clear();
            currentInputProcessor = ProcessAddTaskDescription; // Set next step
        }

        private void ProcessAddTaskDescription(string description)
        {
            AppendUserMessage(description);
            txtInput.Clear();

            if (string.IsNullOrWhiteSpace(description))
            {
                AppendChatbotMessage("Task description cannot be empty. Please try again or type 'cancel'.", Brushes.Red);
                return;
            }
            if (description.ToLower() == "cancel")
            {
                AppendChatbotMessage("Task addition cancelled.", Brushes.Gray);
                ResetInputHandlers();
                return;
            }

            chatbot.TempTaskDescription = description;

            AppendChatbotMessage("Do you want to set a reminder date for this task? (yes/no)", Brushes.Black);
            currentInputProcessor = ProcessAddTaskDueDatePrompt; // Set next step
        }

        private void ProcessAddTaskDueDatePrompt(string response)
        {
            AppendUserMessage(response);
            txtInput.Clear();

            if (response.ToLower() == "yes" || response.ToLower() == "y")
            {
                AppendChatbotMessage("Please enter the due date for the task (YYYY-MM-DD):", Brushes.Black);
                currentInputProcessor = ProcessAddTaskDueDate; // Set next step
            }
            else
            {
                // Use null-forgiving operator '!' as TempTaskDescription should not be null here
                AppendChatbotMessage(chatbot.AddTask(chatbot.TempTaskDescription!, null), Brushes.Green);
                chatbot.TempTaskDescription = null; // Clear temp
                ResetInputHandlers();
            }
        }

        private void ProcessAddTaskDueDate(string dateInput)
        {
            AppendUserMessage(dateInput);
            txtInput.Clear();

            DateTime? dueDate = null;
            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                dueDate = parsedDate;
            }
            else
            {
                AppendChatbotMessage("Invalid date format. Task added without a specific due date.", Brushes.Red);
            }
            // Use null-forgiving operator '!'
            AppendChatbotMessage(chatbot.AddTask(chatbot.TempTaskDescription!, dueDate), Brushes.Green);
            chatbot.TempTaskDescription = null; // Clear temp
            ResetInputHandlers();
        }

        private void HandleCompleteTaskUI()
        {
            string response = chatbot.DisplayTasks();
            AppendChatbotMessage(response, Brushes.Green);

            if (!chatbot.UserTasks.Any())
            {
                AppendChatbotMessage("You have no tasks to complete.", Brushes.Orange);
                ResetInputHandlers();
                return;
            }

            AppendChatbotMessage("Which task would you like to mark as complete? Please enter the number:", Brushes.Black);
            txtInput.Clear();
            currentInputProcessor = ProcessCompleteTaskNumber; // Set next step
        }

        private void ProcessCompleteTaskNumber(string input)
        {
            AppendUserMessage(input);
            txtInput.Clear();

            if (int.TryParse(input, out int taskNumber))
            {
                AppendChatbotMessage(chatbot.CompleteTask(taskNumber), Brushes.Green);
            }
            else
            {
                AppendChatbotMessage("Invalid input. Please enter a valid task number.", Brushes.Red);
            }
            ResetInputHandlers();
        }

        private void HandleSetReminderUI()
        {
            AppendChatbotMessage("What would you like me to remind you about?", Brushes.Black);
            txtInput.Clear();
            currentInputProcessor = ProcessReminderSubject; // Set next step
        }

        private void ProcessReminderSubject(string subject)
        {
            AppendUserMessage(subject);
            txtInput.Clear();

            if (string.IsNullOrWhiteSpace(subject))
            {
                AppendChatbotMessage("Reminder subject cannot be empty. Please try again or type 'cancel'.", Brushes.Red);
                return;
            }
            if (subject.ToLower() == "cancel")
            {
                AppendChatbotMessage("Reminder setting cancelled.", Brushes.Gray);
                ResetInputHandlers();
                return;
            }

            chatbot.TempReminderSubject = subject;

            AppendChatbotMessage("When should I remind you? (e.g., 'tomorrow', 'next week', 'YYYY-MM-DD')", Brushes.Black);
            currentInputProcessor = ProcessReminderDate; // Set next step
        }

        private void ProcessReminderDate(string dateInput)
        {
            AppendUserMessage(dateInput);
            txtInput.Clear();

            // Use null-forgiving operator '!'
            string result = chatbot.SetReminder(chatbot.TempReminderSubject!, dateInput);
            AppendChatbotMessage(result, result.Contains("Invalid") || result.Contains("couldn't") ? Brushes.Red : Brushes.Green);
            chatbot.TempReminderSubject = null; // Clear temp
            ResetInputHandlers();
        }

        private void ResetInputHandlers()
        {
            currentInputProcessor = ProcessGeneralUserInput; // Reset to general processing
            txtInput.Focus();
        }

        // --- UI-driven Quiz Methods ---
        private void StartQuizUI()
        {
            if (isQuizActive)
            {
                AppendChatbotMessage("A quiz is already in progress. Please answer the current question.", Brushes.Orange);
                return;
            }

            AppendChatbotMessage("\n--- Cybersecurity Quiz Time! ---", Brushes.DeepPink);
            AppendChatbotMessage("Answer the following questions to test your knowledge.", Brushes.DeepPink);
            AppendChatbotMessage("Type 'A', 'B', 'C', 'D' for multiple choice or 'True'/'False'.\n", Brushes.DeepPink);

            quizScore = 0;
            questionsAttempted = 0;
            isQuizActive = true;
            // Access QuizQuestions from the chatbot instance. OrderBy() needs System.Linq.
            currentQuizQuestions = new Queue<QuizQuestion>(chatbot.QuizQuestions.OrderBy(q => chatbot.RandomInstance.Next()));

            DisplayNextQuizQuestion();
            currentInputProcessor = HandleQuizAnswer; // Set quiz answer handler
        }

        private void DisplayNextQuizQuestion()
        {
            // Null check for currentQuizQuestions before trying to use it
            if (currentQuizQuestions != null && currentQuizQuestions.Any())
            {
                currentQuestion = currentQuizQuestions.Dequeue();
                questionsAttempted++;
                // Use null-forgiving operator '!' after the null check
                AppendChatbotMessage($"\nQuestion {questionsAttempted}: {currentQuestion!.Question}", Brushes.Blue);

                if (currentQuestion.Type == "MCQ") // currentQuestion is guaranteed non-null here
                {
                    // Options is initialized to new List<string>() in QuizQuestion, so it's not null.
                    foreach (var option in currentQuestion.Options)
                    {
                        AppendChatbotMessage(option, Brushes.DarkBlue);
                    }
                }
                AppendChatbotMessage("Your answer: ", Brushes.Blue);
                txtInput.Clear();
                txtInput.Focus();
            }
            else
            {
                EndQuizUI();
            }
        }

        private void HandleQuizAnswer(string userAnswer)
        {
            AppendUserMessage(userAnswer);
            txtInput.Clear();

            // Explicit null check for currentQuestion
            if (currentQuestion == null)
            {
                AppendChatbotMessage("Error: No active quiz question.", Brushes.Red);
                EndQuizUI();
                return;
            }

            string normalizedAnswer = userAnswer.ToUpper().Trim();
            // currentQuestion is guaranteed non-null here
            string normalizedCorrectAnswer = currentQuestion.CorrectAnswer.ToUpper().Trim();

            if (normalizedAnswer == normalizedCorrectAnswer)
            {
                AppendChatbotMessage("Correct! 🎉", Brushes.Green);
                quizScore++;
            }
            else
            {
                // currentQuestion is guaranteed non-null here
                AppendChatbotMessage($"Incorrect. The correct answer was {currentQuestion.CorrectAnswer}. {currentQuestion.Explanation}", Brushes.Red);
            }

            // Move to next question or end quiz
            DisplayNextQuizQuestion();
        }

        private void EndQuizUI()
        {
            isQuizActive = false;
            AppendChatbotMessage($"\nQuiz complete! You scored {quizScore} out of {questionsAttempted}. Keep learning to boost your cyber awareness!", Brushes.DeepPink);
            chatbot.LogActivity("Quiz Completed", $"User scored {quizScore} out of {questionsAttempted} on the quiz.");
            ResetInputHandlers(); // Reset to general processing
        }
    }
}
