﻿using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckLeaguesAsync();
            await CheckTeamsAsync();
            await CheckTournamentsAsync();
            await CheckMatchesAsync();
            
            var favoriteTeam = await _context.Teams.FirstOrDefaultAsync(m => m.Id == 1);

            var manager = await CheckUserAsync( "Luis", "Nuñez",   "Luis", "Lucho", favoriteTeam, 0, "luisalbertonu@gmail.com", "156814963", "Manager");

            var player1 = await CheckUserAsync("Luis", "Nuñez",   "Luis", "Lucho", favoriteTeam, 0, "luis@yopmail.com", "156814963", "Player");
            var player2 = await CheckUserAsync( "Pablo", "Lacuadri",  "Pablo", "Pablito", favoriteTeam, 0, "lacua@yopmail.com", "123456789", "Player");
            var player3 = await CheckUserAsync( "Diego", "Maradona",   "Maradona", "Dieguito", favoriteTeam, 0, "maradona@yopmail.com", "10101010", "Player");
            await CheckPlayerAsync(player1, player2, player3);
            await CheckManagerAsync(manager);

            await CheckPreditionsAsync();

        }

        private async Task CheckPreditionsAsync()
        {
            if (!_context.Predictions.Any())
            {
                foreach (var player in _context.Players)
                {
                        AddPrediction(player);
                }

                await _context.SaveChangesAsync();
            }
        }

        private void AddPrediction(Player player)
        {
            var random = new Random();
            foreach (var match in _context.Matches)
            {
                _context.Predictions.Add(new PredictionEntity
                {
                    GoalsLocal = random.Next(0, 5),
                    GoalsVisitor = random.Next(0, 5),
                    Match = match,
                    Player=player
                });
            }
        }



        private async Task<User> CheckUserAsync(  string firstName,
                                                  string lastName,
                                                  string picture,
                                                  string nickName,
                                                  TeamEntity favoriteTeam,
                                                  int points,
                                                  string email,
                                                  string phoneNumber,
                                                  string role)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Picture = $"~/images/Users/{picture}.jpg",
                    NickName = nickName,
                    FavoriteTeam = favoriteTeam,
                    Points = points,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phoneNumber,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, Convert.ToString(role));
            }

            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);

            return user;
        }

        private async Task CheckPlayerAsync(User user1, User user2, User user3)
        {
            if (!_context.Players.Any())
            {
                _context.Players.Add(new Player { User = user1 });
                _context.Players.Add(new Player { User = user2 });
                _context.Players.Add(new Player { User = user3 });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_context.Managers.Any())
            {
                _context.Managers.Add(new Manager { User = user });
                await _context.SaveChangesAsync();
            }
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Manager.ToString());
            await _userHelper.CheckRoleAsync(UserType.Player.ToString());
            //await _userHelper.CheckRoleAsync("Manager");
            //await _userHelper.CheckRoleAsync("Player");
        }



        private async Task CheckLeaguesAsync()
        {
            if (!_context.Leagues.Any())
            {
                AddLeague("Superliga Argentina");
                AddLeague("Conmebol");
                AddLeague("Liga Aguila");
                await _context.SaveChangesAsync();
            }
        }

        private void AddLeague(string name)
        {
            _context.Leagues.Add(new LeagueEntity { Name = name, LogoPath = $"~/images/Leagues/{name}.jpg" });
        }


        private async Task CheckTeamsAsync()
        {
            LeagueEntity league1 = _context.Leagues.FirstOrDefault(t => t.Name == "Superliga Argentina");
            LeagueEntity league2 = _context.Leagues.FirstOrDefault(t => t.Name == "Conmebol");
            LeagueEntity league3 = _context.Leagues.FirstOrDefault(t => t.Name == "Liga Aguila");


            if (!_context.Teams.Any())
            {
                AddTeam("Aldosivi", "ALD", league1);
                AddTeam("Argentinos Juniors", "ARJ", league1);
                AddTeam("Arsenal", "ARS", league1);
                AddTeam("Atlético Tucumán", "ATL    ", league1);
                AddTeam("Banfield", "BAN", league1);
                AddTeam("Boca Juniors", "BOC", league1);
                AddTeam("Central Córdoba", "CCA", league1);
                AddTeam("Colón", "COL", league1);
                AddTeam("Defensa y Justicia", "DYJ", league1);
                AddTeam("Estudiantes", "EST", league1);
                AddTeam("Gimnasia", "GIM", league1);
                AddTeam("Godoy Cruz", "GOD", league1);
                AddTeam("Huracán", "HUR", league1);
                AddTeam("Independiente", "IND", league1);
                AddTeam("Lanús", "LAN", league1);
                AddTeam("Newells", "NEW", league1);
                AddTeam("Patronato", "PAT", league1);
                AddTeam("Racing", "RAC", league1);
                AddTeam("River Plate", "RIV", league1);
                AddTeam("Rosario Central", "ROS", league1);
                AddTeam("San Lorenzo", "SLO", league1);
                AddTeam("Talleres", "TAL", league1);
                AddTeam("Union", "UNI", league1);
                AddTeam("Velez Sarsfield", "VEL", league1);

                AddTeam("Argentina", "ARG", league2);
                AddTeam("Bolivia", "BOL", league2);
                AddTeam("Brasil", "BRA", league2);
                AddTeam("Chile", "CHI", league2);
                AddTeam("Colombia", "COL", league2);
                AddTeam("Ecuador", "ECU", league2);
                AddTeam("Paraguay", "PAR", league2);
                AddTeam("Peru", "PER", league2);
                AddTeam("Uruguay", "URU", league2);
                AddTeam("Venezuela", "VEN", league2);

                AddTeam("America", "AME", league3);
                AddTeam("Bucaramanga", "BUC", league3);
                AddTeam("Junior", "JUN", league3);
                AddTeam("Medellin", "MED", league3);
                AddTeam("Millonarios", "MIL", league3);
                AddTeam("Nacional", "NAC", league3);
                AddTeam("Once Caldas", "ONC", league3);
                AddTeam("Santa Fe", "SFE", league3);


                await _context.SaveChangesAsync();
            }
        }

        private void AddTeam(string name,string initials, LeagueEntity league)
        {
            _context.Teams.Add(new TeamEntity { Name = name, Initials=initials, League = league, LogoPath = $"~/images/Teams/{name}.jpg" });
        }


        private async Task CheckMatchesAsync()
        {
            var startDate = DateTime.Today.AddMonths(2).ToUniversalTime();

            if (!_context.Matches.Any())
            {
                GroupEntity group = _context.Groups.FirstOrDefault(t => t.Id == 1);
                DateNameEntity dateName1 = _context.DateNames.FirstOrDefault(t => t.Id == 1);
                
                DateTime date1 = startDate.AddHours(14);
                TeamEntity local1 = _context.Teams.FirstOrDefault(t => t.Name == "Argentina");
                TeamEntity visitor1 = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia");
                AddMatch(group,dateName1,date1,local1,visitor1);

                DateTime date2 = startDate.AddHours(16);
                TeamEntity local2 = _context.Teams.FirstOrDefault(t => t.Name == "Brasil");
                TeamEntity visitor2 = _context.Teams.FirstOrDefault(t => t.Name == "Chile");
                AddMatch(group, dateName1, date2, local2, visitor2);


                
                DateNameEntity dateName2 = _context.DateNames.FirstOrDefault(t => t.Id == 2);

                DateTime date3 = startDate.AddHours(62);
                TeamEntity local3 = _context.Teams.FirstOrDefault(t => t.Name == "Argentina");
                TeamEntity visitor3 = _context.Teams.FirstOrDefault(t => t.Name == "Brasil");
                AddMatch(group, dateName2, date3, local3, visitor3);

                DateTime date4 = startDate.AddHours(64);
                TeamEntity local4 = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia");
                TeamEntity visitor4 = _context.Teams.FirstOrDefault(t => t.Name == "Chile");
                AddMatch(group, dateName2, date4, local4, visitor4);

                
                DateNameEntity dateName3 = _context.DateNames.FirstOrDefault(t => t.Id == 3);

                DateTime date5 = startDate.AddHours(110);
                TeamEntity local5 = _context.Teams.FirstOrDefault(t => t.Name == "Argentina");
                TeamEntity visitor5 = _context.Teams.FirstOrDefault(t => t.Name == "Chile");
                AddMatch(group, dateName3, date3, local5, visitor5);

                DateTime date6 = startDate.AddHours(112);
                TeamEntity local6 = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia");
                TeamEntity visitor6 = _context.Teams.FirstOrDefault(t => t.Name == "Brasil");
                AddMatch(group, dateName3, date4, local6, visitor6);

                await _context.SaveChangesAsync();
            }

        }

        private void AddMatch(GroupEntity group, DateNameEntity dateName, DateTime date, TeamEntity local, TeamEntity visitor)
        {
            _context.Matches.Add(new MatchEntity { Date=date, Local=local, Visitor=visitor, Group=group, DateName=dateName });
        }




        private async Task CheckTournamentsAsync()
        {
            if (!_context.Tournaments.Any())
            {
                var startDate = DateTime.Today.AddMonths(2).ToUniversalTime();
                var endDate = DateTime.Today.AddMonths(3).ToUniversalTime();

                _context.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = $"~/images/Tournaments/Copa América 2020.jpg",
                    Name = "Copa América 2020",
                    DateNames =new List<DateNameEntity>
                    {
                        new DateNameEntity
                        {
                            Name = "Fecha 01",
                        },
                        new DateNameEntity
                        {
                            Name = "Fecha 02",
                        },
                        new DateNameEntity
                        {
                            Name = "Fecha 03",
                        },
                    },
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                             Name = "A",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Argentina") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Brasil") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Chile") }
                             },
                             
                        },
                        new GroupEntity
                        {
                             Name = "B",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Colombia") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Ecuador") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Paraguay") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Peru") }
                             },
                             
                        },
                    }
                });

                startDate = DateTime.Today.AddMonths(1).ToUniversalTime();
                endDate = DateTime.Today.AddMonths(4).ToUniversalTime();

                _context.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = $"~/images/Tournaments/Liga Aguila 2020.jpg",
                    Name = "Liga Aguila 2020",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                             Name = "A",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "America") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Junior") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Medellin") }
                             },
                             
                        },
                        new GroupEntity
                        {
                             Name = "B",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Nacional") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas") },
                                 new GroupDetailEntity { Team = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe") }
                             },
                             
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

    }
}
