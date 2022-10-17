using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient) 
        => _httpClient = httpClient;

    // O método vai retornar uma tupla
    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
        where TOutput : class
    {
        // Faz um Post na API
        var response = await _httpClient.PostAsync(
            route, 
            new StringContent(
                JsonSerializer.Serialize(payload), 
                Encoding.UTF8, 
                "application/json")
            );

        var output = await GetOutput<TOutput>(response);
        
        // Tenho o retorno da tupla, dois resultados em 1 retorno do método
        // Poderia ter até mais retornos nesse método também
        return (response, output);
    }
    
    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload)
        where TOutput : class
    {
        // Faz um Put na API
        var response = await _httpClient.PutAsync(
            route, 
            new StringContent(
                JsonSerializer.Serialize(payload), 
                Encoding.UTF8, 
                "application/json")
        );

        var output = await GetOutput<TOutput>(response);
        
        // Tenho o retorno da tupla, dois resultados em 1 retorno do método
        // Poderia ter até mais retornos nesse método também
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string route, 
        object? queryStringParametersObjects = null
    )
        where TOutput : class
    {
        // Transforma o objeto em parâmetros da rota
        // O retorno fica da seguinte forma: /categories?Page=1&PerPage=5&Search=&Sort=&Dir=0
        var url = PrepareGetRoute(route, queryStringParametersObjects);
        
        // Faz um Get na API
        var response = await _httpClient.GetAsync(url);

        var output = await GetOutput<TOutput>(response);
        
        // Tenho o retorno da tupla, dois resultados em 1 retorno do método
        // Poderia ter até mais retornos nesse método também
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route)
        where TOutput : class
    {
        // Faz um Delete na API
        var response = await _httpClient.DeleteAsync(route);

        var output = await GetOutput<TOutput>(response);
        
        // Tenho o retorno da tupla, dois resultados em 1 retorno do método
        // Poderia ter até mais retornos nesse método também
        return (response, output);
    }

    private async Task<TOutput?> GetOutput<TOutput>(HttpResponseMessage response) where TOutput : class
    {
        // Deserialize o retorno
        // Pego em formato de string
        var outputString = await response.Content.ReadAsStringAsync();
        
        // Faz o Deserialize para o formato TOutput do que tem no outputString
        TOutput? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, new JsonSerializerOptions
            {
                // Para deixar os nomes CaseInsensitive
                PropertyNameCaseInsensitive = true
            });
        }

        return output;
    }
    
    private string PrepareGetRoute(string route, object? queryStringParametersObjects)
    {
        if (queryStringParametersObjects is null)
            return route;
        
        var parametersJson = JsonSerializer.Serialize(queryStringParametersObjects);
        var parametersDictionary =
            Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(parametersJson);
        
        // Adiciona o parametersDictionary na rota
        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }
}