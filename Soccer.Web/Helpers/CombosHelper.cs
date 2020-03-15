using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboLeagues()
        {
            List<SelectListItem> list = _context.Leagues.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Elija una Liga...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboTeams(int Id)
        {
            var list = _context.Teams.Where(p => p.League.Id == Id).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Elija un Equipo...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboTeams2(int Id)
        {
            var list = _context.GroupDetails
                .Include(gd => gd.Team)
                .Where(p => p.Group.Id == Id)
                .Select(p => new SelectListItem
            {
                    Text = p.Team.Name,
                    Value = $"{p.Team.Id}"
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Elija un Equipo...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboDateNames(int Id)
        {
            var list = _context.DateNames.Where(p => p.Tournament.Id == Id).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Elija una Fecha...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboGroups(int Id)
        {
            var list = _context.Groups.Where(p => p.Tournament.Id == Id).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Elija un Grupo...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboUserTypes()
        {
            var list = _context.Teams.Select(l => new SelectListItem
            {
                Text = l.Name,
                Value = $"{l.Id}"
            })
                .OrderBy(l => l.Text)
                .Where(l => l.Text == "z")
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione Rol del Usuario...]",
                Value = "0"
            });

            list.Insert(1, new SelectListItem
            {
                Text = UserType.Manager.ToString(),
                Value = "1"
            });

            list.Insert(2, new SelectListItem
            {
                Text = UserType.Player.ToString(),
                Value = "2"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboSexs()
        {
            var list = _context.Teams.Select(l => new SelectListItem
            {
                Text = l.Name,
                Value = $"{l.Id}"
            })
                .OrderBy(l => l.Text)
                .Where(l => l.Text == "z")
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione Sexo...]",
                Value = "0"
            });

            list.Insert(1, new SelectListItem
            {
                Text = "Hombre",
                Value = "Hombre"
            });

            list.Insert(2, new SelectListItem
            {
                Text = "Mujer",
                Value = "Mujer"
            });

            return list;
        }
    }
}