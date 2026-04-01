using Microsoft.AspNetCore.Mvc.RazorPages;

public class BasePageModel : PageModel
{
    public string FallbackImage { get; set; } = "/Content/shared_pictures/30a2e3e4-3965-42e6-8d0d-56014ab45021_20250802122729_1000035523.png";

    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        // Optionally override or customize per request
        base.OnPageHandlerExecuting(context);
    }
}
