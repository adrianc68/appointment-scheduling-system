using System.Collections;
using System.ComponentModel.DataAnnotations;

public class UniqueUuidListValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable list)
        {
            return new ValidationResult("The value is not a valid list.");
        }

        var uuids = list.Cast<object>()
                        .Select(item => item?.GetType().GetProperty("Uuid")?.GetValue(item, null))
                        .Where(uuid => uuid != null)
                        .ToList();

        var duplicate = uuids.GroupBy(uuid => uuid)
                             .FirstOrDefault(group => group.Count() > 1);

        if (duplicate != null)
        {
            return new ValidationResult($"Duplicate Uuid detected: {duplicate.Key}");
        }

        return ValidationResult.Success;
    }
}
