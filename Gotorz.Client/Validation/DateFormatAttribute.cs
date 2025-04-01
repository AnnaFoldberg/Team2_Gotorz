using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Validates that a string represents a date in a specified format.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DateFormatAttribute : ValidationAttribute
{
    private readonly string _expectedFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateFormatAttribute"/> class.
    /// </summary>
    /// <param name="expectedFormat">The expected date format.</param>
    public DateFormatAttribute(string expectedFormat)
    {
        _expectedFormat = expectedFormat;
    }

    /// <summary>
    /// Validates the value against the expected date format.
    /// </summary>
    /// <param name="value">The <c>object</c> to validate.</param>
    /// <param name="validationContext">The <c>ValidationContext</c> for validation.</param>
    /// <returns>The <c>ValidationResult</c> of the validation.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }
        if (value is string str && DateTime.TryParseExact(str, _expectedFormat, null, System.Globalization.DateTimeStyles.None, out _))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult($"Date must be in the format {_expectedFormat}");
    }
}