using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shift;

namespace MyApp.Client.Controllers
{
    public class DashboardController : Controller
    {
        private static JobClient jobClient;
        private static JobServer jobServer;

        public DashboardController(IMemoryCache cache)
        {
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
            if (jobServer == null)
            {
                jobServer = cache.Get("Shift.JobServer") as JobServer;
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

        public async Task<IActionResult> Delete(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Delete jobs that's not running
            await jobClient.DeleteJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> Reset(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Reset jobs that's not running
            await jobClient.ResetJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        #region Shift Actions
        public async Task<IActionResult> Stop(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            await jobClient.SetCommandStopAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> Pause(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            await jobClient.SetCommandPauseAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> Continue(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            await jobClient.SetCommandContinueAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> RunNow(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to 'run-now', wait for RunServer to pickup and run it
            await jobClient.SetCommandRunNowAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> RunSelected(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            await jobServer.RunJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> RunServer()
        {
            //Jobs running through this function will be running under the IIS Process!
            await jobServer.RunServerAsync().ConfigureAwait(false); //Run jobs server, use the MaxRunableJobs setting
            return Json(true);
        }

        public async Task<IActionResult> StopServer()
        {
            await jobServer.StopServerAsync().ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> CleanUp()
        {
            await jobServer.CleanUpAsync().ConfigureAwait(false);

            return Json(true);
        }

        public async Task<IActionResult> ProcessCmd()
        {
            await jobServer.PauseJobsAsync().ConfigureAwait(false);
            await jobServer.ContinueJobsAsync().ConfigureAwait(false);
            await jobServer.StopJobsAsync().ConfigureAwait(false);

            return Json(true);
        }
        #endregion
    }
}
