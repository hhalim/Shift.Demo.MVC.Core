using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shift;

namespace MyApp.Client.Controllers
{
    public class ProgressController : Controller
    {
        private static JobClient jobClient;

        public ProgressController(IMemoryCache cache)
        {
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ReadData(int? pageIndex, int? pageSize)
        {
            var jobViewList = await jobClient.GetJobViewsAsync(pageIndex, pageSize).ConfigureAwait(false);
            var output = new Dictionary<string, object>();
            output.Add("data", jobViewList.Items);
            output.Add("itemsCount", jobViewList.Total);

            return Json(output);
        }
    }
}
