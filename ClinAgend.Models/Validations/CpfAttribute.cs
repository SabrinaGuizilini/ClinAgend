using System.ComponentModel.DataAnnotations;

namespace ClinAgend.Models.Validations
{
    public class CpfAttribute : ValidationAttribute
    {
        public CpfAttribute() : base("CPF inválido.") { }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            var cpf = new string(value.ToString().Where(char.IsDigit).ToArray());

            if (cpf.Length != 11) return false;

            if (cpf.Distinct().Count() == 1) return false;

            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCpf += digito1;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith($"{digito1}{digito2}");
        }
    }
}
