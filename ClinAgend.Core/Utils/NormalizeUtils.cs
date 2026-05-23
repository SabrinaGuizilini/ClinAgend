using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinAgend.Core.Utils
{
    internal static class NormalizeUtils
    {
        /// <summary>
        /// Remove todos os caracteres que não forem dígitos.
        /// </summary>
        internal static string NormalizeDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return new string(input.Where(char.IsDigit).ToArray());
        }

    }
}
