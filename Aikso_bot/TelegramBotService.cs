using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Aikso_bot
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient botClient;

        public TelegramBotService(string botToken)
        {
            botClient = new TelegramBotClient(botToken);
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            // Здесь вы можете получить все chatId из базы данных и отправить сообщение каждому
            // Пример:
            // var chatIds = GetChatIdsFromDatabase();
            // foreach (var chatId in chatIds)
            // {
            //     await SendMessageAsync(chatId, message);
            // }
        }
    }
}
