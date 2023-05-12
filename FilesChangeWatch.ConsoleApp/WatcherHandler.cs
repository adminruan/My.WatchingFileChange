using System.Collections;
using System.Collections.Concurrent;

namespace FilesChangeWatch.ConsoleApp;

/// <summary>
/// 监听处理
/// </summary>
public static class WatcherHandler
{
    /// <summary>
    /// 已复制文件信息
    /// </summary>
    /// <remarks>用于记录已复制文件信息，防止文件多次复制</remarks>
    static readonly ConcurrentDictionary<string, DateTime> _pairs = new();

    static WatcherHandler()
    {
        Thread thread = new(() =>
        {
            while (true)
            {
                var invalidDatas = _pairs.Where(p => p.Value < DateTime.Now).ToList();
                if (invalidDatas.Any())
                {
                    foreach (var item in invalidDatas)
                    {
                        _pairs.TryRemove(item.Key, out _);
                    }
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    /// <summary>
    /// 文件创建事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal static void WatcherCreated(object sender, FileSystemEventArgs e)
    {
        IList<string?> settings = GetCopyToDirectorys();
        Parallel.ForEach(settings, x =>
        {
            if (!string.IsNullOrWhiteSpace(x))
                FileCopyToPath(e.FullPath, x);
        });
    }

    /// <summary>
    /// 文件变更事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal static void WatcherChanged(object sender, FileSystemEventArgs e)
    {
        IList<string?> settings = GetCopyToDirectorys();
        Parallel.ForEach(settings, x =>
        {
            if (!string.IsNullOrWhiteSpace(x))
                FileCopyToPath(e.FullPath, x);
        });
    }




    #region 文件复制

    static IList<string?> GetCopyToDirectorys()
    {
        return Configuration.GetCopyToPathString("watchedCopyToDires");
    }

    static void FileCopyToPath(string filePath, string copyToPath)
    {
        try
        {
            int index = filePath.LastIndexOf("\\");
            string fileName = filePath[index..];
           
            string key = $"{fileName}:{copyToPath}";
            if (_pairs.ContainsKey(key))
            {
                return;
            }
            else
            {
                _pairs.TryAdd(key, DateTime.Now.AddSeconds(5));
            }

            if (!Directory.Exists(copyToPath))
            {
                Directory.CreateDirectory(copyToPath);
            }
            copyToPath = $"{copyToPath}{fileName}";
            if (File.Exists(copyToPath))
            {
                File.Delete(copyToPath);
            }

            Console.WriteLine(copyToPath);
            File.Copy(filePath, copyToPath);
        }
        catch (Exception ex)
        {

        }
    }

    #endregion
}
