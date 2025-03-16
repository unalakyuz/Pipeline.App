using Microsoft.Extensions.DependencyInjection;
using Pipeline.App.Service;

var serviceProvider = new ServiceCollection()
   .AddTransient<ILogParser, LogParser>()
   .BuildServiceProvider();

var logParser = serviceProvider.GetService<ILogParser>();
await logParser.ParseLogAsync("C:\\Logs\\log.txt");
