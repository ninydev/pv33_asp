using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Sse;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SseService _sseService;

    public HomeController(ILogger<HomeController> logger, SseService sseService)
    {
        _logger = logger;
        _sseService = sseService;
    }

    public async Task<IActionResult> Index()
    {
        await _sseService.SendToAllAsync("Hello from server!");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}