using fitnes.bot;
using fitnes.bot.Extension.DI;
using fitnes.Domain.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

HostApplicationBuilder builder = new HostApplicationBuilder(args);

builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(builder.Configuration[$"{TelegramBotOptions.SectionName}:Token"] ?? throw new ArgumentNullException()));
builder.Services.AddOptions(builder.Configuration);
builder.Services.AddHostedService<BotWorker>();
builder.Services.AddServices();
builder.Services.AddBotHandlers();
builder.Services.AddPipeline();
builder.Services.AddLocalization();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();
try
{
    app.Run();
}catch(Exception ex)
{
    Console.WriteLine("app exception");
}
