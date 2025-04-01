using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DateFormatAttribute : ValidationAttribute
{
    private readonly string _expectedFormat;

    public DateFormatAttribute(string expectedFormat)
    {
        _expectedFormat = expectedFormat;
    }
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