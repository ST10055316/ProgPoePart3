using CyberSecurityPoeWPF;
using System.Globalization;
using System.Text;

namespace CyberSecurityPoeWPF;

public class CyberAwarenessChatbot
{
    private string? userName; // Can be null until set by user
    private List<ActivityLogEntry> activityLog;
    private List<CyberTask> userTasks;
    private List<QuizQuestion> quizQuestions; // Initialized in constructor, so not nullable here
    private Dictionary<string, string> knowledgeBase;
    private List<string> conversationHistory;
    private string? currentResponse; // Can be null if no response yet

    // Properties for multi-step input handling - made nullable as they are temporary
    public string? TempTaskDescription { get; set; }
    public string? TempReminderSubject { get; set; }

    public Random RandomInstance { get; private set; } // For quiz randomization

    // Public accessors for lists (if UI needs to read them)
    public List<QuizQuestion> QuizQuestions => quizQuestions;
    public List<CyberTask> UserTasks => userTasks;

    public CyberAwarenessChatbot()
    {
        // Initialize all lists and dictionaries to empty instances, not null
        activityLog = new List<ActivityLogEntry>();
        userTasks = new List<CyberTask>();
        quizQuestions = new List<QuizQuestion>(); // Initialize the list
        knowledgeBase = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        conversationHistory = new List<string>();
        RandomInstance = new Random();

        InitializeKnowledgeBase();
        LoadQuizQuestions(); // This populates `quizQuestions`
        LogActivity("Chatbot Initialized", "Chatbot system has started.");
    }

