using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using schools_web_api.TokenManager.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
 
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
 
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AuthenticationException e)
        {
            await WriteResponse(context, HttpStatusCode.Forbidden, e.Message);
        }
        catch (AuthorizationException e)
        {
            await WriteResponse(context, HttpStatusCode.Forbidden, e.Message);
        }
        catch (ValidationException e)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, e.Message);
        }
        catch (DataException e)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, e.Message);
        }
        catch (DatabaseException e)
        {
            await WriteResponse(context, HttpStatusCode.InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            await WriteResponse(context, HttpStatusCode.InternalServerError, e.Message);
        }
    }

    private async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(message);
    }
}