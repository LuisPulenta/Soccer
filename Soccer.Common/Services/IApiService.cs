using Soccer.Common.Models;
using System.Threading.Tasks;

namespace Soccer.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller);

        Task<Response> GetListAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            int Id);

        Task<bool> CheckConnectionAsync(string url);

        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

        Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

    }
}