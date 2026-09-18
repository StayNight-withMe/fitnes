using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Meals.MealStats;

public class MealStatsHandler : IRequestHandler<MealStatsMessage, Result<WorkFlowResponse>>
{
    private const int DaysPerWeek = 7;
    private const int SundayShift = 6;
    private const double FallbackValue = 0;
    private const string DayLabelFormat = "dd.MM.yyyy";
    private const string MonthLabelFormat = "MM.yyyy";
    private const string WeekLabelFormat = "{0:dd.MM} - {1:dd.MM.yyyy}";
    private readonly IMealEntryRepository _mealEntryRepository;
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ILocalizer _localizer;

    public MealStatsHandler(IMealEntryRepository mealEntryRepository, IBaseRepository<User, long> userRepository, ILocalizer localizer)
    {
        _mealEntryRepository = mealEntryRepository;
        _userRepository = userRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(MealStatsMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        var timezoneMinutes = user.TimezoneOffsetMinutes ?? 0;
        var today = DateTime.UtcNow.AddMinutes(timezoneMinutes).Date;
        var (fromLocal, toLocal, periodLabel) = GetPeriod(request.Granularity, request.Offset, today);
        var rows = await _mealEntryRepository.GetAverages(request.ChatId, fromLocal.AddMinutes(-timezoneMinutes), toLocal.AddMinutes(-timezoneMinutes), cancellationToken);
        var templates = GetTemplates();
        var buttonRows = BuildButtonRows(request.Granularity, request.Offset, templates);

        if (rows.Count == 0)
        {
            return Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = string.Format(templates.Empty, periodLabel),
                ButtonRows = buttonRows
            });
        }

        var text = string.Format(
            templates.Card,
            periodLabel,
            (int)rows.Sum(e => e.AvgCalories ?? FallbackValue),
            (int)rows.Sum(e => e.AvgProtein ?? FallbackValue),
            (int)rows.Sum(e => e.AvgFat ?? FallbackValue),
            (int)rows.Sum(e => e.AvgCarbs ?? FallbackValue),
            rows.Count);

        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = text,
            ButtonRows = buttonRows
        });
    }

    private sealed record StatsTemplates(string Card, string Empty, string BtnDay, string BtnWeek, string BtnMonth, string BtnToday, string BtnPrev, string BtnNext, string Current, string BtnBack);

    private StatsTemplates GetTemplates()
    {
        return new StatsTemplates(
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.Card),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.Empty),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnDay),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnWeek),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnMonth),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnToday),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnPrev),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.BtnNext),
            _localizer.GetPhrase(WorkflowStep.Stats, LocalizationKeysConstants.Stats.Current),
            _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack));
    }

    private static (DateTime FromUtc, DateTime ToUtc, string Label) GetPeriod(StatsGranularity granularity, int offset, DateTime today)
    {
        if (granularity is StatsGranularity.Week)
        {
            var monday = today.AddDays(-(((int)today.DayOfWeek + SundayShift) % DaysPerWeek));
            var from = monday.AddDays(offset * DaysPerWeek);
            var to = from.AddDays(DaysPerWeek);
            return (from, to, string.Format(CultureInfo.InvariantCulture, WeekLabelFormat, from, to.AddDays(-1)));
        }

        if (granularity is StatsGranularity.Month)
        {
            var from = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc).AddMonths(offset);
            var to = from.AddMonths(1);
            return (from, to, from.ToString(MonthLabelFormat, CultureInfo.InvariantCulture));
        }

        var dayFrom = today.AddDays(offset);
        return (dayFrom, dayFrom.AddDays(1), dayFrom.ToString(DayLabelFormat, CultureInfo.InvariantCulture));
    }

    private static string GranularityCallback(StatsGranularity granularity, int offset)
    {
        return $"{CallbackPrefixConstants.Stats}{CallbackPrefixConstants.Separator}{granularity.ToString().ToLowerInvariant()}{CallbackPrefixConstants.Separator}{offset}";
    }

    private List<ButtonRow> BuildButtonRows(StatsGranularity granularity, int offset, StatsTemplates templates)
    {
        var dayText = granularity is StatsGranularity.Day ? templates.BtnDay + templates.Current : templates.BtnDay;
        var weekText = granularity is StatsGranularity.Week ? templates.BtnWeek + templates.Current : templates.BtnWeek;
        var monthText = granularity is StatsGranularity.Month ? templates.BtnMonth + templates.Current : templates.BtnMonth;

        var rows = new List<ButtonRow>
        {
            new ButtonRow(
                new ButtonData(dayText, GranularityCallback(StatsGranularity.Day, 0)),
                new ButtonData(weekText, GranularityCallback(StatsGranularity.Week, 0)),
                new ButtonData(monthText, GranularityCallback(StatsGranularity.Month, 0))),
            new ButtonRow(
                new ButtonData(templates.BtnPrev, GranularityCallback(granularity, offset - 1)),
                new ButtonData(templates.BtnToday, GranularityCallback(StatsGranularity.Day, 0)))
        };

        if (offset < 0)
        {
            rows.Add(new ButtonRow(new ButtonData(templates.BtnNext, GranularityCallback(granularity, offset + 1))));
        }

        rows.Add(new ButtonRow(new ButtonData(templates.BtnBack, CallbackPrefixConstants.Back)));

        return rows;
    }
}
