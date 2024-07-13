
using Telegram.Bot;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Text.Json;
using Telegram.Bots.Http;
using Telegram.Bot.Types.Enums;
using Telegram.Bots.Types.Inline;
using Telegram.Bots.Types;
using System.Net.Sockets;
using Aikso_bot;

class Program
{
    static ITelegramBotClient bot = new TelegramBotClient("");

    static async Task Main(string[] args)
    {
        var rabbitMqSubscriber = new RabbitMQSubscriber();
        var telegramBotService = new TelegramBotService("");

        rabbitMqSubscriber.MessageReceived += async (sender, message) =>
        {
            Console.WriteLine("Received message: " + message);
            await MessageHandler.HandleMessageAsync(message, telegramBotService);
        };

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<Telegram.Bot.Types.Enums.UpdateType>() // получать все типы обновлений
        };

        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        var listeningTask = Task.Run(() => rabbitMqSubscriber.StartListening());

        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();

        // Ensure the listening task is not aborted prematurely
        await listeningTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message!.Type == MessageType.Text)
        {
            var messageText = update.Message.Text;
            var chatId = update.Message.Chat.Id;

            if (messageText.StartsWith("/start"))
            {
                var uniqueCode = messageText.Split(' ')[1];
                await UpdateChatIdAsync(uniqueCode, chatId);
            }
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.ToString());
    }

    private static async Task UpdateChatIdAsync(string uniqueCode, long chatId)
    {
        using (var httpClient = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("uniqueCode", uniqueCode),
                new KeyValuePair<string, string>("chatId", chatId.ToString())
            });

            var requestContentString = await content.ReadAsStringAsync();
            Console.WriteLine($"Request content: {requestContentString}"); // Вывод содержимого запроса в консоль

            var response = await httpClient.PostAsync("", content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to update chat ID.");
            }
            else
            {
                Console.WriteLine("Successfully updated chat ID.");
            }
        }
    }
}













