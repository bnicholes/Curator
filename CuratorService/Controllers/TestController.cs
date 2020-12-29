using CuratorService.Attributes;
using CuratorService.Data;
using CuratorService.Logic;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
#pragma warning disable 1573


namespace CuratorService.Controllers
{
    /// <summary>
    /// Manage a set of curated images.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;

        /// <summary>
        /// Default constructor for CuratorController.
        /// </summary>
        public TestController()
        {
            _logger = Serilog.Log.Logger;
        }

        /// <summary>
        /// Get a list of curated images.
        /// </summary>
        /// <remarks>
        /// Curated images
        /// </remarks>
        /// <response code="200">Success</response>
        [HttpGet]
        public ActionResult Test()
        {
            _logger.Information("Got here");
            return Ok();
        }
    }
}
