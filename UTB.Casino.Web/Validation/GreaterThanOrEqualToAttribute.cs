using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UTB.Casino.Web.Validation;

//Own validation attribute: value must not be lower than another property (e.g. MaxBet >= MinBet)
[AttributeUsage(AttributeTargets.Property)]
public class GreaterThanOrEqualToAttribute : ValidationAttribute, IClientModelValidator
{
    public string OtherProperty { get; }

    public GreaterThanOrEqualToAttribute(string otherProperty)
    {
        OtherProperty = otherProperty;
        ErrorMessage = "{0} must be greater than or equal to " + otherProperty + ".";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(OtherProperty);
        if (property == null) return new ValidationResult($"Unknown property {OtherProperty}.");

        var other = property.GetValue(validationContext.ObjectInstance);
        if (value is IComparable current && other != null && current.CompareTo(other) < 0)
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-greaterthanorequalto", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        context.Attributes.TryAdd("data-val-greaterthanorequalto-other", OtherProperty);
    }
}
