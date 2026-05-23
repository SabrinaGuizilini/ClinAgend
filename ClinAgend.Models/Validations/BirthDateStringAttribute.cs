using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Validations
{
    public class BirthDateStringAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public BirthDateStringAttribute(int minAge, int maxAge)
        {
            _minAge = minAge;
            _maxAge = maxAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str && DateTime.TryParse(str, out var date))
            {
                var age = DateTime.Today.Year - date.Year;
                if (date > DateTime.Today.AddYears(-age)) age--;

                if (age < _minAge || age > _maxAge)
                    return new ValidationResult(ErrorMessage ?? $"A idade deve estar entre {_minAge} e {_maxAge} anos.");

                return ValidationResult.Success;
            }

            return new ValidationResult("Data inválida.");
        }
    }
}
