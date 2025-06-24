using Microsoft.AspNetCore.Mvc;

namespace aubook.server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TestController : Controller
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        Response.ContentType = "text/plain";
        return Ok("Hallo");
    }
}