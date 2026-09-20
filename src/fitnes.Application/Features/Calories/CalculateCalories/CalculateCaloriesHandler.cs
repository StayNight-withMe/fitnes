using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Abstraction.Services;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using fitnes.Domain.Models.Fitnes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fitnes.Application.Features.Calories.CalculateCalories;

public class CalculateCaloriesHandler : IRequestHandler<CalculateCaloriesMessage, Result<WorkFlowResponse>>
{
    private const double AverageDivisor = 2.0;
    private const double FallbackValue = 0;
    private const string ConfidenceFormat = "P0";
    private readonly ICalorieService _calorieService;
    private readonly IBaseRepository<FoodAnalysisResult, Guid> _analysisRepository;
    private readonly ILogger<CalculateCaloriesHandler> _logger;
    private readonly IRequestContext _requestContext;
    private readonly ILocalizer _localizer;
    public CalculateCaloriesHandler(ICalorieService calorieService, IBaseRepository<FoodAnalysisResult, Guid> analysisRepository, ILogger<CalculateCaloriesHandler> logger, IRequestContext requestContext, ILocalizer localizer)
    {
        _calorieService = calorieService;
        _analysisRepository = analysisRepository;
        _logger = logger;
        _requestContext = requestContext;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(CalculateCaloriesMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calorie analysis started, image size {ImageBytes} bytes", request.ImageBytes.Length);
        var result = await _calorieService.GetCaloriesFromImageAsync(request.ImageBytes, request.AdditionalInfo, _requestContext.Session.Language, cancellationToken);

        if (result is null)
        {
            _logger.LogWarning("Calorie analysis returned null (Gemini empty/error response)");
            return Result.Failure<WorkFlowResponse>(Errors.InternalError);
        }

        _logger.LogInformation("Calorie analysis finished, dish={Dish}", result.Dish_Name);

        var created = await TrySaveHistory(result, cancellationToken);
        var templates = GetTemplates();
        var buttonRows = BuildButtonRows(result, created?.Id, templates);

        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = result.Analysis_Summary ?? templates.NoAnalysis,
            ButtonRows = buttonRows
        });
    }

    private sealed record AnalysisTemplates(string UnknownDish, string NoAnalysis, string DefaultUnit, string DefaultServingDescription, string Dish, string Accuracy, string Serving, string Calories, string Protein, string Fat, string Carbs, string BtnSave);

    private AnalysisTemplates GetTemplates()
    {
        return new AnalysisTemplates(
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.UnknownDish),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.NoAnalysis),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.DefaultUnit),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.DefaultServingDescription),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Dish),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Accuracy),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Serving),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Calories),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Protein),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Fat),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Carbs),
            _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.BtnSave));
    }

    private static double CalculateAverage(RangeValue? range)
    {
        return ((range?.Min ?? FallbackValue) + (range?.Max ?? FallbackValue)) / AverageDivisor;
    }

    private async Task<FoodAnalysisResult?> TrySaveHistory(FoodAnalysisResult result, CancellationToken cancellationToken)
    {
        if (CalculateAverage(result.Nutrition?.Calories_Kcal) <= FallbackValue)
        {
            return null;
        }

        return await _analysisRepository.Create(result, cancellationToken);
    }

    private List<ButtonRow> BuildButtonRows(FoodAnalysisResult result, Guid? analysisId, AnalysisTemplates templates)
    {
        var dishName = result.Dish_Name ?? templates.UnknownDish;
        var dishText = string.Format(templates.Dish, dishName);
        var confidenceText = string.Format(templates.Accuracy, result.Confidence_Score.ToString(ConfidenceFormat, CultureInfo.InvariantCulture));
        var servingUnit = result.Serving?.Unit ?? templates.DefaultUnit;
        var servingDescription = result.Serving?.Description ?? templates.DefaultServingDescription;
        var servingValue = (result.Serving?.Value ?? FallbackValue).ToString(CultureInfo.InvariantCulture);
        var servingDetail = string.Format(templates.Serving, $"{servingValue} {servingUnit} ({servingDescription})");
        var caloriesMin = (result.Nutrition?.Calories_Kcal?.Min ?? FallbackValue).ToString(CultureInfo.InvariantCulture);
        var caloriesMax = (result.Nutrition?.Calories_Kcal?.Max ?? FallbackValue).ToString(CultureInfo.InvariantCulture);
        var caloriesText = string.Format(templates.Calories, (int)CalculateAverage(result.Nutrition?.Calories_Kcal), caloriesMin, caloriesMax);
        var proteinText = string.Format(templates.Protein, (int)CalculateAverage(result.Nutrition?.Protein_g));
        var fatText = string.Format(templates.Fat, (int)CalculateAverage(result.Nutrition?.Fat_g));
        var carbsText = string.Format(templates.Carbs, (int)CalculateAverage(result.Nutrition?.Carbs_g));

        var rows = new List<ButtonRow>();

        if (analysisId is Guid id)
        {
            rows.Add(new ButtonRow(new ButtonData(templates.BtnSave, $"{CallbackPrefixConstants.MealSave}{CallbackPrefixConstants.Separator}{id}")));
        }

        rows.Add(new ButtonRow(new ButtonData(dishText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(confidenceText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(servingDetail, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(caloriesText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(proteinText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(fatText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(carbsText, CallbackPrefixConstants.NoAction)));
        rows.Add(new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back)));

        return rows;
    }
}
