using Cinema.Infrastructure.Abstract;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Infrastructure.Entities
{
    public class CinemaApi : ICinemaApi
    {
        public string Host { get; }

        public CinemaApi(string host)
        {
            Host = host;
        }

        public async Task<string> SendRequest(string requestType, string request, string content)
        {
            string result = null;
            HttpResponseMessage response;

            switch (requestType.ToUpper())
            {
                case "POST":
                    response = SendPost(request, content);
                    break;

                case "GET":
                    response = await SendGet(request);
                    break;

                default:
                    throw new HttpRequestException("Не распознан метод запроса '" + requestType + "'");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            result = response.Content.ReadAsStringAsync().Result;

            return result;

        }

        private async Task<HttpResponseMessage> SendGet(string request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Host + request));
                return response;
            }
        }

        private HttpResponseMessage SendPost(string request, string content)
        {
            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = client.PostAsync(Host + request, stringContent).Result;
                return response;
            }
        }
    }
}
