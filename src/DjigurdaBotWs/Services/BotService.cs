using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DjigurdaBotWs.Services
{
    public interface IBotService
    {
        void Start(string token);
        void Stop();

        void SayTest();
        void SayMorningGreeting();
        void SayBirthdayToday();
    }

    public class BotService : IBotService
    {
        private const long TechTalksChatId = -1001032644594;
        private const int FoodChatId = -86354973;

        private readonly IToastService _toastService;
        private readonly IWaterService _waterService;
        private readonly IQuoteService _quoteService;
        private readonly IObsceneService _obsceneService;
        private readonly IBirthdayService _birthdayService;
        private readonly ILogger _messageLogger;
        private readonly ILogger _exceptionLogger;
        private readonly Random _random;

        private TelegramBotClient _bot;

        public BotService(
            IToastService toastService,
            IWaterService waterService,
            IQuoteService quoteService,
            IObsceneService obsceneService,
            IBirthdayService birthdayService
            )
        {
            _toastService = toastService;
            _messageLogger = LogManager.GetLogger("message");
            _exceptionLogger = LogManager.GetCurrentClassLogger();
            _waterService = waterService;
            _quoteService = quoteService;
            _obsceneService = obsceneService;
            _birthdayService = birthdayService;

            _random = new Random();
        }

        public void Start(string token)
        {
            _bot = new TelegramBotClient(token);

            _bot.OnMessage += BotOnMessageReceived;

            _bot.StartReceiving();
        }

        public void Stop()
        {
            _bot.StopReceiving();
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;
                if (message == null || message.Type != MessageType.TextMessage) return;

                _messageLogger.Info($"{message.Chat.Id};{message.Chat.Title};{message.From.Id};{message.From.Username};{message.Text}");

                await ProcessMessage(message);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                _exceptionLogger.Fatal(e, "Fatal exception in BotOnMessageReceived()");
            }
        }

        private async Task ProcessMessage(Message message)
        {
            var messageTextLowered = message.Text.ToLowerInvariant();

            if (messageTextLowered.Contains("тост"))
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);

                await _bot.SendTextMessageAsync(message.Chat.Id, _toastService.GetRandomToast());
            }
            else if (messageTextLowered.Contains("вода сколько"))
            {
                var bottlesCount = _waterService.GetBottlesCount();
                await
                    _bot.SendTextMessageAsync(message.Chat.Id,
                        $"Мой повелитель, на складах осталось {bottlesCount} бутылей воды.");
            }
            else if (messageTextLowered.Contains("вода потратить"))
            {
                var bottlesCount = _waterService.UseBottle();
                await
                    _bot.SendTextMessageAsync(message.Chat.Id,
                        $"Одна бутыль использована. На складах осталось {bottlesCount} бутылей воды.");
            }
            else if (messageTextLowered.Contains("вода восстановить"))
            {
                var messageText = messageTextLowered;
                var messageBottlesCount =
                    messageText.Substring(messageText.IndexOf("вода восстановить", StringComparison.Ordinal) +
                                          "вода восстановить".Length + 1).Trim();
                var bottlesCount = Convert.ToInt32(messageBottlesCount);

                if (bottlesCount >= 0)
                {
                    _waterService.SetBottlesCount(bottlesCount);
                }

                await _bot.SendTextMessageAsync(message.Chat.Id,
                    $"Мой повелитель, добрые вести! Запасы пополнены. На складах находится {bottlesCount} бутылей воды.");
            }
            else if (messageTextLowered.Contains("артемий"))
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, "Артемий божественен!");
            }
            else if (messageTextLowered.Contains("джигурда"))
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, "Пора накатить!");
            }
            else if (messageTextLowered.Contains("доброе утро"))
            {
                await  _bot.SendTextMessageAsync(message.Chat.Id, $"И тебе наидобрейшего утра, {message.From.FirstName}!");
            } 
            else if (message.Text.StartsWith("/writeFood"))
            {
                await _bot.SendTextMessageAsync(FoodChatId, message.Text.Replace("/writeFood", "").Trim());
            }
            /*   else if (message.Text.StartsWith("/writeTechTalks"))
              {
                  await _bot.SendTextMessageAsync(TechTalksChatId, message.Text.Replace("/writeTechTalks", "").Trim());
              }
               else if (messageTextLowered.Contains("квоты"))
               {
                   await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                   await Task.Delay(2000);

                   await _bot.SendTextMessageAsync(message.Chat.Id, "Квоты ...");

                   await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                   await Task.Delay(1000);

                   await _bot.SendTextMessageAsync(message.Chat.Id, "про ...");

                   await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                   await Task.Delay(1000);

                   await _bot.SendTextMessageAsync(message.Chat.Id, "кэ ...");

                   await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                   await Task.Delay(1000);

                   await _bot.SendTextMessageAsync(message.Chat.Id, "шированы!!");
               }
               else if (message.From.Username.Contains("askmeforproject"))
               {
                   var random = new Random();
                   if (random.Next(30) == 0)
                   {
                       await _bot.SendTextMessageAsync(message.Chat.Id, "Я ХОЧУ ЕСТЬ ...");

                       await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                       await Task.Delay(1000);

                       await _bot.SendTextMessageAsync(message.Chat.Id, "м ...");

                       await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                       await Task.Delay(1000);

                       await _bot.SendTextMessageAsync(message.Chat.Id, "ы ...");

                       await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                       await Task.Delay(1000);

                       await _bot.SendTextMessageAsync(message.Chat.Id, "л ...");

                       await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                       await Task.Delay(1000);

                       await _bot.SendTextMessageAsync(message.Chat.Id, "о ...");
                   }
               } */
            else if (messageTextLowered.Contains("спеть про стаканы"))
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Ну-ка мечи стаканы на стол,");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Ну-ка мечи стаканы на стол");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Ну-ка мечи стаканы на стол");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "И прочую посуду.");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Все говорят, что пить нельзя,");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Все говорят, что пить нельзя");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Все говорят, что пить нельзя");

                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, "А я говорю, что буду!");
            }
            else if (messageTextLowered.Contains("лепс спеть"))
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, "Щас спою ......");
                await
                    _bot.SendAudioAsync(message.Chat.Id, "http://tiburon-research.ru/files/leps.mp3", 4 * 60 + 23,
                        "Григорий Лепс",
                        "Рюмка водки на столе");
            }
            else if (messageTextLowered.Contains("котик прислать") || _random.Next(250) == 0)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, "Кстати, зацени, какую фоточку котика я нашел!", replyToMessageId: message.MessageId);
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
                using (var memoryStream = new MemoryStream(new WebClient().DownloadData("http://thecatapi.com/api/images/get?format=src")))
                {
                    var filename = $"{Guid.NewGuid().ToString().ToLower()}.gif";
                    var fileToSend = new FileToSend { Filename = filename, Content = memoryStream };
                    await _bot.SendPhotoAsync(message.Chat.Id, fileToSend);
                }
            }

            if (_obsceneService.ContainsBadWords(messageTextLowered) && _random.Next(2) == 0)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, "Извольте не материться! Здесь же дамы!!");
            }

        }

        public void SayMorningGreeting()
        {
            var task = Task.Run(async () =>
            {
                await _bot.SendTextMessageAsync(FoodChatId, "Всех с добрым утром!");
                await _bot.SendTextMessageAsync(FoodChatId, "Наступает время унылых, неинтересных и демотивирующих цитат!");

                await _bot.SendChatActionAsync(FoodChatId, ChatAction.Typing);
                await Task.Delay(2000);

                var quote = _quoteService.GetRandomQuote();

                await _bot.SendTextMessageAsync(FoodChatId, quote.Text + "\n" + quote.Author);
            });
            task.Wait();
        }

        public void SayBirthdayToday()
        {
            var task = Task.Run(async () =>
            {
                var birthdayBoys = await _birthdayService.GetBirthdayBoysForTodayAsync();
                foreach (var birthdayBoy in birthdayBoys)
                {
                    await _bot.SendTextMessageAsync(FoodChatId, $"Урааа!!! Сегодня день рождения у {birthdayBoy}!! Сердечно поздравляю!");
                }
            });
            task.Wait();
        }

        public void SayTest()
        {
            var task = Task.Run(async () =>
            {
                await _bot.SendTextMessageAsync(TechTalksChatId, "Тест");
            });
            task.Wait();
        }
    }
}
