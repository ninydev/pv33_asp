namespace WebApplication1.Controllers;

using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite; // Замените при другой БД
using Microsoft.Extensions.Options;


[ApiController]
[Route("check/health")]
public class HealthController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IConfiguration config, ILogger<HealthController> logger)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dbHealthy = await CheckDatabaseAsync();

        var driveInfo = GetDriveInfo(); // Можно сузить до диска с контентом/логами
        var memoryInfo = GetMemoryInfo();

        var status = new
        {
            status = dbHealthy && driveInfo.FreeBytes > 0 ? "Healthy" : "Degraded",
            timestampUtc = DateTime.UtcNow,
            database = new
            {
                healthy = dbHealthy
            },
            storage = new
            {
                drive = driveInfo.DriveName,
                totalBytes = driveInfo.TotalBytes,
                freeBytes = driveInfo.FreeBytes
            },
            memory = new
            {
                workingSetBytes = memoryInfo.WorkingSetBytes,
                privateMemoryBytes = memoryInfo.PrivateBytes,
                gcAllocatedBytes = memoryInfo.GcAllocatedBytes
            }
        };

        return dbHealthy ? Ok(status) : StatusCode(503, status);
    }

    private async Task<bool> CheckDatabaseAsync()
    {
        try
        {
            await using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1;";
            var result = await cmd.ExecuteScalarAsync();

            return result is long l && l == 1
                   || result is int i && i == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQLite health check failed");
            return false;
        }
    }

    private static (string DriveName, long TotalBytes, long FreeBytes) GetDriveInfo()
    {
        // Выберем диск, где находится текущий контент приложения
        var contentRoot = AppContext.BaseDirectory;
        var rootPath = Path.GetPathRoot(contentRoot) ?? "/";
        var drive = DriveInfo.GetDrives()
            .FirstOrDefault(d => d.IsReady && string.Equals(d.RootDirectory.FullName, rootPath, StringComparison.OrdinalIgnoreCase))
            ?? DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady);

        if (drive == null)
            return ("unknown", 0, 0);

        return (drive.Name, drive.TotalSize, drive.AvailableFreeSpace);
    }

    private static (long WorkingSetBytes, long PrivateBytes, long GcAllocatedBytes) GetMemoryInfo()
    {
        var proc = Process.GetCurrentProcess();
        // WorkingSet64 — текущая физическая память процесса
        var workingSet = proc.WorkingSet64;
        // Частная память: на Windows можно через PerformanceCounter/PROCESS_MEMORY_COUNTERS; здесь приблизим PrivateMemorySize64
        var privateBytes = proc.PrivateMemorySize64;
        // Общий объём выделенной управляемой памяти (последний GC snapshot)
        var gcAllocated = GC.GetTotalMemory(forceFullCollection: false);

        return (workingSet, privateBytes, gcAllocated);
    }
}