using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shift;
using Newtonsoft.Json;

namespace MyApp.Client.Controllers
{
    public class StatusController : Controller
    {
        private ShiftConfig _settings;
        private static JobClient jobClient;

        public StatusController(IMemoryCache cache, IOptions<ShiftConfig> settings)
        {
            _settings = settings.Value;
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
        }

        public IActionResult Index()
        {
            ViewBag.ApplicationID = _settings.ApplicationID;

            return View();
        }

        public async Task<IActionResult> StatusCount(string appID, string userID)
        {
            var output = await jobClient.GetJobStatusCountAsync(appID, userID).ConfigureAwait(false);
            ViewBag.Result = JsonConvert.SerializeObject(output);
            ViewBag.ApplicationID = _settings.ApplicationID;
            return View("Index");
        }

    }
}
