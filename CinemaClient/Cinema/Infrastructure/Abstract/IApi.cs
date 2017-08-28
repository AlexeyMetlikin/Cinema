using System.Threading.Tasks;

namespace Cinema.Infrastructure.Abstract
{
    public interface IApi
    {
        Task<string> SendRequest(string requestType, string request, string content);
    }
}
