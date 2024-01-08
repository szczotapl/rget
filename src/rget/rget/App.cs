using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

class App
{
    [Verb("download", HelpText = "Download a file.")]
    class DownloadOptions
    {
        [Value(0, MetaName = "url", Required = true, HelpText = "URL to download")]
        public string Url { get; set; } = string.Empty;

        [Option('o', "output", HelpText = "Output file path")]
        public string? OutputFile { get; set; }

        [Option('h', "headers", Separator = ',', HelpText = "Custom headers (comma-separated)")]
        public IEnumerable<string> Headers { get; set; } = new List<string>();

        [Option('c', "concurrent", Default = 1, HelpText = "Number of concurrent downloads")]
        public int ConcurrentDownloads { get; set; }

        [Option('t', "timeout", Default = 30, HelpText = "Timeout in seconds for each request")]
        public int TimeoutSeconds { get; set; }

        [Option('p', "post", Default = false, HelpText = "Use POST request instead of GET")]

        public bool UsePost { get; set; }
    }

    public static void Run(string[] args)
    {
        try
        {
            Parser.Default.ParseArguments<DownloadOptions>(args)
                .WithParsed(options => Download(options))
                .WithNotParsed(errors => HandleParseErrors(errors));
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = Colors.ErrorMessage;
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void Download(DownloadOptions options)
    {
        using (HttpClient client = new HttpClient())
        {
            foreach (var header in options.Headers ?? Array.Empty<string>())
            {
                var headerParts = header.Split(':');
                if (headerParts.Length == 2)
                {
                    client.DefaultRequestHeaders.Add(headerParts[0], headerParts[1]);
                }
            }

            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            Console.Write($"Downloading from {options.Url}... ");
            var resultBytes = DownloadWithProgress(client, options.Url, PrintProgressBar, options.ConcurrentDownloads, options.UsePost).Result;

            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                File.WriteAllBytes(options.OutputFile, resultBytes);
                Console.WriteLine($"\nData downloaded successfully and saved to {options.OutputFile}");

                // Display the content of the downloaded file
                string content = Encoding.UTF8.GetString(resultBytes);
                Console.WriteLine($"\nContent:\n{content}");
            }
            else
            {
                string content = Encoding.UTF8.GetString(resultBytes);
                Console.WriteLine($"\nContent:\n{content}");
            }
        }
    }

    static async Task<byte[]> DownloadWithProgress(HttpClient client, string url, Action<int, int> progressCallback, int concurrentDownloads, bool usePost)
    {
        using (HttpResponseMessage response = usePost
            ? await client.PostAsync(url, null)
            : await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            var contentLength = response.Content.Headers.ContentLength;
            var totalBytesRead = 0L;
            var result = new MemoryStream();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[4096];
                var isMoreToRead = true;

                do
                {
                    var read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        totalBytesRead += read;
                        var progress = (int)((totalBytesRead * 100) / contentLength);
                        progressCallback(progress, 100);
                        result.Write(buffer, 0, read);
                    }
                } while (isMoreToRead);
            }

            progressCallback(100, 100);
            return result.ToArray();
        }
    }

    static void PrintProgressBar(int progress, int total)
    {
        var percentage = (int)((progress * 100.0) / total);
        Console.ForegroundColor = Colors.ProgressText;
        Console.Write($"\rProgress: {progress}% ");
        Console.Write("[");
        Console.BackgroundColor = Colors.ProgressBarBackground;
        Console.ForegroundColor = Colors.ProgressBarForeground;
        var completed = (int)(progress / (double)total * 50);
        Console.Write(new string('#', completed));
        Console.Write(new string(' ', 50 - completed));
        Console.ResetColor();
        Console.Write("]");
    }

    static void HandleParseErrors(IEnumerable<Error> errors)
    {
        Console.ForegroundColor = Colors.ErrorMessage;
        Console.WriteLine("Invalid command-line arguments. Please check the provided options.");
        Console.ResetColor();
    }
}
