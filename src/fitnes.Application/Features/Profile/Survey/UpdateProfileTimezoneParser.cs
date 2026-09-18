using fitnes.Domain.Constants.Profile;

namespace fitnes.Application.Features.Profile.Survey;

public static class UpdateProfileTimezoneParser
{
    public static bool TryParse(string input, out int offsetMinutes)
    {
        offsetMinutes = 0;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalized = input.Trim();

        if (normalized.StartsWith(ProfileValidationConstants.Plus))
        {
            normalized = normalized.Substring(1);
        }

        var parts = normalized.Split(ProfileValidationConstants.Colon);

        if (parts.Length > 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var hours))
        {
            return false;
        }

        var minutes = 0;

        if (parts.Length == 2)
        {
            if (!int.TryParse(parts[1], out minutes))
            {
                return false;
            }
        }

        if (!ProfileValidationConstants.TimezoneValidMinuteParts.Contains(minutes))
        {
            return false;
        }

        var sign = input.Trim().StartsWith(ProfileValidationConstants.Minus) ? -1 : 1;
        offsetMinutes = sign * (Math.Abs(hours) * ProfileValidationConstants.MinutesPerHour + minutes);

        return offsetMinutes >= ProfileValidationConstants.TimezoneOffsetMinMinutes && offsetMinutes <= ProfileValidationConstants.TimezoneOffsetMaxMinutes;
    }
}
