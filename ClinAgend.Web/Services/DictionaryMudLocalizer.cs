using Microsoft.Extensions.Localization;
using MudBlazor;

namespace ClinAgend.Web.Services
{
    public class DictionaryMudLocalizer : MudLocalizer
    {
        // Dicionário para armazenar as traduções
        private readonly Dictionary<string, string> _localization;

        public DictionaryMudLocalizer()
        {
            _localization = new Dictionary<string, string>
            {
                // MudDataGrid / Tabela de Dados:
                { "MudDataGridPager_RowsPerPage", "Linhas por página:" },
                { "MudDataGrid_Unsort", "Remover Ordenação" },
                { "MudDataGridPager_InfoFormat", "{0}-{1} de {2}" },

            };
        }

        // O indexador é o método que o MudBlazor chama para obter a tradução
        public override LocalizedString this[string key]
        {
            get
            {
                // Tenta obter o valor no dicionário
                if (_localization.TryGetValue(key, out var localizedString))
                {
                    // Retorna a string traduzida
                    return new LocalizedString(key, localizedString);
                }

                // Se a chave não for encontrada no nosso dicionário, 
                // retorna a chave original (indicando que a tradução não foi encontrada)
                return new LocalizedString(key, key, resourceNotFound: true);
            }
        }
    }
}
