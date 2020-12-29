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
    [Route("service/[controller]")]
    public class CuratorController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;

        /// <summary>
        /// Default constructor for CuratorController.
        /// </summary>
        public CuratorController()
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
        [UnhandledExceptionError]
        [HttpGet]
        public ActionResult<IEnumerable<ICuratedImage>> GetCuratedImages([FromServices] ICuratorServiceLogic curatorService)
        {
            var result = curatorService.GetCuratedImages();
            return Ok(result);
        }

        /// <summary>
        /// Get a curated image by Id.
        /// </summary>
        /// <remarks>
        /// Get an image
        /// </remarks>
        /// <param name="key">Key of curated image</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        [UnhandledExceptionError]
        [HttpGet("{key}")]
        public ActionResult<ICuratedImage> GetCuratedImageByKey([FromServices] ICuratorServiceLogic curatorService, [FromRoute] string key)
        {
            var result = curatorService.GetCuratedImageByKey(key);

            return Ok(result);
        }

        /// <summary>
        /// Add a curated image.
        /// </summary>
        /// <remarks>
        /// Add an image
        /// </remarks>
        /// <param name="curatedImage">Curated image</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        [UnhandledExceptionError]
        [HttpPost]
        public ActionResult<ICuratedImage> AddCuratedImage([FromServices] ICuratorServiceLogic curatorService, CuratedImage curatedImage)
        {
            if (curatedImage?.Path == null)
            {
                return BadRequest("Invalid or missing image path.");
            }

            var result = curatorService.SaveCuratedImage(curatedImage);

            return Ok(result);
        }

        /// <summary>
        /// Update curated image.
        /// </summary>
        /// <remarks>
        /// Update curated image
        /// </remarks>
        /// <param name="key">Key of curated image</param>
        /// <param name="curatedImage">Curated image</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        [UnhandledExceptionError]
        [HttpPut("{key}")]
        public ActionResult<ICuratedImage> UpdateCuratedImage([FromServices] ICuratorServiceLogic curatorService, [FromRoute] string key, CuratedImage curatedImage)
        {
            if (key == null)
            {
                return BadRequest("Invalid or missing key.");
            }
            if (curatedImage?.Path == null)
            {
                return BadRequest("Invalid or missing image path.");
            }

            var result = curatorService.SaveCuratedImage(key, curatedImage);

            return Ok(result);
        }

        /// <summary>
        /// Delete curated image.
        /// </summary>
        /// <remarks>
        /// Delete curated image
        /// </remarks>
        /// <param name="key">Key of curated image</param>
        /// <response code="204">Success</response>
        /// <response code="400">Bad Request</response>
        [UnhandledExceptionError]
        [HttpDelete("{key}")]
        public ActionResult DeleteCuratedImageByKey([FromServices] ICuratorServiceLogic curatorService, [FromRoute] string key)
        {
            if (key == null)
            {
                return BadRequest("Invalid or missing key.");
            }

            curatorService.DeleteCuratedImage(key);

            return NoContent();
        }

    }
}
