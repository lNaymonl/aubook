using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace aubook.server.Models;

public class TimeSpanConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var casted = value as string;
        if (casted != null && decimal.TryParse(casted.Replace('.', ','), out decimal dec))
        {
            int durInSec = (int)Math.Round(dec);
            return new TimeSpan(0, 0, durInSec);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        var casted = value as TimeSpan?;
        if (destinationType == typeof(string) && casted != null)
        {
            return casted.Value.TotalSeconds.ToString();
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

public class AudioData
{
    [JsonPropertyName("filename")]
    public required string Filename { get; set; }
    [JsonPropertyName("nb_streams")]
    public required int NbStreams { get; set; }
    [JsonPropertyName("nb_programs")]
    public required int NbPrograms { get; set; }
    [JsonPropertyName("nb_stream_groups")]
    public required int NbStreamGroups { get; set; }
    [JsonPropertyName("format_name")]
    public required string FormatName { get; set; }
    [JsonPropertyName("format_long_name")]
    public required string FormatLongName { get; set; }
    [JsonPropertyName("start_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required decimal StartTime { get; set; }
    // [JsonPropertyName("duration")]
    // [JsonConverter(typeof(TimeSpanConverter))]
    // public required TimeSpan Duration { get; set; }
    [JsonPropertyName("size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required uint Size { get; set; }
    [JsonPropertyName("bit_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required uint BitRate { get; set; }
    [JsonPropertyName("probe_score")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required uint ProbeScore { get; set; }
    [JsonPropertyName("tags")]
    public required Tags Tags { get; set; }

    // public required TimeSpan Duration { get; set; }
    // public required string Filename { get; set; }
    // public required string Title { get; set; }
    // public required string Artist { get; set; }
    // public required string Comment { get; set; }
    // public required string Description { get; set; }
    // public required string Synopsis { get; set; }
    // public required DateTime Date { get; set; }

    // private class Metadata
    // {

    // }

    public static AudioData ParseMetadata(string filePath)
    {
        string fullPath = Path.GetFullPath(filePath);

        StringBuilder args = new();
        args.Append($"-i {fullPath} ");
        args.Append("-v quiet ");
        args.Append("-print_format json ");
        args.Append("-show_format ");

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffprobe",
                Arguments = args.ToString(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        proc.Start();

        _ = Task.Run(() =>
        {
            var error = proc.StandardError.ReadToEnd();
            Console.Error.WriteLine(error);
        });

        using StreamReader sr = new(proc.StandardOutput.BaseStream);
        string data = sr.ReadToEnd();

        var metadata = JsonSerializer.Deserialize<Metadata>(data) ??
            throw new NullReferenceException("Faild to parse properly");

        return metadata.Format;
    }
}

public class Tags
{
    [JsonPropertyName("major_brand")]
    public required string MajorBrand { get; set; }
    [JsonPropertyName("minor_version")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required uint MinorVersion { get; set; }
    [JsonPropertyName("compatible_brands")]
    public required string CompatibleBrands { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [JsonPropertyName("artist")]
    public required string Artist { get; set; }
    // [JsonPropertyName("date")]
    // public required DateTime Date { get; set; }
    [JsonPropertyName("synopsis")]
    public required string Synopsis { get; set; }
    [JsonPropertyName("comment")]
    public required string Comment { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("encoder")]
    public required string Encoder { get; set; }
}

public class Metadata
{
    [JsonPropertyName("format")]
    public required AudioData Format { get; set; }
}