    private void InitializeKnowledgeBase()
    {
        // Your existing knowledge base entries here.
        // (Example provided for context, add all your entries back)
        knowledgeBase.Add("phishing", "Phishing is a type of cyberattack where attackers disguise as trustworthy entities to trick individuals into revealing sensitive information, such as passwords or credit card numbers, typically through fraudulent emails, messages, or websites.");
        knowledgeBase.Add("malware", "Malware, short for malicious software, is any software intentionally designed to cause damage to a computer, server, client, or computer network, or to otherwise compromise the security of a system.");
        knowledgeBase.Add("data encryption", "Data encryption converts data into a code to prevent unauthorized access. It's essential for protecting sensitive information, especially when it's transmitted over networks or stored on devices.");
        knowledgeBase.Add("password", "Strong passwords are crucial. Use a combination of uppercase and lowercase letters, numbers, and symbols. Avoid personal information. Consider using a password manager and enable two-factor authentication (2FA).");
        knowledgeBase.Add("hello", "Hello there! How can I assist you with your cybersecurity awareness today?");
        knowledgeBase.Add("what can you do", "I can tell you about various cybersecurity topics, help you manage tasks, create reminders, and quiz your knowledge. Just ask!");
        knowledgeBase.Add("bye", "Goodbye! Remember to stay cyber aware.");
        // Add all your other knowledge base entries here as `knowledgeBase.Add("key", "value");`
        knowledgeBase.Add("ransomware", "Ransomware is a type of malware that threatens to publish the victim's data or perpetually block access to it unless a ransom is paid. Often, payment is demanded in cryptocurrency.");
        knowledgeBase.Add("2fa", "Two-factor authentication (2FA) adds an extra layer of security beyond just a password. It requires a second piece of information (like a code from your phone) to verify your identity.");
        knowledgeBase.Add("firewall", "A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules. It acts as a barrier between a trusted internal network and untrusted external networks, such as the internet.");
        knowledgeBase.Add("vpn", "A Virtual Private Network (VPN) extends a private network across a public network and enables users to send and receive data across shared or public networks as if their computing devices were directly connected to the private network. This provides increased security and privacy.");
        knowledgeBase.Add("social engineering", "Social engineering is the psychological manipulation of people into performing actions or divulging confidential information. It's a common tactic in cyberattacks, relying on human error rather than technical exploits.");
        knowledgeBase.Add("updates", "Regularly updating your software, operating system, and antivirus programs is vital. Updates often include security patches that fix vulnerabilities attackers could exploit.");
        knowledgeBase.Add("backup", "Regularly backing up your important data to an external drive or cloud service can protect you from data loss due to cyberattacks, hardware failure, or accidents.");
        knowledgeBase.Add("wi-fi security", "Always use strong, unique passwords for your Wi-Fi network. Enable WPA2 or WPA3 encryption. Be cautious when using public Wi-Fi, as it's often insecure.");
        knowledgeBase.Add("cookies", "Cookies are small pieces of data stored on your computer by websites you visit. While some are harmless and improve browsing, third-party cookies can track your online activity for advertising purposes.");
        knowledgeBase.Add("gdpr", "The General Data Protection Regulation (GDPR) is a comprehensive data privacy law in the European Union that sets guidelines for the collection and processing of personal information from individuals within the EU.");
        knowledgeBase.Add("breach", "A data breach is a security incident where sensitive, protected, or confidential data is copied, transmitted, viewed, stolen, or used by an individual unauthorized to do so.");
        knowledgeBase.Add("dark web", "The Dark Web is a part of the internet that is not indexed by search engines and requires specific software, configurations, or authorizations to access. It's often associated with illicit activities but also used for privacy by journalists and activists.");
        knowledgeBase.Add("zero-day", "A zero-day vulnerability is a software flaw that is unknown to the vendor (and therefore unpatched) when it is exploited by attackers. A 'zero-day exploit' is the method attackers use to target the vulnerability.");
        knowledgeBase.Add("whaling", "Whaling is a specific type of phishing attack that targets high-profile individuals, such as CEOs or senior executives, to gain access to sensitive company information or large sums of money.");
        knowledgeBase.Add("smishing", "Smishing is a form of phishing that uses text messages to trick victims into giving up personal information or clicking malicious links.");
        knowledgeBase.Add("vishing", "Vishing is a form of phishing that uses voice calls (VoIP) to trick victims into giving up personal information or performing actions.");
        knowledgeBase.Add("incident response", "Incident response is an organized approach to addressing and managing the aftermath of a security breach or cyberattack. The goal is to handle the situation in a way that limits damage and reduces recovery time and cost.");
        knowledgeBase.Add("privacy policy", "A privacy policy is a legal document that discloses some or all of the ways a party gathers, uses, discloses, and manages a customer's data. It fulfills a legal requirement to protect a customer or client's privacy.");
        knowledgeBase.Add("cyber hygiene", "Cyber hygiene refers to the practices and steps that users of computers and other devices can take to improve their online security and maintain system health. Examples include regularly changing passwords, updating software, and using antivirus.");
        knowledgeBase.Add("cryptocurrency", "Cryptocurrency is a digital or virtual currency that is secured by cryptography, making it nearly impossible to counterfeit or double-spend. Many cryptocurrencies are decentralized networks based on blockchain technology.");
        knowledgeBase.Add("blockchain", "Blockchain is a distributed ledger technology that enables secure, transparent, and tamper-proof record-keeping. It's the underlying technology for cryptocurrencies like Bitcoin, but has applications beyond finance.");

        knowledgeBase.Add("hi", "Hi! What cyber-related topic would you like to discuss?");
        knowledgeBase.Add("how are you", "I'm an AI, so I don't have feelings, but I'm ready to help you with cybersecurity! How can I assist?");
        knowledgeBase.Add("what is your name", "I am your Cyber Awareness Assistant, designed to help you navigate the digital world safely.");
        knowledgeBase.Add("thank you", "You're welcome! Stay safe out there.");
        knowledgeBase.Add("thanks", "Anytime! Knowledge is your best defense.");
        knowledgeBase.Add("goodbye", "Farewell! Protect your digital life.");
    }

