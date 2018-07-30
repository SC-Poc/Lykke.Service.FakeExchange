using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.FakeExchange.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Lykke.Service.FakeExchange
{
    public static class HandleDomainExceptionsMiddleware
    {
    public static void UseHandleDomainExceptionsMiddleware(this IApplicationBuilder app)
    {
      app.Use(new Func<HttpContext, Func<Task>, Task>(SetStatusOnBusinessError));
    }

    public static async Task SetStatusOnBusinessError(HttpContext httpContext, Func<Task> next)
    {
      try
      {
        await next();
      }
      catch (NotEnoughLiquidityException ex)
      {
          MakeBadRequest(httpContext, ex.Message);
      }
      catch (InsufficientBalanceException ex)
      {
        MakeBadRequest(httpContext, "notEnoughBalance");
      }
      catch (InvalidInstrumentException ex)
      {
        MakeBadRequest(httpContext, ex.Message);
      }
      catch (NotImplementedException ex)
      {
        MakeBadRequest(httpContext, "notImplemented", HttpStatusCode.NotImplemented);
      }
    }

    private static void MakeBadRequest(HttpContext httpContext, string error, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(error)))
      {
        httpContext.Response.ContentType = "text/plain";
        httpContext.Response.StatusCode = (int) code;
        memoryStream.CopyTo(httpContext.Response.Body);
      }
    }
    }
}
