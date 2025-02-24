using System.Diagnostics;
using DatastarSamples.Models;
using DatastarSamples.Extensions;
using Microsoft.AspNetCore.Mvc;
using StarFederation.Datastar.DependencyInjection;

namespace DatastarSamples.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        //show all orders
        var vm = new IndexViewModel(Order.SampleOrders);
        return View(vm);
    }

    [HttpPost("max")]
    public async Task _OrdersTable([FromServices] IDatastarServerSentEventService sse, [FromForm] int maxOrderCount, [FromForm] int delayInSeconds = 0)
    {
        //get first x orders
        if (maxOrderCount < 0)
        {
            var badAlertVm = new AlertViewModel("You can't ask for a negative max order count", "alert-danger");
            await StreamAlert(badAlertVm, sse);
            return;
        }

        string alertMessage;

        if (maxOrderCount > Order.SampleOrders.Count())
        {
            alertMessage = $"There are only {Order.SampleOrders.Count()} sample orders so only those will returned.";
            maxOrderCount = Order.SampleOrders.Count();
        }
        else
        {
            alertMessage = $"[{maxOrderCount}] orders will be returned.";
        }
        
        var alertVm = new AlertViewModel(alertMessage, "alert-info");
        await StreamAlert(alertVm, sse);
        
        if (delayInSeconds is < 0 or > 5)
        {
            delayInSeconds = 5;
        }
        
        await Task.Delay(delayInSeconds * 1000);
        
        IEnumerable<Order> orders = Order.SampleOrders.Take(maxOrderCount);
        var tableVm = new TablePartialViewModel(orders);
        await StreamOrderTable(tableVm, sse);
        
        await StreamAlert(new AlertViewModel("Loading complete", "alert-success"), sse);
    }

    public async Task StreamAlert(AlertViewModel alertViewModel, IDatastarServerSentEventService sse)
    {
        var html = await this.RenderViewToStringAsync("_Alert", alertViewModel);
        await sse.MergeFragmentsAsync(html);
    }

    public async Task StreamOrderTable(TablePartialViewModel tablePartialViewModel, IDatastarServerSentEventService sse)
    {
        var html = await this.RenderViewToStringAsync("_TablePartial", tablePartialViewModel);
        await sse.MergeFragmentsAsync(html);
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