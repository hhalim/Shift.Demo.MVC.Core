# Shift.Demo.Mvc.Core
A Shift client running in ASP.NET MVC Core web app. This demo demonstrates the Shift capability to run client and server in the same application process.

## Quick Startup
Install Redis for windows [Redis-x64-<version>.msi](https://github.com/MSOpenTech/redis/releases) package.

Or to use the SQL Server:
- Run the sql script to create Shift database in [/setup/create_db.sql](https://github.com/hhalim/Shift.Demo.Mvc.Core/blob/master/setup/create_db.sql). 
- If you want to use Redis cache, setup and create a Redis instance. 

Open this project solution in Visual Studio, update the appsettings.json connection string and cache.
```
  "Shift": {
    //    "ShiftDBConnection": "Data Source=localhost\\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;", //MS SQL
    //    "ShiftDBConnection": "mongodb://localhost", //mongodb
    "ShiftDBConnection": "localhost:6379", //redis
    "ApplicationID": "Demo.MVC.Core",
    "MaxRunnableJobs": 10,
    "ShiftPID": "8c76f3d540394ced94b1b51b9bf1a43c",
    "StorageMode": "redis", //mongo, mssql
    "ProgressDBInterval": "00:00:00",
    //"UseCache": true,
    //"RedisConfiguration": "localhost:6379",
    //"AutoDeletePeriod": 120,
    //"ServerTimerInterval": 5000,
    //"ServerTimerInterval2": 10000,
    //"AssemblyFolder": "client-assemblies\\",
    //"AssemblyListPath": "client-assemblies\\assemblylist.txt",
    //"ShiftEncryptionParametersKey": "",
    "PollingOnce": true
  } 
```

- Build and run the site.
- Click Add Jobs link to add multiple test jobs into the queue.
- Go to the Dashboard and click Run Server to run Shift server. Or select jobs and click Run Selected to run selected jobs.
- Use the Status & Progress link to view the running jobs in auto refreshing grid. 
- Try other action buttons to see what Shift can do.

The Mark as Stop and Mark as Run-Now buttons only marks the selected jobs for those actions. Shift server will pick up those jobs and acted on them as marked. 
