using Microsoft.AspNetCore.Mvc;

namespace Fusion.Microservice.Controller;

/// <summary>
/// Serves as the base controller for the API, providing common configurations
/// and behaviors for derived controllers.
/// </summary>
/// <remarks>
/// This controller includes attributes such as <c>ApiController</c> for
/// automatic model validation and response formatting, <c>Consumes</c> to
/// specify the content type supported, and <c>Produces</c> to specify the
/// content type returned by actions within derived controllers.
/// </remarks>
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/")]
public class BaseController : ControllerBase;