using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GovITHub.Auth.Common.Infrastructure.Localization
{
    public class ValidateMimeMultipartContentFilter : ActionFilterAttribute
    {
        private readonly ILogger logger;

        public ValidateMimeMultipartContentFilter(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("ctor ValidateMimeMultipartContentFilter");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsMultipartContentType(context.HttpContext.Request.ContentType))
            {
                context.Result = new StatusCodeResult(415);
                return;
            }

            base.OnActionExecuting(context);
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
