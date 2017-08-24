using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyApp.BusinessLayer;
using MyApp.DataLayer;
using Shift;
using Shift.Entities;

namespace MyApp.Client.Controllers
{
    public class JobController : Controller
    {
        private ShiftConfig _settings;
        private static JobClient jobClient;

        public JobController(IMemoryCache cache, IOptions<ShiftConfig> settings)
        {
            _settings = settings.Value;
            if (jobClient == null)
            {
                jobClient = cache.Get("Shift.JobClient") as JobClient;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Add(int? p1Count, int? p2Count, int? p3Count, int? p4Count, int? p5Count)
        {
            var appID = _settings.ApplicationID;
            if (string.IsNullOrWhiteSpace(appID))
                appID = "Shift.Demo.MVC.Core";

            var message = "";
            if (p1Count > 0)
            {
                for (var i = 0; i < p1Count; i++)
                {
                    var job1 = new Job1();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    var cancelToken = (new CancellationTokenSource()).Token; //placeholder to be replaced by Server token
                    var pauseToken = (new PauseTokenSource()).Token;
                    await jobClient.AddAsync(appID, () => job1.Start("Hello World!", progress, cancelToken, pauseToken));
                }

                message += p1Count + " - Job1 added to queue. <br/>";
            }

            if (p2Count > 0)
            {
                for (var i = 0; i < p2Count; i++)
                {
                    var job2 = new Job2();
                    var progress = new SynchronousProgress<ProgressInfo>(); //just a place holder to be replaced by real Progress object from the server
                    var cancelToken = (new CancellationTokenSource()).Token; //placeholder to be replaced by Server token
                    var pauseToken = (new PauseTokenSource()).Token;
                    await jobClient.AddAsync(appID, () => job2.Start(progress, 1, cancelToken, pauseToken));
                }

                message += p2Count + " - Job2 added to queue. <br/>";
            }

            if (p3Count > 0)
            {
                var simpleList = new List<int>();
                for (var i = 1; i <= 100; i++)
                {
                    simpleList.Add(i);
                }

                for (var i = 0; i < p3Count; i++)
                {
                    var job3 = new Job3();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    await jobClient.AddAsync(appID, () => job3.Start(progress, simpleList));
                }

                message += p3Count + " - Job3 added to queue. <br/>";
            }

            if (p4Count > 0)
            {
                var complexList = new List<TestData>();
                for (var i = 1; i <= 100; i++)
                {
                    var newData = new TestData();
                    newData.MyNumber = i;
                    newData.MyString = "mystring " + i;
                    complexList.Add(newData);
                }

                for (var i = 0; i < p4Count; i++)
                {
                    var job4 = new Job4();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    var cancelToken = (new CancellationTokenSource()).Token; //placeholder to be replaced by Server token
                    var pauseToken = (new PauseTokenSource()).Token;
                    await jobClient.AddAsync(appID, () => job4.Start(progress, complexList, cancelToken, pauseToken));
                }

                message += p4Count + " - Job4 added to queue. <br/>";
            }

            if (p5Count > 0)
            {
                var simpleList = new List<int>();
                for (var i = 1; i <= 100; i++)
                {
                    simpleList.Add(i);
                }

                for (var i = 0; i < p5Count; i++)
                {
                    var job5 = new Job5();
                    var cancelToken = (new CancellationTokenSource()).Token; //placeholder to be replaced by Server token
                    var pauseToken = (new PauseTokenSource()).Token;
                    var progress = new SynchronousProgress<ProgressInfo>();
                    await jobClient.AddAsync(appID, () => job5.StartAsync(progress, simpleList, cancelToken, pauseToken));
                }

                message += p5Count + " - Job5 added to queue. <br/>";
            }

            ViewBag.Message = message;

            return View("Index");
        }


    }
}
