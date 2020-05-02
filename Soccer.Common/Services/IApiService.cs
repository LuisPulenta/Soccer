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

        bool CheckConnection();

        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

        Task<Response> GetGroupBetPlayerByIds(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, GroupBetPlayerRequest2 groupBetRequest);

        Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

        Task<Response> RecoverPasswordAsync(string urlBase, string servicePrefix, string controller, EmailRequest emailRequest);

        Task<Response> PutAsync<T>(string urlBase, string servicePrefix, string controller, T model, string tokenType, string accessToken);

        Task<Response> ChangePasswordAsync(string urlBase, string servicePrefix, string controller, ChangePasswordRequest changePasswordRequest, string tokenType, string accessToken);

        Task<Response> AddGroupBetAsync(string urlBase, string servicePrefix, string controller, GroupBetRequest groupBetRequest, string tokenType, string accessToken);

        Task<Response> AddGroupBetPlayerAsync(string urlBase, string servicePrefix, string controller, GroupBetPlayerRequest groupBetPlayerRequest, string tokenType, string accessToken);

        Task<ResponseT<object>> GetGroupBetsByEmail(
           string urlBase,
           string servicePrefix,
           string controller,
           string id,
           string tokenType,
           string accessToken);

        Task<Response> DeleteAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            int id,
            string tokenType,
            string accessToken);

         Task<Response> GetPredictionsForUserAsync(string urlBase, string servicePrefix, string controller, PredictionsForUserRequest predictionsForUserRequest, string tokenType, string accessToken);

        Task<Response> MakePredictionAsync(string urlBase, string servicePrefix, string controller, PredictionRequest predictionRequest, string tokenType, string accessToken);

        Task<Response> InviteAsync(string urlBase, string servicePrefix, string controller, AddUserGroupBetRequest addUserGroupBetRequest, string tokenType, string accessToken);

        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken);
    }
}