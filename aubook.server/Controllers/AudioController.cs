using System.Diagnostics;
using System.Text;
using System.Text.Json;
using aubook.server.Models;
using Microsoft.AspNetCore.Mvc;

namespace aubook.server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioController : Controller
{
    [HttpGet("stream")]
    public async Task<IActionResult> StreamTranscodedAudio()
    {
        Console.WriteLine("stream");
        // string inputFile = "./Eragon_1_CD1_01.mp3";
        string inputFile = "./lol.mp3";
        string fullPath = Path.GetFullPath(inputFile);

        Console.WriteLine(JsonSerializer.Serialize(AudioData.ParseMetadata(fullPath)));
        // Console.WriteLine(fullPath);

        // var ffprobeArgs = new StringBuilder();
        // ffprobeArgs.Append($"-i {fullPath} ");
        // ffprobeArgs.Append("-show_entries format=duration ");
        // ffprobeArgs.Append("-v quiet ");
        // ffprobeArgs.Append("-of csv=\"p=0\" ");

        // Console.WriteLine(ffprobeArgs.ToString());

        // var ffprobeProcess = new Process
        // {
        //     StartInfo = new ProcessStartInfo
        //     {
        //         FileName = "ffprobe",
        //         Arguments = ffprobeArgs.ToString(),
        //         RedirectStandardOutput = true,
        //         RedirectStandardError = true,
        //         UseShellExecute = false,
        //         CreateNoWindow = true,
        //     }
        // };

        // ffprobeProcess.Start();

        // _ = Task.Run(() =>
        // {
        //     var error = ffprobeProcess.StandardError.ReadToEnd();
        //     Console.Error.WriteLine(error);
        // });

        // using StreamReader sr = new(ffprobeProcess.StandardOutput.BaseStream);
        // string audioFile = sr.ReadToEnd().Replace('.', ',');
        // int duration = (int)Math.Round(decimal.Parse(audioFile));
        // TimeSpan ts = new(0, 0, duration);
        // Console.WriteLine($"{ts.Minutes}:{ts.Seconds}");
        // Console.WriteLine(audioFile);
        // Console.WriteLine(duration);

        var ffmpegProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                // Arguments = $"-i \"{fullPath}\" -vn -map_metadata -1 -ss 3 -f mp3 -b:a 96k -",
                Arguments = $"-i \"{fullPath}\" -vn -map_metadata -1 -f mp3 -b:a 96k -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        bool finished = false;

        ffmpegProcess.EnableRaisingEvents = true;
        ffmpegProcess.Exited += (_, _) =>
        {
            Console.WriteLine("Exited");
            finished = true;
        };

        ffmpegProcess.Start();

        // Optional: Log FFmpeg errors (you can read StandardError in a background task)
        _ = Task.Run(() =>
        {
            var error = ffmpegProcess.StandardError.ReadToEnd();
            Console.Error.WriteLine(error);
        });

        var outputStream = ffmpegProcess.StandardOutput.BaseStream;

        Response.OnCompleted(async () =>
        {
            // Console.WriteLine("Cloesed");
            // ffmpegProcess.Close();
            // // ffmpegProcess.Kill();
            // Console.WriteLine(ffmpegProcess.HasExited);
        });

        // while (!finished) {
        //     Task.Delay(10).Wait();
        // }

        // outputStream.

        using MemoryStream ms = new();
        await ffmpegProcess.StandardOutput.BaseStream.CopyToAsync(ms);
        await ffmpegProcess.WaitForExitAsync();

        ms.Position = 0;
        return File(ms.ToArray(), "audio/mpeg");

        // using StreamReader sr = new(outputStream);
        // string audioFile = sr.ReadToEnd();
        // Response.ContentType = "audio/mpeg";
        // // return Ok(audioFile);

        // return File([..audioFile.Select(ch => (byte)ch)], "audio/mpeg");
        // return File([], "audio/mpeg");
        // return FileResult("audio/mpeg");
        // return new FileStreamResult(, "audio/mpeg");
    }
}