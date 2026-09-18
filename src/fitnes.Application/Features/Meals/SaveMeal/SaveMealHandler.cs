using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using fitnes.Domain.Models.Fitnes;
using MediatR;

namespace fitnes.Application.Features.Meals.SaveMeal;

public class SaveMealHandler : IRequestHandler<SaveMealMessage, Result<WorkFlowResponse>>
{
    private readonly IMealEntryRepository _mealEntryRepository;
    private readonly IBaseRepository<FoodAnalysisResult, Guid> _analysisRepository;
    private readonly ILocalizer _localizer;

    public SaveMealHandler(IMealEntryRepository mealEntryRepository, IBaseRepository<FoodAnalysisResult, Guid> analysisRepository, ILocalizer localizer)
    {
        _mealEntryRepository = mealEntryRepository;
        _analysisRepository = analysisRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(SaveMealMessage request, CancellationToken cancellationToken)
    {
        var analysis = await _analysisRepository.GetById(request.AnalysisResultId, cancellationToken);

        if (analysis is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.InternalError);
        }

        var btnBack = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

        if (await _mealEntryRepository.ExistsByAnalysisResultId(request.AnalysisResultId, cancellationToken))
        {
            var alreadySavedText = _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.AlreadySaved);
            return Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = alreadySavedText,
                ButtonRows = new[]
                {
                    new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
                }
            });
        }

        await _mealEntryRepository.Create(new MealEntry
        {
            UserId = request.ChatId,
            ConsumedAt = DateTime.UtcNow,
            Source = EntrySource.AiAnalysis,
            AnalysisResultId = request.AnalysisResultId
        }, cancellationToken);

        var savedText = _localizer.GetPhrase(WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.Saved);
        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = savedText,
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
            }
        });
    }
}
