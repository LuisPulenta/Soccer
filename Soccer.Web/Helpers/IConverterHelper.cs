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

        LeagueResponse ToLeagueResponse(LeagueEntity leagueEntity);

        List<TournamentResponse> ToTournamentResponse(List<TournamentEntity> tournamentEntities);

        List<LeagueResponse> ToLeagueResponse(List<LeagueEntity> leagueEntities);

        User ToUser(Player player);

        Player ToPlayer(User user);

        PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity);

        MatchResponse ToMatchResponse(MatchEntity matchEntity);

        PlayerResponse ToPlayerResponse(Player player);

        Task<GroupBetResponse> ToGroupBetResponse(GroupBet groupBet);

    }
}