using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IConverterHelper
    {
        LeagueEntity ToLeagueEntity(LeagueViewModel model, string path, bool isNew);

        LeagueViewModel ToLeagueViewModel(LeagueEntity leagueEntity);

        GroupBet ToGroupBet(GroupBetViewModel model, string path, bool isNew);

        GroupBetViewModel ToGroupBetViewModel(GroupBet groupBet);

        TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew);

        TeamViewModel ToTeamViewModel(TeamEntity teamEntity);

        TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew);

        TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity);

        Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew);

        GroupViewModel ToGroupViewModel(GroupEntity groupEntity);

        Task<DateNameEntity> ToDateNameEntityAsync(DateNameViewModel model, bool isNew);

        DateNameViewModel ToDateNameViewModel(DateNameEntity dateNameEntity);

        Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew);

        GroupDetailViewModel ToGroupDetailViewModel(GroupDetailEntity groupDetailEntity);

        Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew);

        MatchViewModel ToMatchViewModel(MatchEntity matchEntity);

        Task<MatchEntity> ToMatchEntityAsync2(MatchViewModel2 model, bool isNew);

        MatchViewModel2 ToMatchViewModel2(MatchEntity matchEntity);

        TournamentResponse ToTournamentResponse(TournamentEntity tournamentEntity);
        List<TournamentResponse> ToTournamentResponse(List<TournamentEntity> tournamentEntities);
        LeagueResponse ToLeagueResponse(LeagueEntity leagueEntity);
        List<LeagueResponse> ToLeagueResponse(List<LeagueEntity> leagueEntities);
        User ToUser(Player player);
        Task<Player> ToPlayer2(User user);
        Player ToPlayer(User user);
        PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity);
        PredictionResponse3 ToPredictionResponse3(PredictionEntity predictionEntity);

        MatchResponse ToMatchResponse(MatchEntity matchEntity);

        PlayerResponse ToPlayerResponse(Player player);

        

        //Task<GroupBetPlayerResponse> ToGroupBetPlayerResponse(GroupBetPlayer groupBetPlayer);

        GroupResponse2 ToGroupResponse(GroupEntity groupEntity);
        List<GroupResponse2> ToGroupResponse(List<GroupEntity> groupEntities);

        Task<GroupDetailResponse2> ToGroupDetailResponse(GroupDetailEntity groupDetailEntity);
        Task<List<GroupDetailResponse2>> ToGroupDetailResponse(List<GroupDetailEntity> groupDetailEntities);

        Task<MatchResponse2> ToMatchResponse2(MatchEntity matchEntity);
        Task<List<MatchResponse2>> ToMatchResponse2(List<MatchEntity> matchEntities);

        Task<GroupBetResponse2> ToGroupBetResponse2(GroupBetPlayer groupBetPlayer);
        Task<List<GroupBetResponse2>> ToGroupBetResponse2(List<GroupBetPlayer> groupBetPlayerEntities);

    }
}