    private void LoadQuizQuestions()
    {
        // True/False Questions
        quizQuestions.Add(new QuizQuestion("It's safe to click on any link sent to you by email.", "TF", "False", "Clicking on suspicious links can lead to malware infections or phishing scams. Always verify the sender.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Using the same password for all your accounts is a good security practice.", "TF", "False", "This is a major security risk. If one account is compromised, all others are vulnerable. Use unique, strong passwords or a password manager.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Public Wi-Fi networks are generally very secure for sensitive transactions.", "TF", "False", "Public Wi-Fi can be insecure and susceptible to eavesdropping. Avoid conducting sensitive transactions on them.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Two-factor authentication (2FA) adds an extra layer of security.", "TF", "True", "2FA requires a second verification step, significantly enhancing security.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Antivirus software provides 100% protection against all malware.", "TF", "False", "While essential, antivirus software is not foolproof. New threats emerge constantly, requiring multiple layers of defense.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Regularly backing up your data protects against ransomware.", "TF", "True", "If your data is encrypted by ransomware, a recent backup allows you to restore your files without paying the ransom.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Incognito mode in a browser makes you completely anonymous online.", "TF", "False", "Incognito mode prevents your browser from saving your browsing history, cookies, and site data. However, your IP address is still visible to websites and your internet service provider can still track your activity.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("A strong password should be easy for you to remember but hard for others to guess.", "TF", "True", "This is the ideal balance. Using passphrases or memorable but complex combinations is key.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Sharing personal information like your birthdate on social media is always safe.", "TF", "False", "Such information can be used by cybercriminals for identity theft or to answer security questions.", options: new List<string> { "True", "False" }));
        quizQuestions.Add(new QuizQuestion("Phishing attacks only happen via email.", "TF", "False", "Phishing can occur through text messages (smishing), voice calls (vishing), and even social media.", options: new List<string> { "True", "False" }));

        // Multiple Choice Questions
        quizQuestions.Add(new QuizQuestion(
            "What is the primary purpose of a firewall?", "MCQ", "C",
            "A firewall acts as a security barrier, monitoring and controlling network traffic to prevent unauthorized access.",
            new List<string> {
                    "A. To speed up your internet connection",
                    "B. To store your files securely",
                    "C. To block unauthorized access to your network",
                    "D. To send spam emails"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "Which of the following is an example of a strong password?", "MCQ", "D",
            "A strong password combines uppercase, lowercase, numbers, and symbols, and is not easily guessable.",
            new List<string> {
                    "A. password123",
                    "B. YourName123",
                    "C. 12345678",
                    "D. P@$$w0rdS3curE!"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "What should you do if you receive a suspicious email asking for your login credentials?", "MCQ", "A",
            "Never click links or provide information in suspicious emails. It's best to delete them or report them.",
            new List<string> {
                    "A. Delete it and block the sender",
                    "B. Click on the link to see where it leads",
                    "C. Reply asking for more information",
                    "D. Forward it to all your contacts"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "What does 'malware' stand for?", "MCQ", "B",
            "Malware is a broad term for any software designed to harm or exploit a computer system.",
            new List<string> {
                    "A. Maximum Learning Software",
                    "B. Malicious Software",
                    "C. Modern Application Ware",
                    "D. Managed Logic Ware"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "What is phishing?", "MCQ", "B",
            "Phishing is a social engineering technique where attackers try to trick users into revealing sensitive information.",
            new List<string> {
                    "A. A type of computer virus",
                    "B. A scam to trick you into revealing personal information",
                    "C. A method to backup your data",
                    "D. A way to encrypt your files"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "Why is it important to regularly update your software and operating system?", "MCQ", "C",
            "Updates often include crucial security patches that protect against newly discovered vulnerabilities.",
            new List<string> {
                    "A. To make your computer run faster",
                    "B. To get new features only",
                    "C. To patch security vulnerabilities and fix bugs",
                    "D. To consume more data storage"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "Which of these is NOT a good practice for protecting your privacy online?", "MCQ", "A",
            "Sharing too much personal information can make you a target for cybercriminals.",
            new List<string> {
                    "A. Sharing your full birthdate and home address on social media",
                    "B. Using a VPN when on public Wi-Fi",
                    "C. Reviewing privacy settings on your accounts",
                    "D. Using strong, unique passwords for each service"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
            "What is ransomware?", "MCQ", "C",
            "Ransomware encrypts your files and demands payment, often in cryptocurrency, to restore access.",
            new List<string> {
                    "A. Software that cleans your computer",
                    "B. A type of online game",
                    "C. Malware that encrypts your files and demands payment",
                    "D. A tool for secure communication"
            }
        ));
        quizQuestions.Add(new QuizQuestion(
           "What does URL stand for?", "MCQ", "B",
           "URL is the address of a given resource on the web, often a website.",
           new List<string> {
                    "A. Universal Remote Locator",
                    "B. Uniform Resource Locator",
                    "C. Unified Request Link",
                    "D. User Resource Log"
           }
       ));
        quizQuestions.Add(new QuizQuestion(
            "Which term refers to manipulating people into performing actions or divulging confidential information?", "MCQ", "A",
            "Social engineering exploits human psychology, not technical vulnerabilities, to achieve its goals.",
            new List<string> {
                    "A. Social Engineering",
                    "B. Digital Forensics",
                    "C. Ethical Hacking",
                    "D. Penetration Testing"
            }
        ));
    }

    public string SetUserName(string name)
    {
        userName = name;
        LogActivity("User Name Set", $"User's name set to: {userName}");
        return $"Nice to meet you, {userName}!";
    }

    public string GetChatbotResponse(string userInput)
    {
        LogActivity("User Input", userInput);
        conversationHistory.Add($"User: {userInput}");

        string lowerInput = userInput.ToLower().Trim();

        // Check knowledge base
        foreach (var entry in knowledgeBase)
        {
            if (lowerInput.Contains(entry.Key))
            {
                LogActivity("Knowledge Base Lookup", $"Found answer for: {entry.Key}");
                return entry.Value;
            }
        }

        // General responses or default
        if (lowerInput.Contains("hello") || lowerInput.Contains("hi"))
        {
            return knowledgeBase["hello"];
        }
        if (lowerInput.Contains("how are you"))
        {
            return knowledgeBase["how are you"];
        }
        if (lowerInput.Contains("your name"))
        {
            return knowledgeBase["what is your name"];
        }
        if (lowerInput.Contains("thank you") || lowerInput.Contains("thanks"))
        {
            return knowledgeBase["thank you"];
        }
        if (lowerInput.Contains("what can you do"))
        {
            return knowledgeBase["what can you do"];
        }
        if (lowerInput.Contains("exit") || lowerInput.Contains("bye") || lowerInput.Contains("goodbye"))
        {
            return "Goodbye! Remember to stay cyber aware.";
        }

        LogActivity("Unrecognized Input", $"Could not find response for: {userInput}");
        return "I'm not sure how to respond to that. Could you please rephrase or ask about a specific cybersecurity topic? You can also type 'menu'.";
    }

    public string DisplayMainMenuText()
    {
        LogActivity("Main Menu Displayed", "User requested main menu.");
        return "\n--- Main Menu ---\n" +
               "1. Ask about a topic (e.g., 'What is phishing?')\n" +
               "2. Start Quiz\n" +
               "3. Add a Task\n" +
               "4. Show My Tasks\n" +
               "5. Complete Task\n" +
               "6. Set a Reminder\n" +
               "7. Show Activity Log\n" +
               "8. Exit\n" +
               "What would you like to do?";
    }

    public void LogActivity(string action, string details)
    {
        activityLog.Add(new ActivityLogEntry(action, details));
    }

    public string DisplayActivityLog()
    {
        if (!activityLog.Any())
        {
            return "No activities logged yet.";
        }

        StringBuilder log = new StringBuilder("\n--- Activity Log (Last 10) ---\n");
        // Show last 10, newest first
        var recentEntries = activityLog.Skip(Math.Max(0, activityLog.Count - 10)).Reverse();
        foreach (var entry in recentEntries)
        {
            log.AppendLine($"[{entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}] {entry.Action}: {entry.Details}");
        }
        LogActivity("Activity Log Displayed", "User viewed activity log.");
        return log.ToString();
    }

    public string DisplayFullActivityLog()
    {
        if (!activityLog.Any())
        {
            return "No activities logged yet.";
        }

        StringBuilder log = new StringBuilder("\n--- Full Activity Log ---\n");
        var reversedEntries = activityLog.AsEnumerable().Reverse(); // Show newest first
        foreach (var entry in reversedEntries)
        {
            log.AppendLine($"[{entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}] {entry.Action}: {entry.Details}");
        }
        LogActivity("Full Activity Log Displayed", "User viewed full activity log.");
        return log.ToString();
    }

    public string AddTask(string description, DateTime? dueDate)
    {
        CyberTask newTask = new CyberTask(description, dueDate);
        userTasks.Add(newTask);
        LogActivity("Task Added", $"Task: '{description}' {(dueDate.HasValue ? "due " + dueDate.Value.ToShortDateString() : "no due date")}");
        return $"Task '{description}' has been added to your list{(dueDate.HasValue ? " with due date: " + dueDate.Value.ToShortDateString() : "")}.";
    }

    public string DisplayTasks()
    {
        if (!userTasks.Any())
        {
            return "You currently have no tasks.";
        }

        StringBuilder tasksList = new StringBuilder("\n--- Your Cybersecurity Tasks ---\n");
        for (int i = 0; i < userTasks.Count; i++)
        {
            CyberTask task = userTasks[i];
            string status = task.IsCompleted ? "✅ Completed" : "⏳ Pending";
            string dueDate = task.DueDate.HasValue ? $" (Due: {task.DueDate.Value.ToShortDateString()})" : "";
            tasksList.AppendLine($"{i + 1}. {task.Description}{dueDate} - {status}");
        }
        LogActivity("Tasks Displayed", "User viewed their tasks.");
        return tasksList.ToString();
    }

    public string CompleteTask(int taskNumber)
    {
        if (taskNumber <= 0 || taskNumber > userTasks.Count)
        {
            return "Invalid task number. Please provide a number from the list.";
        }

        CyberTask taskToComplete = userTasks[taskNumber - 1];
        if (taskToComplete.IsCompleted)
        {
            return $"Task '{taskToComplete.Description}' is already marked as completed.";
        }

        taskToComplete.IsCompleted = true;
        LogActivity("Task Completed", $"Task '{taskToComplete.Description}' marked as completed.");
        return $"Task '{taskToComplete.Description}' marked as completed! Well done!";
    }

    public string SetReminder(string subject, string dateTimeInput)
    {
        DateTime reminderTime;
        // More robust date/time parsing
        if (DateTime.TryParse(dateTimeInput, out reminderTime))
        {
            // General parsing success
        }
        else if (DateTime.TryParseExact(dateTimeInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out reminderTime))
        {
            // Specific yyyy-MM-DD HH:mm
        }
        else if (DateTime.TryParseExact(dateTimeInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out reminderTime))
        {
            // Specific yyyy-MM-DD (set to start of day)
        }
        else if (dateTimeInput.ToLower() == "tomorrow")
        {
            reminderTime = DateTime.Now.AddDays(1).Date.AddHours(9); // Default to 9 AM tomorrow
        }
        else if (dateTimeInput.ToLower() == "next week")
        {
            reminderTime = DateTime.Now.AddDays(7).Date.AddHours(9); // Default to 9 AM next week
        }
        else if (dateTimeInput.ToLower().Contains("in ") && dateTimeInput.ToLower().Contains("minutes"))
        {
            if (int.TryParse(dateTimeInput.ToLower().Replace("in ", "").Replace("minutes", "").Trim(), out int minutes))
            {
                reminderTime = DateTime.Now.AddMinutes(minutes);
            }
            else
            {
                LogActivity("Reminder Failed", $"Could not parse minutes for reminder: '{dateTimeInput}'");
                return "I couldn't understand the time for the reminder. Please be more specific (e.g., '2024-12-31 14:30', 'tomorrow', 'in 30 minutes').";
            }
        }
        // Add more flexible parsing logic here if needed (e.g., "next Monday at 3 PM")
        else
        {
            LogActivity("Reminder Failed", $"Could not parse reminder date/time: '{dateTimeInput}'");
            return "I couldn't understand the time for the reminder. Please be more specific (e.g., '2024-12-31 14:30', 'tomorrow', 'next week').";
        }

        if (reminderTime <= DateTime.Now)
        {
            LogActivity("Reminder Failed", "Reminder time is in the past.");
            return "The reminder time must be in the future. Please try again.";
        }

        // In a real app, you'd schedule a system notification or a background task here.
        // For this chatbot, we'll just log it.
        LogActivity("Reminder Set", $"Reminder for '{subject}' set for {reminderTime.ToString("yyyy-MM-dd HH:mm")}.");
        return $"Okay, I'll remind you about '{subject}' on {reminderTime.ToString("yyyy-MM-dd HH:mm")}.";
    }
}
