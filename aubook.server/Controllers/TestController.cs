using Microsoft.AspNetCore.Mvc;

namespace aubook.server;

public class TestController : Controller
{
    public IActionResult Test()
    {
        return Ok();
    }
}