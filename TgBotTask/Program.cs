using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotTask;
using TgBotTask.Data;
using TgBotTask.Handelers;
using TgBotTask.Models.DtoModels;
using TgBotTask.Services;
using static System.Net.Mime.MediaTypeNames;



public class Program
{
    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        IServiceProvider _serviceProvider;


        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
            new TelegramBotClient("8319989705:AAHgfb_V9nkEkQBR_6uBcElj6xmhLcQ8uIc")
            );



        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;"));

        builder.Services.AddTransient<Buttons>();


        builder.Services.AddScoped<TaskCrud>();
        builder.Services.AddScoped<UserCrud>();
        builder.Services.AddScoped<ShowTask>();

        builder.Services.AddSingleton<UpdateHandler>();
        builder.Services.AddSingleton<UpdateHandler>();


        builder.Services.AddHostedService<MessegeService>();
            
        IHost host = builder.Build();
        _serviceProvider = host.Services; 

        var botClient = new TelegramBotClient("8319989705:AAHgfb_V9nkEkQBR_6uBcElj6xmhLcQ8uIc");

        var handler = _serviceProvider.GetRequiredService<UpdateHandler>();
       

        Console.WriteLine("Бот запущен...");
        botClient.StartReceiving(handler.HandleUpdateAsync, handler.HandleErrorAsync);


        Console.ReadLine();
    }  
}