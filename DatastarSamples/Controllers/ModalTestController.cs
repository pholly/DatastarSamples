using DatastarSamples.Extensions;
using Microsoft.AspNetCore.Mvc;
using StarFederation.Datastar.DependencyInjection;

namespace DatastarSamples.Controllers;

public class ModalTestController: Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task ReloadModal([FromServices] IDatastarServerSentEventService sse, bool first)
    {
        var modalContent = await this.RenderViewToStringAsync("_Modal1", first);
        await sse.MergeFragmentsAsync(modalContent);

        if (first)
        {
            await sse.ExecuteScriptAsync("showModal()");
        }
    }

    [HttpPost]
    public async Task Save([FromServices] IDatastarServerSentEventService sse)
    {
        await Task.Delay(200);
        await sse.ExecuteScriptAsync("hideModal()");
        await sse.ExecuteScriptAsync("showSuccessToast()");
    }
}