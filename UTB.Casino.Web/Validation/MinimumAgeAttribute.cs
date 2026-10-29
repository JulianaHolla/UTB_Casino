using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UTB.Casino.Web.Validation;

// Own validation attribute: the date of birth must be at least N years ago (server + client side)
[AttributeUsage(AttributeTargets.Property)]
public class MinimumAgeAttribute : ValidationAttribute, IClientModelValidator
{
    public int Years { get; }

    public MinimumAgeAttribute(int years)
    {
        Years = years;
        ErrorMessage = $"You must be at least {years} years old.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime dob) return ValidationResult.Success;   // [Required] handles empty values
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age >= Years ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-minimumage", ErrorMessage ?? "");
        context.Attributes.TryAdd("data-val-minimumage-years", Years.ToString());
    }
}
