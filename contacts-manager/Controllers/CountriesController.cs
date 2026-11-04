using Microsoft.AspNetCore.Mvc;

namespace contacts;

[Route("countries")]
public class CountriesController : Controller
{
    [Route("UploadFromExcel")]
    public async Task<IActionResult> UploadFromExcel()
    {
        return View();
    }
}