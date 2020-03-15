using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }



        public LeagueEntity ToLeagueEntity(LeagueViewModel model, string path, bool isNew)
        {
            return new LeagueEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name
            };
        }

        public LeagueViewModel ToLeagueViewModel(LeagueEntity leagueEntity)
        {
            return new LeagueViewModel
            {
                Id = leagueEntity.Id,
                LogoPath = leagueEntity.LogoPath,
                Name = leagueEntity.Name
            };
        }

        public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
        {
            return new TeamEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name,
                Initials=model.Initials,
                League=model.League,
            };
        }

        public TeamViewModel ToTeamViewModel(TeamEntity teamEntity)
        {
            return new TeamViewModel
            {
                Id = teamEntity.Id,
                LogoPath = teamEntity.LogoPath,
                Name = teamEntity.Name,
                Initials=teamEntity.Initials,
                League=teamEntity.League,
                LeagueId= teamEntity.League.Id
            };
        }

        public TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew)
        {
            return new TournamentEntity
            {
                EndDate = model.EndDate,
                Groups = model.Groups,
                DateNames=model.DateNames,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                LogoPath = path,
                Name = model.Name,
                StartDate = model.StartDate
            };
        }

        public TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity)
        {
            return new TournamentViewModel
            {
                EndDate = tournamentEntity.EndDate,
                Groups = tournamentEntity.Groups,
                DateNames= tournamentEntity.DateNames,
                Id = tournamentEntity.Id,
                IsActive = tournamentEntity.IsActive,
                LogoPath = tournamentEntity.LogoPath,
                Name = tournamentEntity.Name,
                StartDate = tournamentEntity.StartDate,
            };
        }

        public async Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew)
        {
            return new GroupEntity
            {
                GroupDetails = model.GroupDetails,
                Id = isNew ? 0 : model.Id,
                Matches = model.Matches,
                Name = model.Name,
                Tournament = await _context.Tournaments.FindAsync(model.TournamentId)
            };
        }

        public GroupViewModel ToGroupViewModel(GroupEntity groupEntity)
        {
            return new GroupViewModel
            {
                GroupDetails = groupEntity.GroupDetails,
                Id = groupEntity.Id,
                Matches = groupEntity.Matches,
                Name = groupEntity.Name,
                Tournament = groupEntity.Tournament,
                TournamentId = groupEntity.Tournament.Id
            };
        }

        public async Task<DateNameEntity> ToDateNameEntityAsync(DateNameViewModel model, bool isNew)
        {
            return new DateNameEntity
            {
                Id = isNew ? 0 : model.Id,
                Matches = model.Matches,
                Name = model.Name,
                Tournament = await _context.Tournaments.FindAsync(model.TournamentId)
            };
        }

        public DateNameViewModel ToDateNameViewModel(DateNameEntity dateNameEntity)
        {
            return new DateNameViewModel
            {
                Id = dateNameEntity.Id,
                Matches = dateNameEntity.Matches,
                Name = dateNameEntity.Name,
                Tournament = dateNameEntity.Tournament,
                TournamentId = dateNameEntity.Tournament.Id
            };
        }

        public async Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew)
        {
            return new GroupDetailEntity
            {
                GoalsAgainst = model.GoalsAgainst,
                GoalsFor = model.GoalsFor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                MatchesLost = model.MatchesLost,
                MatchesPlayed = model.MatchesPlayed,
                MatchesTied = model.MatchesTied,
                MatchesWon = model.MatchesWon,
                Team = await _context.Teams.FindAsync(model.TeamId)
            };
        }

        public GroupDetailViewModel ToGroupDetailViewModel(GroupDetailEntity groupDetailEntity)
        {
            return new GroupDetailViewModel
            {
                GoalsAgainst = groupDetailEntity.GoalsAgainst,
                GoalsFor = groupDetailEntity.GoalsFor,
                Group = groupDetailEntity.Group,
                GroupId = groupDetailEntity.Group.Id,
                Id = groupDetailEntity.Id,
                MatchesLost = groupDetailEntity.MatchesLost,
                MatchesPlayed = groupDetailEntity.MatchesPlayed,
                MatchesTied = groupDetailEntity.MatchesTied,
                MatchesWon = groupDetailEntity.MatchesWon,
                Team = groupDetailEntity.Team,
                LeagueId = groupDetailEntity.Team.League.Id,
                Leagues = _combosHelper.GetComboLeagues(),
                TeamId = groupDetailEntity.Team.Id,
                Teams = _combosHelper.GetComboTeams(0)
            };
        }

        public async Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew)
        {
            return new MatchEntity
            {
                Date = model.Date.ToUniversalTime(),
                GoalsLocal = model.GoalsLocal,
                GoalsVisitor = model.GoalsVisitor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                IsClosed = model.IsClosed,
                Local = await _context.Teams.FindAsync(model.LocalId),
                Visitor = await _context.Teams.FindAsync(model.VisitorId),
                DateName= model.DateName,
                
            };
        }

        public MatchViewModel ToMatchViewModel(MatchEntity matchEntity)
        {
            return new MatchViewModel
            {
                Date = matchEntity.Date.ToLocalTime(),
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                Teams = _combosHelper.GetComboTeams(matchEntity.Group.Id),
                DateNames = _combosHelper.GetComboDateNames(matchEntity.Group.Tournament.Id),
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                DateName= matchEntity.DateName,
                DateNameId=matchEntity.DateName.Id
            };
        }

        public async Task<MatchEntity> ToMatchEntityAsync2(MatchViewModel2 model, bool isNew)
        {
            return new MatchEntity
            {
                Date = model.Date.ToUniversalTime(),
                GoalsLocal = model.GoalsLocal,
                GoalsVisitor = model.GoalsVisitor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                IsClosed = model.IsClosed,
                Local = await _context.Teams.FindAsync(model.LocalId),
                Visitor = await _context.Teams.FindAsync(model.VisitorId),
                DateName = model.DateName,

            };
        }

        public MatchViewModel2 ToMatchViewModel2(MatchEntity matchEntity)
        {
            return new MatchViewModel2
            {
                Date = matchEntity.Date.ToLocalTime(),
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                Teams = _combosHelper.GetComboTeams(matchEntity.Group.Id),
                Groups = _combosHelper.GetComboGroups(matchEntity.Group.Tournament.Id),
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                DateName = matchEntity.DateName,
            };
        }

        public TournamentResponse ToTournamentResponse(TournamentEntity tournamentEntity)
        {
            return new TournamentResponse
            {
                Id = tournamentEntity.Id,
                Name = tournamentEntity.Name,
                StartDate = tournamentEntity.StartDate,
                EndDate = tournamentEntity.EndDate,
                IsActive = tournamentEntity.IsActive,
                LogoPath = tournamentEntity.LogoPath,
                Groups = tournamentEntity.Groups?.Select(g => new GroupResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    GroupDetails = g.GroupDetails?.Select(gd => new GroupDetailResponse
                    {
                        Id = gd.Id,
                        Team = ToTeamResponse(gd.Team),
                        MatchesPlayed = gd.MatchesPlayed,
                        MatchesWon = gd.MatchesWon,
                        MatchesTied = gd.MatchesTied,
                        MatchesLost = gd.MatchesLost,
                        GoalsFor = gd.GoalsFor,
                        GoalsAgainst = gd.GoalsAgainst,
                    }).ToList(),
                    Matches = g.Matches?.Select(m => new MatchResponse
                    {
                        Date = m.Date,
                        Local = ToTeamResponse(m.Local),
                        Visitor = ToTeamResponse(m.Visitor),
                        GoalsLocal = m.GoalsLocal,
                        GoalsVisitor = m.GoalsVisitor,
                        Id = m.Id,
                        IsClosed = m.IsClosed,
                        Predictions = m.Predictions?.Select(p => new PredictionResponse
                        {
                            Id = p.Id,
                            Player = ToPlayerResponse(p.Player),
                            GoalsLocal = p.GoalsLocal,
                            GoalsVisitor = p.GoalsVisitor,
                            Points = p.Points,
                        }).ToList()
                    }).ToList()
                }).ToList(),
                DateNames = tournamentEntity.DateNames?.Select(g => new DateNameResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Matches = g.Matches?.Select(m => new MatchResponse
                    {
                        Date = m.Date,
                        Local = ToTeamResponse(m.Local),
                        Visitor = ToTeamResponse(m.Visitor),
                        GoalsLocal = m.GoalsLocal,
                        GoalsVisitor = m.GoalsVisitor,
                        Id = m.Id,
                        IsClosed = m.IsClosed,
                        Predictions = m.Predictions?.Select(p => new PredictionResponse
                        {
                            Id = p.Id,
                            Player = ToPlayerResponse(p.Player),
                            GoalsLocal = p.GoalsLocal,
                            GoalsVisitor = p.GoalsVisitor,
                            Points = p.Points,
                        }).ToList()
                    }).ToList()
                }).ToList(),
            };
        }

        public List<TournamentResponse> ToTournamentResponse(List<TournamentEntity> tournamentEntities)
        {
            List<TournamentResponse> list = new List<TournamentResponse>();
            foreach (TournamentEntity tournamentEntity in tournamentEntities)
            {
                list.Add(ToTournamentResponse(tournamentEntity));
            }

            return list;
        }

        private PlayerResponse ToPlayerResponse(Player player)
        {
            if (player == null)
            {
                return null;
            }

            return new PlayerResponse
            {
                Address = player.User.Address,
                Document = player.User.Document,
                Email = player.User.Email,
                FirstName = player.User.FirstName,
                Id = player.Id,
                LastName = player.User.LastName,
                PhoneNumber = player.User.PhoneNumber,
                PicturePath = player.User.Picture,
                Team = ToTeamResponse(player?.User.FavoriteTeam),

            };
        }

        private TeamResponse ToTeamResponse(TeamEntity team)
        {
            if (team == null)
            {
                return null;
            }

            return new TeamResponse
            {
                Id = team.Id,
                LogoPath = team.LogoPath,
                Name = team.Name,
                Initials=team.Initials
            };
        }

        public User ToUser(Player player)
        {
            return new User
            {
             Address=player.User.Address,
             BornDate = player.User.BornDate,
             Document = player.User.Document,
             Email = player.User.Email,
             FirstName = player.User.FirstName,
             LastName = player.User.LastName,
             Latitude = player.User.Latitude,
             Longitude = player.User.Longitude,
             NickName = player.User.NickName,
             PhoneNumber = player.User.PhoneNumber,
             Picture = player.User.Picture,
             Points = player.User.Points,
             Sex = player.User.Sex,
             UserName = player.User.UserName,
             FavoriteTeam = player.User.FavoriteTeam,
             Id = player.User.Id,
            };
        }

        public PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity)
        {
            return new PredictionResponse
            {
                GoalsLocal = predictionEntity.GoalsLocal,
                GoalsVisitor = predictionEntity.GoalsVisitor,
                Id = predictionEntity.Id,
                Match = ToMatchResponse(predictionEntity.Match),
                Points = predictionEntity.Points
            };
        }

        public MatchResponse ToMatchResponse(MatchEntity matchEntity)
        {
            return new MatchResponse
            {
                Date = matchEntity.Date,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = ToTeamResponse(matchEntity.Local),
                Visitor = ToTeamResponse(matchEntity.Visitor)
            };
        }

    }
}