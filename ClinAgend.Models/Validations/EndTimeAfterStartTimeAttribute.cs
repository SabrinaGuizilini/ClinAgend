using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Validations
{
    public class EndTimeAfterStartTimeAttribute : ValidationAttribute
    {
        private readonly string _startTimeProperty;

        public EndTimeAfterStartTimeAttribute(string startTimeProperty)
        {
            _startTimeProperty = startTimeProperty;
            ErrorMessage = "O horário de término deve ser posterior ao horário de início.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endTimeString = value as string;
            var startTimeProperty = validationContext.ObjectType.GetProperty(_startTimeProperty);

            if (startTimeProperty == null)
                return new ValidationResult($"Propriedade '{_startTimeProperty}' não encontrada.");

            var startTimeString = startTimeProperty.GetValue(validationContext.ObjectInstance) as string;

            if (string.IsNullOrWhiteSpace(startTimeString) || string.IsNullOrWhiteSpace(endTimeString))
                return ValidationResult.Success;

            if (TimeSpan.TryParse(startTimeString, out var start) &&
                TimeSpan.TryParse(endTimeString, out var end))
            {
                if (end <= start)
                    return new ValidationResult(ErrorMessage);
            }
            else
            {
                return new ValidationResult("Formato de hora inválido. Use HH:mm.");
            }

            return ValidationResult.Success;
        }
    }
}
