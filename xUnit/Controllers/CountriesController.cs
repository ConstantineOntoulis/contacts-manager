using Microsoft.AspNetCore.Mvc;

namespace xUnit.Controllers;

[Route("countries")]
public class CountriesController : Controller
{
    [Route("UploadFromExcel")]
    public async Task<IActionResult> UploadFromExcel()
    {
        return View();
    }
}