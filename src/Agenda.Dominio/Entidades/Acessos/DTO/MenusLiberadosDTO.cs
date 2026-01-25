using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Agenda.Dominio.Entidades.Acessos.DTO
{
    public class MenusLiberadosDTO
    {
        public long ID { get; set; } = default!;
        public long Codigo { get; set; } = default!;
        public long? CodigoPai { get; set; } = default!;
        public string Descricao { get; set; } = default!;

        // PROPRIEDADES PARA MAPEAR O BANCO
        public string Area { get; set; } = default!;
        public string Controller { get; set; } = default!;
        public string Metodo { get; set; } = default!;

        // Mantemos essa caso alguma query complexa já traga pronta
        public string RotaController { get; set; } = default!;

        public string Icone { get; set; } = default!;
        public IList<MenusLiberadosDTO> Filhos { get; set; } = default!;

        public override string ToString()
        {
            // --- MONTAGEM DA ROTA ---
            string _rotaFinal = "";

            if (!string.IsNullOrEmpty(RotaController))
            {
                _rotaFinal = RotaController;
            }
            else
            {
                var _area = !string.IsNullOrEmpty(Area) ? $"/{Area}" : "";
                var _controller = !string.IsNullOrEmpty(Controller) ? $"/{Controller}" : "";
                var _metodo = !string.IsNullOrEmpty(Metodo) ? $"/{Metodo}" : "";

                _rotaFinal = $"{_area}{_controller}{_metodo}";
            }

            // Limpeza básica
            _rotaFinal = _rotaFinal.Trim().ToLower(culture: CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(_rotaFinal)) _rotaFinal = "javascript: void(0);";

            // Verificação de Filhos (mesmo vindo vazio do banco, mantemos a lógica caso mude no futuro)
            var _filhos = Filhos ?? new List<MenusLiberadosDTO>();
            bool temFilhos = _filhos.Any();
            bool ehRaiz = (CodigoPai == null || CodigoPai == 0);

            var sb = new StringBuilder();

            // --- [MODIFICAÇÃO IMPORTANTE] ---
            // Adicionamos data-id e data-pai para o JavaScript conseguir ler a hierarquia
            sb.AppendLine($"<li data-id=\"{Codigo}\" data-pai=\"{CodigoPai ?? 0}\">");

            // Lógica de classes
            string classeLink = "";
            string href = "";

            if (ehRaiz)
            {
                // Se tiver filhos JÁ na lista (backend), usa has-arrow. 
                // Se não, o JS vai adicionar essa classe depois.
                classeLink = temFilhos ? "has-arrow waves-effect" : "waves-effect";
                href = temFilhos ? "javascript: void(0);" : _rotaFinal;
            }
            else
            {
                classeLink = temFilhos ? "has-arrow" : "";
                href = temFilhos ? "javascript: void(0);" : _rotaFinal;
            }

            // --- CONTEÚDO DO LINK ---
            sb.Append($"<a href=\"{href}\" class=\"{classeLink}\" key=\"t-{Codigo}\">");

            if (!string.IsNullOrEmpty(Icone))
            {
                sb.Append($"<i class=\"{Icone}\"></i>");
            }

            sb.Append($"<span key=\"t-span-{Codigo}\">{Descricao}</span>");
            sb.AppendLine("</a>");

            // Se vier filhos do backend (futuro), renderiza aqui
            if (temFilhos)
            {
                sb.AppendLine("<ul class=\"sub-menu\" aria-expanded=\"false\">");
                foreach (var filho in _filhos)
                {
                    sb.Append(filho.ToString());
                }
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("</li>");
            return sb.ToString();
        }
    }
}