namespace Deliverystack.DeliveryApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Deliverystack.DeliveryApi.Models;
    using Deliverystack.Models;

    [Route("[controller]")]
    public class PathApiController : ControllerBase
    {
        private readonly ILogger<PathApiController> _logger;
        PathApiCache _pathCache;

        public PathApiController(
            ILogger<PathApiController> logger,
            PathApiCache pathCache)
        {
            _logger = logger;
            _pathCache = pathCache;
        }

        [HttpGet]
        public ActionResult Get(PathApiBindingModel values)
        {
            if (String.IsNullOrWhiteSpace(values.Path))
            {
                return BadRequest(new ProblemDetails()
                {
                    Status = 400,
                    Title = "Path required",
                    // Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
                });
            }

            if (!_pathCache.ContainsPath(values.Path))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Entry identified by path not found",
                });
            }

            List<PathApiEntryModel> list = new();
            PathApiResultModel result = new();

            if (values.Ancestors > 0)
            {
                PathApiEntryModel[] ancestors = _pathCache.GetAncestors(values.Path);

                if (values.Ancestors > ancestors.Length)
                {
                    values.Ancestors = ancestors.Length;
                }

                for (int i = ancestors.Length - values.Ancestors; i < ancestors.Length; i++)
                {
                    list.Add(ancestors[i]);
                    result.Ancestors++;
                }
            }

            foreach(PathApiEntryModel entry in _pathCache.GetCurrentGeneration(
                values.Path, values.ExcludeSelf, values.Siblings))
            {
                list.Add(entry);
                result.CurrentGeneration++;
            }

            if (values.Descendants > 0)
            {
                foreach (PathApiEntryModel child in _pathCache.GetDescendants(values.Path, values.Descendants))
                {
                    result.Descendants++;
                    list.Add(child);
                }
            }

            result.Entries = list.Skip(values.PageIndex * values.PageSize).Take(values.PageSize);
            return Ok(result);
        }
    }
}