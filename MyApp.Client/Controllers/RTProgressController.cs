using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shift;

namespace MyApp.Client.Controllers
{
    [Route("[controller]")]
    public class RTProgressController : Controller
    {
        private static JobClient jobClient;

        public RTProgressController(IMemoryCache cache)
        {
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
        }

        [Route("[action]/{jobID?}")]
        public async Task<IActionResult> Index(string jobID)
        {
            ViewBag.JobID = jobID;
            return View();
        }

        [Route("[action]/{jobID?}")]
        public async Task<IActionResult> GetProgress(string jobID)
        {
            var jsProgress = await jobClient.GetProgressAsync(jobID).ConfigureAwait(false);

            return Json(jsProgress);
        }
    }
}
