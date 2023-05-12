// See https://aka.ms/new-console-template for more information
using FilesChangeWatch.ConsoleApp;

string? watchDirectory = Configuration.GetWatcherSettingString("watchDirectory");
if (string.IsNullOrWhiteSpace(watchDirectory))
{
    throw new Exception("请设置监听文件目录");
}

FileSystemWatcher watcher = new();
watcher.Path = watchDirectory;
watcher.NotifyFilter = NotifyFilters.LastWrite;
watcher.Filter = Configuration.GetWatcherSettingString("watchFileType") ?? "*.dll";
watcher.Created += WatcherHandler.WatcherCreated;
watcher.Changed += WatcherHandler.WatcherChanged;
watcher.EnableRaisingEvents = true;

Console.WriteLine($"开始监听目录：{watchDirectory}");

Console.ReadKey();



