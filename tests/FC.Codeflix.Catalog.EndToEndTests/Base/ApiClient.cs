using System.Text;
using System.Text.Json;

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

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route)
        where TOutput : class
    {
        // Faz um Get na API
        var response = await _httpClient.GetAsync(route);

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
}