using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public static class TranslationService
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://ftapi.pythonanywhere.com")
    };

    public static async Task<TranslationResponse> TranslateAsync(string text, string destinationLanguage, string sourceLanguage = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text must be provided", nameof(text));
        }
        if (string.IsNullOrWhiteSpace(destinationLanguage))
        {
            throw new ArgumentException("Destination language must be provided", nameof(destinationLanguage));
        }

        var query = new StringBuilder("/translate?dl=");
        query.Append(Uri.EscapeDataString(destinationLanguage));
        query.Append("&text=");
        query.Append(Uri.EscapeDataString(text));

        if (!string.IsNullOrWhiteSpace(sourceLanguage))
        {
            query.Append("&sl=");
            query.Append(Uri.EscapeDataString(sourceLanguage));
        }

        try
        {
            using HttpResponseMessage httpResponse = await _httpClient.GetAsync(query.ToString());
            httpResponse.EnsureSuccessStatusCode();

            using Stream stream = await httpResponse.Content.ReadAsStreamAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await JsonSerializer.DeserializeAsync<TranslationResponse>(stream, options, CancellationToken.None);
        }
        catch (Exception)
        {

            throw; 
        }
    }
}