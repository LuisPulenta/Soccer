using Soccer.Common.Models;
using Soccer.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public VersionsController(DataContext dataContext)


        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetVersions()
        {
            var Versions = await _dataContext.Versions
                .ToListAsync();

            var response = new List<VersionResponse>(Versions.Select(o => new VersionResponse
            {
                Fecha = o.Fecha,
                NroVersion = o.NroVersion
            }).ToList());

            return Ok(response);
        }
    }
}