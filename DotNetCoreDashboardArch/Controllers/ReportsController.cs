using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using DotNetCoreDashboardArch.Services;
using DotNetCoreDashboardArch.Models.Reports;
using DotNetCoreDashboardArch.Services.Repositories;


namespace DotNetCoreDashboardArch.Controllers
{
    [AllowAnonymous]
    public class ReportsController : Controller
    {
        private ICacheProvider _cache;
        private DateTime _lastUpdated;

        public ReportsController(ICacheDependency dependency, ICacheProvider cache)
        {
            _cache = cache;
            _lastUpdated = dependency.LastUpdated;
        }
        public IActionResult Main()
        {
            ViewBag.lastUpdated = _lastUpdated;
            return View();
        }

        public async Task<IActionResult> SearchFields([FromServices] IReportsRepository data)
        {
            string cacheKey = "customfields";
            bool cacheExists = _cache.TryGetItem(cacheKey, _lastUpdated, out CustomSearchFields fields);
            if (!cacheExists)
            {
                fields = await data.GetCustomFields();
                _cache.AddItem(cacheKey, _lastUpdated, fields);
            }
            return Content(JsonConvert.SerializeObject(fields), "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> Accounts([FromServices] IReportsRepository data, [FromForm] ReportParms parms)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            ReportResults results = await data.GetAccountSearchResults(parms);
            return Content(JsonConvert.SerializeObject(results), "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> SearchTA([FromServices] IReportsRepository data, [FromBody] TypeaheadParms parms)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            TypeaheadResults results = await data.GetTypeAheadResults(parms);
            return Content(JsonConvert.SerializeObject(results), "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> Entitlement([FromServices] IReportsRepository data, [FromForm] EntitlementSearchParms parms)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            EntitlementSearchResults searchObject = await data.GetEntitlementSearchResults(parms);
            return Content(JsonConvert.SerializeObject(searchObject), "application/json");
        }
    }
}
