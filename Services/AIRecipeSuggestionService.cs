using System.Text;
using System.Text.Json;

namespace CakeProduction.Services
{
    public class AIRecipeSuggestionService
    {
        private readonly HttpClient _httpClient;
        private const string apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxx";
        private const string apiUrl = "https://api.openai.com/v1/chat/completions";

        public AIRecipeSuggestionService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey.Trim()}");
        }

        public async Task<string> GetAIResponse(List<string> ingredientNames)
        {
            var ingredients = string.Join(", ", ingredientNames);
            var prompt = $"Suggest 3 creative cake recipes using: {ingredients}. " +
                $"Provide a title and a short description for each.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                temperature = 0.7,
                messages = new[]
                {
        new { role = "system", content = "You are a helpful assistant." },
        new { role = "user", content = prompt }
    }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error calling OpenAI API: {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}
