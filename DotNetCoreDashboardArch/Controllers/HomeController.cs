using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using DotNetCoreDashboardArch.Services;
using DotNetCoreDashboardArch.Models.Home;
using DotNetCoreDashboardArch.Services.Repositories;

namespace DotNetCoreDashboardArch.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private ICacheProvider _cache;
        private DateTime _lastUpdated;
   
        public HomeController(ICacheDependency dependency, ICacheProvider cache)
        {
            _cache = cache;
            _lastUpdated = dependency.LastUpdated;
        }

        public IActionResult Main()
        {
            ViewBag.lastUpdated = _lastUpdated;
            return View();
        }

        public async Task<IActionResult> GetSummaryStats([FromServices] IHomeRepository data)
        {
            string cacheKey = "sumStats";
            SummaryStats ssData;
            bool cacheExists = _cache.TryGetItem(cacheKey, _lastUpdated, out ssData);
            if (!cacheExists)
            {
                ssData = await data.GetSummaryAsync();
                _cache.AddItem(cacheKey, _lastUpdated, ssData);
            }
            return Content(JsonConvert.SerializeObject(ssData), "application/json");
        }
    }
}
