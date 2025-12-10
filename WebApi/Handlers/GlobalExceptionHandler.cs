using Microsoft.AspNetCore.Diagnostics;
using RealStateApp.Core.Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RealStateWebApi.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            string title = "Unexpected error";
            int status = (int)HttpStatusCode.InternalServerError;
            string details = exception.Message;
            switch (exception)
            {
                case ApiException api:
                    status = api.StatusCode;

                    switch (api.StatusCode)
                    {
                        case (int)HttpStatusCode.BadRequest:
                            title = "Bad request";
                            break;

                        case (int)HttpStatusCode.NotFound:
                            title = "Not found";
                            break;

                        case (int)HttpStatusCode.InternalServerError:
                            title = "Internal server error";
                            break;

                        default:
                            title = "Unexpected API error";
                            break;
                    }
                    break;

                case KeyNotFoundException:
                    status = (int)HttpStatusCode.NotFound;
                    title = "Not found";
                    break;

                case ArgumentException:
                    status = (int)HttpStatusCode.BadRequest;
                    title = "Bad request";
                    break;

                case RealStateApp.Core.Application.Exceptions.ValidationException:
                    status = (int)HttpStatusCode.BadRequest;
                    title = "Bad request";
                    details= ((RealStateApp.Core.Application.Exceptions.ValidationException) exception).Errors.Aggregate((a, b) => a + "," + b);
                    break;

                default:
                    break;
            }

            var problem = new
            {
                Title = title,
                Status = status,
                Detail = details,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken: cancellationToken);

            return true;
        }
    }

}