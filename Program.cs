using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace adlordy.wallpaper
{
    class Program
    {
        static string folder = "D:/Videos/Wallpapers";
        static string ffmpeg = @"C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg-3.2.4-win64-static\bin\ffmpeg.exe";
        static string video = Path.Combine(folder, "video.mp4");

        static void Main(string[] args)
        {
            // var link = args.Length > 0 ? args[0] : "https://www.youtube.com/watch?v=QPdWJeybMo8";
            // var video = SelectVideo(link);
            // DownloadVideo(video);
            // var line = Console.ReadLine();
            // var second = Int32.Parse(line);
            // System.Console.WriteLine("Select second:");

            Task.Run(async () =>
            {
                var file = Path.Combine(folder,"image-0001.jpg");
                var second = 0;
                while(true){
                    ExtractImage(TimeSpan.FromSeconds(second));
                    second++;
                    Wallpaper.SetDesktopWallpaper(file, WallpaperStyle.Fill);
                    await Task.Delay(1000);
                }
            });
            Console.ReadLine();
        }

        private static void ExtractImage(TimeSpan start)
        {

            var image = Path.Combine(folder, "image-%4d.jpg");
            var arguments = $"-ss {start} -i \"{video}\" -q:v 1 -vframes 1 \"{image}\"";
            
            var processInfo = new ProcessStartInfo(ffmpeg, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            
            var process = Process.Start(processInfo);
            process.WaitForExit();
        }

        private static VideoInfo SelectVideo(string link)
        {
            var videoInfos = DownloadUrlResolver.GetDownloadUrls(link).
                Where(info => info.VideoType == VideoType.Mp4)
                .OrderByDescending(info => info.Resolution).ToArray();

            for (var i = 0; i < videoInfos.Length; i++)
            {
                var info = videoInfos[i];
                System.Console.WriteLine("{0}: {1}", i, info.Resolution);
            }

            System.Console.WriteLine("Select Resolution:");
            var line = Console.ReadLine();
            if (Int32.TryParse(line, out int index))
            {
                return videoInfos[index];
            }
            return videoInfos[0];
        }

        private static void DownloadVideo(VideoInfo videoInfo)
        {
            var videoDownloader = new VideoDownloader(videoInfo, video);
            videoDownloader.DownloadProgressChanged += (s, a) => Console.WriteLine(a.ProgressPercentage);
            videoDownloader.Execute();
        }
    }
}
