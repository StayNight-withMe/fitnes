using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using fitnes.Domain.Utils;
using FluentValidation;
using MediatR;

namespace fitnes.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IRequestContext _context;
    private readonly ILocalizer _localizer;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IRequestContext context, ILocalizer localizer)
    {
        _validators = validators;
        _context = context;
        _localizer = localizer;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_context.Session is null)
        {
            return await next();
        }

        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(result => result.Errors).ToList();

            if (failures.Count > 0)
            {
                return BuildFailure(string.Join("\n", failures.Select(failure => failure.ErrorMessage)));
            }
        }

        return await next();
    }

    private TResponse BuildFailure(string text)
    {
        if (typeof(TResponse) == typeof(Result<WorkFlowResponse>))
        {
            var response = Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = text,
                ButtonRows = new[] { ResolveButtonRow() }
            });

            return (TResponse)(object)response;
        }

        return ResultFactory.Failure<TResponse>(Errors.ValidationError, [text]);
    }

    private ButtonRow ResolveButtonRow()
    {
        if (_context.Session?.State is WorkflowStep state && (state.IsProfileStep() || state is WorkflowStep.AwaitingGoalWeight || state is WorkflowStep.AwaitingWeightEdit))
        {
            var cancelText = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel);
            return new ButtonRow(new ButtonData(cancelText, CallbackPrefixConstants.Cancel));
        }

        var backText = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);
        return new ButtonRow(new ButtonData(backText, CallbackPrefixConstants.Back));
    }
}
