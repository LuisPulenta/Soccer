using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Soccer.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboLeagues();

        IEnumerable<SelectListItem> GetComboTeams(int Id);

        IEnumerable<SelectListItem> GetComboTeams2(int Id);

        IEnumerable<SelectListItem> GetComboDateNames(int Id);

        IEnumerable<SelectListItem> GetComboGroups(int Id);

    }
}