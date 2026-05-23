using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ClinAgend.Models.Validations
{
    public class EndDateAfterStartDateAttribute : ValidationAttribute
    {
        private readonly string _startDateProperty;
        private readonly string _dateFormat = "dd/MM/yyyy";

        public EndDateAfterStartDateAttribute(string startDateProperty)
        {
            _startDateProperty = startDateProperty;
            ErrorMessage = "A data final não pode ser anterior à data inicial.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDateStr = value as string;
            var startDateProp = validationContext.ObjectType.GetProperty(_startDateProperty);

            if (startDateProp == null)
                return new ValidationResult($"Propriedade '{_startDateProperty}' não encontrada.");

            var startDateStr = startDateProp.GetValue(validationContext.ObjectInstance) as string;

            // Se algum campo estiver vazio, deixa o [Required] tratar.
            if (string.IsNullOrWhiteSpace(startDateStr) || string.IsNullOrWhiteSpace(endDateStr))
                return ValidationResult.Success;

            // Tenta converter as duas datas no formato dd/MM/yyyy.
            if (!DateTime.TryParseExact(startDateStr, _dateFormat, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out var startDate))
                return new ValidationResult($"Formato inválido para '{_startDateProperty}'. Use o formato dd/MM/yyyy.");

            if (!DateTime.TryParseExact(endDateStr, _dateFormat, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out var endDate))
                return new ValidationResult($"Formato inválido para a data final. Use o formato dd/MM/yyyy.");

            // Verifica se a data final é anterior à inicial.
            if (endDate < startDate)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
