using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Validations
{
    public class CnpjAttribute : ValidationAttribute
    {
        public CnpjAttribute() : base("CNPJ inválido.") { }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            var cnpj = new string(value.ToString().Where(char.IsDigit).ToArray());

            if (cnpj.Length != 14) return false;

            // Evita CNPJ com todos os dígitos iguais
            if (cnpj.Distinct().Count() == 1) return false;

            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicadores1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCnpj += digito1;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicadores2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith($"{digito1}{digito2}");
        }
    }
}
