using System.Net.Http.Json;
using System.Text.Json;
using fitnes.Domain.Abstraction.Services;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Fitnes;
using fitnes.Domain.Options;
using fitnes.Infrastructure.Constants;
using fitnes.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace fitnes.Infrastructure.Utils;

public class GeminiCalorieUtils : ICalorieService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeminiCalorieUtils> _logger;

    private const string JsonTypeString = "STRING";
    private const string JsonTypeNumber = "NUMBER";
    private const string JsonTypeObject = "OBJECT";

    private const string FieldDishName = "dish_name";
    private const string FieldConfidenceScore = "confidence_score";
    private const string FieldNutrition = "nutrition";
    private const string FieldCaloriesKcal = "calories_kcal";
    private const string FieldProteinG = "protein_g";
    private const string FieldFatG = "fat_g";
    private const string FieldCarbsG = "carbs_g";
    private const string FieldMin = "min";
    private const string FieldMax = "max";
    private const string FieldServing = "serving";
    private const string FieldValue = "value";
    private const string FieldUnit = "unit";
    private const string FieldDescription = "description";
    private const string FieldAnalysisSummary = "analysis_summary";

    private const int MaxImageDimension = 1280;
    private const int JpegQuality = 85;

    public GeminiCalorieUtils(HttpClient httpClient, IOptions<ApiKeyOptions> apiKey, ILogger<GeminiCalorieUtils> logger)
    {
        _httpClient = httpClient;
        _apiKey = apiKey.Value.ApiKey;
        _logger = logger;
    }

    private byte[] PrepareImage(byte[] imageBytes)
    {
        try
        {
            using var image = Image.Load(imageBytes);

            if (image.Width <= MaxImageDimension && image.Height <= MaxImageDimension)
            {
                return imageBytes;
            }

            int originalWidth = image.Width;
            int originalHeight = image.Height;
            double scale = Math.Min((double)MaxImageDimension / originalWidth, (double)MaxImageDimension / originalHeight);
            int newWidth = (int)(originalWidth * scale);
            int newHeight = (int)(originalHeight * scale);

            image.Mutate(x => x.Resize(newWidth, newHeight));

            using var output = new MemoryStream();
            image.Save(output, new JpegEncoder { Quality = JpegQuality });
            byte[] resized = output.ToArray();

            _logger.LogInformation(
                "Image resized {OriginalWidth}x{OriginalHeight} -> {NewWidth}x{NewHeight}, {OriginalBytes} -> {ResizedBytes} bytes",
                originalWidth, originalHeight, newWidth, newHeight, imageBytes.Length, resized.Length);

            return resized;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Image resize failed, sending original ({ImageBytes} bytes)", imageBytes.Length);
            return imageBytes;
        }
    }

    public async Task<FoodAnalysisResult?> GetCaloriesFromImageAsync(byte[] imageBytes, string? additionalInformation, Language language, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("API Key для Gemini не найден или пуст.");
        }

        imageBytes = PrepareImage(imageBytes);

        _logger.LogInformation("Gemini request started, image size {ImageBytes} bytes", imageBytes.Length);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(GeminiConstants.RequestTimeoutSeconds));
        CancellationToken ct = timeoutCts.Token;

        string requestUrl = $"{GeminiConstants.BaseUrl}:generateContent?key={_apiKey}";
        string base64Image = Convert.ToBase64String(imageBytes);
        string info = additionalInformation ?? string.Empty;

        var responseSchema = new Dictionary<string, object>
        {
            ["type"] = JsonTypeObject,
            ["properties"] = new Dictionary<string, object>
            {
                [FieldDishName] = new { type = JsonTypeString },
                [FieldConfidenceScore] = new { type = JsonTypeNumber },
                [FieldNutrition] = new Dictionary<string, object>
                {
                    ["type"] = JsonTypeObject,
                    ["properties"] = new Dictionary<string, object>
                    {
                        [FieldCaloriesKcal] = new { type = JsonTypeObject, properties = new { min = new { type = JsonTypeNumber }, max = new { type = JsonTypeNumber } }, required = new[] { FieldMin, FieldMax } },
                        [FieldProteinG] = new { type = JsonTypeObject, properties = new { min = new { type = JsonTypeNumber }, max = new { type = JsonTypeNumber } }, required = new[] { FieldMin, FieldMax } },
                        [FieldFatG] = new { type = JsonTypeObject, properties = new { min = new { type = JsonTypeNumber }, max = new { type = JsonTypeNumber } }, required = new[] { FieldMin, FieldMax } },
                        [FieldCarbsG] = new { type = JsonTypeObject, properties = new { min = new { type = JsonTypeNumber }, max = new { type = JsonTypeNumber } }, required = new[] { FieldMin, FieldMax } }
                    },
                    ["required"] = new[] { FieldCaloriesKcal, FieldProteinG, FieldFatG, FieldCarbsG }
                },
                [FieldServing] = new Dictionary<string, object>
                {
                    ["type"] = JsonTypeObject,
                    ["properties"] = new Dictionary<string, object>
                    {
                        [FieldValue] = new { type = JsonTypeNumber },
                        [FieldUnit] = new { type = JsonTypeString },
                        [FieldDescription] = new { type = JsonTypeString }
                    },
                    ["required"] = new[] { FieldValue, FieldUnit }
                },
                [FieldAnalysisSummary] = new { type = JsonTypeString }
            },
            ["required"] = new[] { FieldDishName, FieldNutrition, FieldServing, FieldAnalysisSummary }
        };

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = string.Format(GeminiConstants.PromptTemplate, info, language.ToString()) },
                        new { inline_data = new { mime_type = GeminiConstants.MimeType, data = base64Image } }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = responseSchema
            }
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUrl, requestBody, ct);
        string jsonResponse = await response.Content.ReadAsStringAsync(ct);

        if (response.IsSuccessStatusCode is false)
        {
            _logger.LogError(
                "Ошибка при запросе к Gemini API. StatusCode: {StatusCode}, Response: {ResponseContent}",
                response.StatusCode,
                jsonResponse);
            return null;
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse, jsonOptions);

        string? innerJson = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(innerJson))
        {
            _logger.LogWarning("Gemini API вернул пустой текст в candidates.");
            return null;
        }

        var result = JsonSerializer.Deserialize<FoodAnalysisResult>(innerJson, jsonOptions);
        _logger.LogInformation("Gemini request finished successfully");

        return result;
    }
}
