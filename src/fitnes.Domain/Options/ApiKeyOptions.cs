using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes.Domain.Options;

public class ApiKeyOptions
{
    public const string SectionName = "Gemini";
    public string ApiKey { get; set; } = string.Empty;
}
