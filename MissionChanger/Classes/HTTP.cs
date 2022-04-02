using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MissionChanger.Classes
{
    internal class HTTP
    {
        internal static HttpStatusCode Status = HttpStatusCode.OK;


        internal static string GetString(string request, out HttpStatusCode statuscode)
        {
            string s = GetString(request);
            statuscode = Status;
            return s;
        }


        internal static string GetString(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                Status = HttpStatusCode.ServiceUnavailable;

                HttpResponseMessage response = client.GetAsync(request).GetAwaiter().GetResult();

                Status = response.StatusCode;

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content;
            }
        }

        internal static async Task<string> GetStringAsync(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                Status = HttpStatusCode.ServiceUnavailable;

                HttpResponseMessage response = await client.GetAsync(request);

                Status = response.StatusCode;

                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }

        internal static void DownloadFile(string Uri, string destFile, BackgroundWorker backgoundWorker)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(Uri).GetAwaiter().GetResult())
                {
                    using (var fs = new FileStream(destFile, FileMode.CreateNew))
                    {
                        response.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}
