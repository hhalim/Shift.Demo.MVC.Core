using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shift;

namespace MyApp.Client.Controllers
{
    [Route("[controller]")]
    public class MethodController : Controller
    {
        private static JobClient jobClient;

        public MethodController(IMemoryCache cache)
        {
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
        }

        [Route("[action]/{jobID?}")]
        public async Task<IActionResult> Index(string jobID)
        {
            ViewBag.JobView = await jobClient.GetJobViewAsync(jobID);

            return View();
        }

    }
}
