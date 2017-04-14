using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shift;

namespace MyApp.Client.Controllers
{
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

        public IActionResult Index(string jobID)
        {
            ViewBag.JobID = jobID;
            return View();
        }

        public async Task<IActionResult> GetProgress(string jobID)
        {
            var jsProgress = await jobClient.GetProgressAsync(jobID).ConfigureAwait(false);

            return Json(jsProgress);
        }
    }
}
