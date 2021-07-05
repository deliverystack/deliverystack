namespace Deliverystack.DeliveryApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Deliverystack.DeliveryApi.Models;
    using Deliverystack.Models;

    [Route("[controller]")]
    public class RedirectApiController : ControllerBase
    {
        private readonly ILogger<PathApiController> _logger;
        RedirectApiCache _redirectApiCache;

        public RedirectApiController(
            ILogger<PathApiController> logger,
            RedirectApiCache redirectApiCache)
        {
            _logger = logger;
            _redirectApiCache = redirectApiCache;
        }

        [HttpGet]
        public ActionResult Get(RedirectModel values)
        {
            if (_redirectApiCache.ContainsPath(values.Url))
            {
                return Ok(new RedirectModel(_redirectApiCache.Get(values.Url).Url));
            }

            return Ok(string.Empty);
            //TODO: more detailed response - ErrorDetails?
    //            return NotFound(values.Url + " not in " + _redirectApiCache);
        }
    }
}
