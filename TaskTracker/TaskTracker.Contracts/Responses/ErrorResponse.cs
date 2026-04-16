using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Contracts.Responses;

public class ErrorResponse
{
    public string TranslationKey { get; set; } = string.Empty;
    public string? Details { get; set; }
}
