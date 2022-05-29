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

        public GroupBet ToGroupBet(GroupBetViewModel model, string path, bool isNew)
        {
            return new GroupBet
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                CreationDate = model.CreationDate,
                Admin = model.Admin,
                Tournament = model.Tournament,
                GroupBetPlayers = model.GroupBetPlayers,
                Name = model.Name
            };
        }

        public GroupBetViewModel ToGroupBetViewModel(GroupBet groupBet)
        {
            return new GroupBetViewModel
            {
                Id = groupBet.Id,
                LogoPath = groupBet.LogoPath,
                Name = groupBet.Name,
                Admin = groupBet.Admin,
                CreationDate = groupBet.CreationDate,
                GroupBetPlayers = groupBet.GroupBetPlayers,
                Tournament = groupBet.Tournament
            };
        }

        public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
        {
            return new TeamEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name,
                Initials = model.Initials,
                League = model.League,
            };
        }

        public TeamViewModel ToTeamViewModel(TeamEntity teamEntity)
        {
            return new TeamViewModel
            {
                Id = teamEntity.Id,
                LogoPath = teamEntity.LogoPath,
                Name = teamEntity.Name,
                Initials = teamEntity.Initials,
                League = teamEntity.League,
                LeagueId = teamEntity.League.Id
            };
        }

        public TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew)
        {
            return new TournamentEntity
            {
                EndDate = model.EndDate,
                Groups = model.Groups,
                DateNames = model.DateNames,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                LogoPath = path,
                Name = model.Name,
                StartDate = model.StartDate
            };
        }

        public TournamentEntity ToTournamentEntity(TournamentResponse model)
        {
            return new TournamentEntity
            {
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                Name = model.Name,
                StartDate = model.StartDate,
                Id = model.Id,
                LogoPath = model.LogoPath,
            };
        }

        public TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity)
        {
            return new TournamentViewModel
            {
                EndDate = tournamentEntity.EndDate,
                Groups = tournamentEntity.Groups,
                DateNames = tournamentEntity.DateNames,
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
                DateName = model.DateName,

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
                DateName = matchEntity.DateName,
                DateNameId = matchEntity.DateName.Id
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
                        DateName=m.DateName.Name,
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

               public PlayerResponse ToPlayerResponse(Player player)
        {
            if (player == null)
            {
                return null;
            }

            return new PlayerResponse
            {
                NickName = player.User.NickName,
                Email = player.User.Email,
                FirstName = player.User.FirstName,
                Id = player.Id,
                UserId = player.User.Id,
                LastName = player.User.LastName,
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
                Initials = team.Initials,
                LeagueId = team.League.Id,
                LeagueName = team.League.Name

            };
        }

        private DateNameResponse ToDateNameResponse(DateNameEntity dateNameEntity)
        {
            if (dateNameEntity == null)
            {
                return null;
            }

            return new DateNameResponse
            {
                Id = dateNameEntity.Id,
                Name = dateNameEntity.Name,
                Tournament = ToTournamentResponse(dateNameEntity.Tournament)
            };
        }

        

        public User ToUser(Player player)
        {
            return new User
            {
                Email = player.User.Email,
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                NickName = player.User.NickName,
                PhoneNumber = player.User.PhoneNumber,
                Picture = player.User.Picture,
                Points = player.User.Points,
                UserName = player.User.UserName,
                FavoriteTeam = player.User.FavoriteTeam,
                Id = player.User.Id,
            };
        }

        public async Task<Player> ToPlayer2(User user)
        {
            var player = await _context.Players.FindAsync(user.Id);
            return new Player
            {
                Id = player.Id,
                User = player.User,
            };
        }

        public Player ToPlayer(User user)
        {
            return new Player
            {
                User = user,
            };
        }

        public Player ToPlayer(PlayerResponse playerResponse)
        {
            return new Player
            {
                User = ToUser(ToPlayer(playerResponse))
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
                Points = predictionEntity.Points,
                Player= ToPlayerResponse(predictionEntity.Player)
            };
        }

        public PredictionResponse3 ToPredictionResponse3(PredictionEntity predictionEntity)
        {
            return new PredictionResponse3
            {
                
                GoalsLocalPrediction = predictionEntity.GoalsLocal,
                GoalsVisitorPrediction = predictionEntity.GoalsVisitor,
                GoalsLocalReal= predictionEntity.Match.GoalsLocal,
                GoalsVisitorReal = predictionEntity.Match.GoalsVisitor,
                InitialsLocal= predictionEntity.Match.Local.Initials,
                InitialsVisitor = predictionEntity.Match.Visitor.Initials,
                LogoPathLocal= predictionEntity.Match.Local.LogoPath,
                LogoPathVisitor= predictionEntity.Match.Visitor.LogoPath,
                NameLocal=predictionEntity.Match.Local.Name,
                NameVisitor= predictionEntity.Match.Visitor.Name,
                MatchId= predictionEntity.Match.Id,
                MatchDate= predictionEntity.Match.DateLocal,
                PlayerId = predictionEntity.Player.Id,
                TournamentId= predictionEntity.Match.Group.Tournament.Id,
                Id = predictionEntity.Id,
                Points = predictionEntity.Points,
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
                Visitor = ToTeamResponse(matchEntity.Visitor),
                DateName= matchEntity.DateName.Name,
                //Group= matchEntity.Group
            };
        }

        public LeagueResponse ToLeagueResponse(LeagueEntity leagueEntity)
        {
            return new LeagueResponse
            {
                Id = leagueEntity.Id,
                Name = leagueEntity.Name,
                LogoPath=leagueEntity.LogoPath,
                Teams = leagueEntity.Teams?.Select(g => new TeamResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Initials = g.Initials,
                    LogoPath = g.LogoPath,
                    LeagueId = g.League.Id,
                    LeagueName = g.League.Name
                }).ToList(),
            };
        }


        public List<LeagueResponse> ToLeagueResponse(List<LeagueEntity> leagueEntities)
        {
            List<LeagueResponse> list = new List<LeagueResponse>();
            foreach (LeagueEntity leagueEntity in leagueEntities)
            {
                list.Add(ToLeagueResponse(leagueEntity));
            }

            return list;
        }

        public GroupResponse2 ToGroupResponse(GroupEntity groupEntity)
        {
            if (groupEntity == null)
            {
                return null;
            }

            return new GroupResponse2
            {
                Id = groupEntity.Id,
                Name = groupEntity.Name,
            };
        }

        public List<GroupResponse2> ToGroupResponse(List<GroupEntity> groupEntities)
        {
            List<GroupResponse2> list = new List<GroupResponse2>();
            foreach (GroupEntity groupEntity in groupEntities)
            {
                list.Add(ToGroupResponse(groupEntity));
            }

            return list;
        }

        public async Task<GroupDetailResponse2> ToGroupDetailResponse(GroupDetailEntity groupDetailEntity)
        {
            if (groupDetailEntity == null)
            {
                return null;
            }

            return new GroupDetailResponse2
            {
                Id = groupDetailEntity.Id,
                GoalsAgainst = groupDetailEntity.GoalsAgainst,
                GoalsFor = groupDetailEntity.GoalsFor,
                MatchesLost = groupDetailEntity.MatchesLost,
                MatchesPlayed = groupDetailEntity.MatchesPlayed,
                MatchesTied = groupDetailEntity.MatchesTied,
                MatchesWon = groupDetailEntity.MatchesWon,
                Team = ToTeamResponse(await _context.Teams.FindAsync(groupDetailEntity.Team.Id)),
            };
        }

        public async Task<List<GroupDetailResponse2>> ToGroupDetailResponse(List<GroupDetailEntity> groupDetailEntities)
        {
            List<GroupDetailResponse2> list = new List<GroupDetailResponse2>();
            foreach (GroupDetailEntity groupDetailEntity in groupDetailEntities)
            {
                list.Add(await ToGroupDetailResponse(groupDetailEntity));
            }

            return list;
        }

        public async Task<MatchResponse2> ToMatchResponse2(MatchEntity matchEntity)
        {
            if (matchEntity == null)
            {
                return null;
            }

            return new MatchResponse2
            {
                Date= matchEntity.Date,
                DateName = matchEntity.DateName.Name,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Group = ToGroupResponse(await _context.Groups.FindAsync(matchEntity.Group.Id)),
                IsClosed = matchEntity.IsClosed,
                Local= ToTeamResponse(await _context.Teams.FindAsync(matchEntity.Local.Id)),
                Visitor = ToTeamResponse(await _context.Teams.FindAsync(matchEntity.Visitor.Id)),
                Id = matchEntity.Id,
            };
        }

        public async Task<List<MatchResponse2>> ToMatchResponse2(List<MatchEntity> matches)
        {
            List<MatchResponse2> list = new List<MatchResponse2>();
            foreach (MatchEntity matchEntity in matches)
            {
                list.Add(await ToMatchResponse2(matchEntity));
            }
            return list;
        }

        public async Task<GroupBetResponse2> ToGroupBetResponse2(GroupBetPlayer groupBetPlayer)
        {
            return new GroupBetResponse2
            {
                Id = groupBetPlayer.Id,
                AdminName= groupBetPlayer.GroupBet.Admin.User.FullName,
                AdminPicture = groupBetPlayer.GroupBet.Admin.User.ImageFullPath,
                AdminTeam = groupBetPlayer.GroupBet.Admin.User.FavoriteTeam.LogoFullPath,
                LogoPath = groupBetPlayer.GroupBet.LogoPath,
                TournamentName=groupBetPlayer.GroupBet.Tournament.Name,
                GroupBetPlayers = groupBetPlayer.GroupBet.GroupBetPlayers?.Select(g => new GroupBetPlayerResponse2
                {
                    Id = g.Id,
                    IsAccepted = g.IsAccepted,
                    IsBlocked=g.IsBlocked,
                }).ToList(),
                CreationDate=groupBetPlayer.GroupBet.CreationDate,
                Name=groupBetPlayer.GroupBet.Name,
                CantPlayers=groupBetPlayer.GroupBet.GroupBetPlayers.Count(),
            };
        }

        public async Task<List<GroupBetResponse2>> ToGroupBetResponse2(List<GroupBetPlayer> groupBetPlayers)
        {
            List<GroupBetResponse2> list = new List<GroupBetResponse2>();
            foreach (GroupBetPlayer groupBetPlayer in groupBetPlayers)
            {
                list.Add(await ToGroupBetResponse2(groupBetPlayer));
            }
            return list;
        }
    }
}