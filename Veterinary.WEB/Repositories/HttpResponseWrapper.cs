using System.Net;

namespace Veterinary.WEB.Repositories;

public class HttpResponseWrapper<T>(T? response, bool error, HttpResponseMessage httpResponseMessage)
{
    public bool Error { get; set; } = error;

    public T? Response { get; set; } = response;

    public HttpResponseMessage HttpResponseMessage { get; set; } = httpResponseMessage;

    public async Task<string?> GetErrorMessageAsync()
    {
        if (!Error)
        {
            return null;
        }

        return HttpResponseMessage.StatusCode switch
        {
            HttpStatusCode.NotFound => "Recurso no encontrado",
            HttpStatusCode.BadRequest => await HttpResponseMessage.Content.ReadAsStringAsync(),
            HttpStatusCode.Unauthorized => "Debes loguearte para realizar esta accion",
            HttpStatusCode.Forbidden => "No tienes permisos para ejecutar esta accion",
            _ => "Ha ocurrido un error inesperado"
        };
    }
}
