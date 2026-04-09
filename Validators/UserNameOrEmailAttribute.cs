using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LevelUpLifeBackend.Validators;

// Scan if attribute is a username or an email and validate the corresponding format.
// Reusable attribute: detects if the value is an email or a username

public class UserNameOrEmailAttribute : ValidationAttribute
{
    //Standard email regex:
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    // Username: letters, numbers, underscore, and dot. Between 3 and 50 characters.
    private static readonly Regex UserNameRegex = new(
        @"^[a-zA-Z0-9._]{3,50}$",
        RegexOptions.Compiled
    );

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var input = value as string;

        if (string.IsNullOrWhiteSpace(input))
            return new ValidationResult("El usuario o email es requerido.");

        // If it contains '@', we assume it's an email and validate accordingly.
        if (input.Contains('@'))
        {
            if (!EmailRegex.IsMatch(input))
                return new ValidationResult("El formato del email no es válido.");
        }
        else
        {
            if (!UserNameRegex.IsMatch(input))
                return new ValidationResult("El usuario solo puede contener letras, números, puntos y guiones bajos (3-50 caracteres).");
        }

        return ValidationResult.Success;
    }
}
