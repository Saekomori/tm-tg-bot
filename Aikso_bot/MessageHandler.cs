using System.Text.Json;
using Telegram.Bot.Types;

namespace Aikso_bot
{
    public static class MessageHandler
    {
        public static async Task HandleMessageAsync(string message, TelegramBotService telegramBotService)
        {
            var parsedMessage = ParseMessage(message);
            if (parsedMessage != null)
            {
                long? chatId = null;

                
                if (!string.IsNullOrEmpty(parsedMessage.UserEmail))
                {
                    chatId = await GetChatIdAsync(parsedMessage.UserEmail);
                }
                else if (!string.IsNullOrEmpty(parsedMessage.Executor))
                {
                    chatId = await GetChatIdByExecutorAsync(parsedMessage.Executor);
                }

                if (chatId.HasValue)
                {
                    Console.WriteLine($"Sending message to chat ID: {chatId.Value}");
                    await telegramBotService.SendMessageAsync(chatId.Value, FormatMessage(parsedMessage));
                }
                else
                {
                    Console.WriteLine("Chat ID not found for user or executor");
                }
            }
            else
            {
                Console.WriteLine("Failed to parse message or no user/email or executor found.");
            }
        }

        private static async Task<long?> GetChatIdAsync(string userEmail)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    if (jsonDocument.RootElement.TryGetProperty("chatId", out var chatIdElement))
                    {
                        var chatId = chatIdElement.GetInt64();
                        Console.WriteLine($"Chat ID for user {userEmail}: {chatId}");
                        return chatId;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to get chat ID for user: {userEmail}. Status code: {response.StatusCode}");
                }
            }
            return null;
        }

        private static async Task<long?> GetChatIdByExecutorAsync(string executor)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    if (jsonDocument.RootElement.TryGetProperty("chatId", out var chatIdElement))
                    {
                        var chatId = chatIdElement.GetInt64();
                        Console.WriteLine($"Chat ID for executor {executor}: {chatId}");
                        return chatId;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to get chat ID for executor: {executor}. Status code: {response.StatusCode}");
                }
            }
            return null;
        }

        private static ParsedMessage ParseMessage(string message)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(message);
                var root = jsonDocument.RootElement;

                if (root.TryGetProperty("userEmail", out var userEmailElement))
                {
                    string userEmail = userEmailElement.GetString();
                    string title = root.GetProperty("title").GetString();
                    string description = root.GetProperty("description").GetString();
                    DateTime dateTime = GetDateTime(root.GetProperty("dateTime"));
                    DateTime dateTimeNotification = GetDateTime(root.GetProperty("dateTimeNotification"));

                    Console.WriteLine($"Parsed message for userEmail: {userEmail}, title: {title}, description: {description}, dateTime: {dateTime}, dateTimeNotification: {dateTimeNotification}");

                    return new ParsedMessage
                    {
                        UserEmail = userEmail,
                        Title = title,
                        Description = description,
                        DateTime = dateTime,
                        DateTimeNotification = dateTimeNotification,
                    };
                }
                else if (root.TryGetProperty("executor", out var executorElement))
                {
                    string executor = executorElement.GetString();
                    string title = root.GetProperty("title").GetString();
                    string description = root.GetProperty("description").GetString();
                    DateTime dateTime = GetDateTime(root.GetProperty("dateTime"));
                    DateTime dateTimeNotification = GetDateTime(root.GetProperty("dateTimeNotification"));

                    Console.WriteLine($"Parsed message for executor: {executor}, title: {title}, description: {description}, dateTime: {dateTime}, dateTimeNotification: {dateTimeNotification}");

                    return new ParsedMessage
                    {
                        Executor = executor,
                        Title = title,
                        Description = description,
                        DateTime = dateTime,
                        DateTimeNotification = dateTimeNotification,
                    };
                }
                else
                {
                    Console.WriteLine("No userEmail or executor found in the message.");
                    return null;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse message: {ex.Message}");
                return null;
            }
        }

        private static string FormatMessage(ParsedMessage message)
        {
            return $"Напоминание о задаче \"{message.Title}\", крайний срок выполнения {message.DateTime}";
        }

        private static DateTime GetDateTime(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("$date", out var dateElement))
            {
                return dateElement.GetDateTime();
            }
            throw new InvalidOperationException("Invalid date format");
        }
    }

 

   
}