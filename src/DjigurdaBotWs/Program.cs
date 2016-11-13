using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DjigurdaBotWs.Repositories;
using DjigurdaBotWs.Services;
using NLog;
using SimpleInjector;
using Topshelf;

namespace DjigurdaBotWs
{

    class Program
    {
        static string TelegramApiKey { get; set; }

        /// <summary>
        /// Entry point
        /// </summary>
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Program>(s =>
                {
                    s.ConstructUsing(name => new Program());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());

                });
                x.RunAsLocalSystem();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                });

                // getting telegram bot api key
                x.AddCommandLineDefinition("telegramapikey", val => TelegramApiKey = val);
                if (string.IsNullOrEmpty(TelegramApiKey))
                {
                    TelegramApiKey = ConfigurationManager.AppSettings["TelegramApiKey"];
                }

                x.SetDescription("Greatest spammer in Tiburon Research");
                x.SetDisplayName("Djigurda Bot");
                x.SetServiceName("Djigurda Bot");
            });
        }

        private readonly ILogger _logger;
        private readonly IBotService _bot;

        public Program()
        {
            var serviceProvider = ConfigureServices();

            _logger = serviceProvider.GetInstance<ILogger>();
            _bot = serviceProvider.GetInstance<IBotService>();
        }

        /// <summary>
        /// On Windows Service start
        /// </summary>
        public void Start()
        {
            try
            {
                _logger.Info("Starting");

                if (string.IsNullOrEmpty(TelegramApiKey))
                {
                    _logger.Info("TelegramApiKey is not defined");
                    return;
                }

                _bot.Start(TelegramApiKey);
                _logger.Info("Bot is listening. ApiKey {0}", TelegramApiKey);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                _logger.Fatal(e, "Fatal exception in Start()");
            }
        }

        /// <summary>
        /// On Windows Service stop
        /// </summary>
        public void Stop()
        {
            try
            {
                _bot.Stop();
                _logger.Info("Bot is stopped");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                _logger.Fatal(e, "Fatal exception in Stop()");
            }
        }

        /// <summary>
        /// DI container configuration
        /// </summary>
        /// <returns>Simple injector configured container instance</returns>
        private static Container ConfigureServices()
        {
            var container = new Container();

            container.Register<ILogger>(LogManager.GetCurrentClassLogger, Lifestyle.Singleton);
            container.Register<IBotService, BotService>(Lifestyle.Singleton);

            container.Register<IWaterRepository, WaterFileRepository>(Lifestyle.Singleton);
            container.Register<IToastService, ToastService>(Lifestyle.Singleton);
            container.Register<IWaterService, WaterService>(Lifestyle.Singleton);
            container.Register<IQuoteService, QuoteService>(Lifestyle.Singleton);

            container.Verify();
            return container;
        }
    }
}
