using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RebalanceBot
{
    public class Utilities
    {
        public static async Task DownloadMap(string id)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://osu.ppy.sh/osu/{id}");
            await using Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                stream = new FileStream($"cache/{id}.osu", FileMode.Create);
            await contentStream.CopyToAsync(stream);
        }
    }
}