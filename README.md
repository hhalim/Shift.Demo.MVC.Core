# Shift.Demo.Mvc.Core
A Shift client running in ASP.NET MVC Core web app. This demo demonstrates the Shift capability to run client and server in the same application process.

## Quick Startup
Install Redis for windows [Redis-x64-<version>.msi](https://github.com/MSOpenTech/redis/releases) package.

Or to use the SQL Server, first run the sql script to create Shift database in [/setup/create_db.sql](https://github.com/hhalim/Shift.Demo.Mvc.Core/blob/master/setup/create_db.sql). 

Open this project solution in Visual Studio 2017, update the appsettings.json connection string.
```
  "Shift": {
    //"ShiftDBConnection": "Data Source=localhost\\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;", //MS SQL
    //"ShiftDBConnection": "mongodb://localhost", //mongodb
    //"ShiftDBConnection": "https://localhost:8081/", //Azure DocumentDB
    "ShiftDBConnection": "localhost:6379", //redis

    //"DocumentDBAuthKey" : "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",

    "ApplicationID": "Demo.MVC.Core",
    "MaxRunnableJobs": 2,
    "ShiftWorkers": 2,
    //"ShiftPID": "8c76f3d540394ced94b1b51b9bf1a43c",

    "StorageMode": "redis", //redis, mongo, mssql, documentdb
    "ProgressDBInterval": "00:00:00",

    "ForceStopServer": true,
    "StopServerDelay": 5000,

    //"AutoDeletePeriod": 120,
    //"ServerTimerInterval": 5000,
    //"ServerTimerInterval2": 10000,
    //"AssemblyFolder": "client-assemblies\\",
    //"AssemblyListPath": "client-assemblies\\assemblylist.txt",
    //"ShiftEncryptionParametersKey": "",
    //"PollingOnce": true
  } 
```

- Build and run the site.
- Click Add Jobs link to add multiple test jobs into the queue.
- Go to the Dashboard and click `Run Server` to run Shift server. Or select jobs and click `Run Selected` to run selected jobs.
- Use the Status & Progress link to view the running jobs in auto refreshing grid. 
- Try other action buttons to see what Shift can do.

The `Stop Cmd`, `Pause Cmd`, `Continue Cmd`, and `Run-Now Cmd` buttons only marks the selected jobs for those actions. Shift server will pick up jobs and acted on them as marked. However if the jobs are manually started through `Run Selected` button, then you must manually use `Process Commands` action to execute the commands in the server process or nothing would happen to the background jobs.
