using System.Text;
using System.Text.Json;

namespace Veterinary.WEB.Repositories;

public class Repository(HttpClient httpClient) : IRepository
{
    private readonly HttpClient _httpClient = httpClient;

    private JsonSerializerOptions JsonDefaultOptions => new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
    {
        var responseHttp = await _httpClient.GetAsync(url);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<T>(responseHttp, JsonDefaultOptions);
            return new HttpResponseWrapper<T>(response, false, responseHttp);
        }

        return new HttpResponseWrapper<T>(default, true, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);
        return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    public async Task<HttpResponseWrapper<TResponse>> PostAsync<T, TResponse>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<TResponse>(responseHttp, JsonDefaultOptions);
            return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
        }

        return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
    {
        var responseHttp = await _httpClient.DeleteAsync(url);
        return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PutAsync(url, messageContent);
        return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    public async Task<HttpResponseWrapper<TResponse>> PutAsync<T, TResponse>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PutAsync(url, messageContent);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<TResponse>(responseHttp, JsonDefaultOptions);
            return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
        }

        return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
    }

    private static async Task<T> UnserializeAnswer<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
    {
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        if (typeof(T) == typeof(string))
        {
            var mediaType = httpResponse.Content.Headers.ContentType?.MediaType;
            if (string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase))
            {
                return (T)(object)(JsonSerializer.Deserialize<string>(responseString, jsonSerializerOptions) ?? string.Empty);
            }

            return (T)(object)responseString;
        }

        return JsonSerializer.Deserialize<T>(responseString, jsonSerializerOptions)!;
    }
}
