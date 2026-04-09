using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IPaperService _paperService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IPaperService paperService, IWebHostEnvironment env)
        {
            _paperService = paperService;
            _env = env;
        }


    }
}