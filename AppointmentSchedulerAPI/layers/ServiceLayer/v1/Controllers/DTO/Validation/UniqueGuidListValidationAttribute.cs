using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class UniqueGuidListValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<Guid> list)
        {
            return new ValidationResult("The value is not a valid list of GUIDs.");
        }

        var duplicate = list.GroupBy(uuid => uuid)
                            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate != null)
        {
            return new ValidationResult($"Duplicate UUID detected: {duplicate.Key}");
        }

        return ValidationResult.Success;
    }
}
