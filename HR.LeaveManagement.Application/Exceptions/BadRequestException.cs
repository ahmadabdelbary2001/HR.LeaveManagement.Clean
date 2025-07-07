using FluentValidation.Results;

namespace HR.LeaveManagement.Application.Exceptions;

public class BadRequestException : Exception
{
    public IReadOnlyList<string> ValidationErrors { get; }

    public BadRequestException(string message) : base(message)
    {
        ValidationErrors = Array.Empty<string>(); // Initialize as empty collection
    }

    public BadRequestException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationErrors = validationResult.Errors
            .Select(e => e.ErrorMessage)
            .ToList()
            .AsReadOnly(); // Convert to read-only collection
    }
}