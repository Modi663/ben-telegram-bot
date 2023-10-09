using System.IO;
using System.Runtime.Serialization;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;


namespace BenTelegramBot
{
    class Bot
    {
        // Объявляем константы
        private static DialogHandler DialogHandler = new DialogHandler();
        private static ITelegramBotClient bot = new TelegramBotClient(File.ReadAllText("../../../TOKEN.txt"));

        private static List<string> pathList = new List<string>
        {
            "../../../resourses/f1.mp4",
            "../../../resourses/f2.ogg",
            "../../../resourses/f4.ogg",
            "../../../resourses/f5.ogg",
            "../../../resourses/f6.ogg",
            "../../../resourses/f7.ogg",
            "../../../resourses/f8.ogg"
        };
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };

            bot.StartReceiving(
                BotMessageHandler,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
                );
            Console.ReadLine();
        }


        static async Task BotMessageHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {   
            // Собирание логов в консоль
            Console.WriteLine($"Update Id: {update.Id}, Username: {update?.Message?.Chat.Username}, Message: {update?.Message?.Text}, Date: {update?.Message?.Date}");
            Random random = new Random();

            if (update?.Type == UpdateType.Message)
            {
                var message = update?.Message;
                switch (message?.Text?.ToLower())
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(message.Chat,"Привет, привет. Я пёс Ден. Не Бен, не надо путать, мне это не нравится.");
                        return;
                    case "/call":
                        await botClient.SendVideoNoteAsync(message.Chat, System.IO.File.OpenRead("../../../resourses/f3.mp4"));
                        DialogHandler.AddDialog(message.Chat.Id, true);
                        return;
                    default:
                    {
                        // Получаем объект
                        var PATH = pathList[random.Next(pathList.Count)];
                        await using var stream = System.IO.File.OpenRead(PATH);
                        if (message != null && DialogHandler.inDialog(message.Chat.Id))
                        {
                            if (PATH == pathList[0]) { DialogHandler?.RemoveDialog(message.Chat.Id); }

                            Console.WriteLine($"Responce: {PATH}\n");
                            switch (Path.GetExtension(PATH))
                            {
                                case ".ogg":
                                    await botClient.SendVoiceAsync(message.Chat, voice: stream);
                                    return;
                                case ".mp4":
                                    await botClient.SendVideoNoteAsync(message.Chat, videoNote: stream);
                                    return;
                            }
                        }
                    } 
                    break;
                }
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
            

    }
}
