using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace aubook.server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioController : Controller
{
    [HttpGet("stream")]
    public IActionResult StreamTranscodedAudio()
    {
        Console.WriteLine("stream");
        string inputFile = "./Eragon_1_CD1_01.mp3";
        string fullPath = Path.GetFullPath(inputFile);

        var ffmpegProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{fullPath}\" -f mp3 -b:a 96k -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
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

        Response.ContentType = "audio/mpeg";
        return new FileStreamResult(outputStream, "audio/mpeg");
    }
}