using Agenda.Dominio.Entidades.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Infra.Padronizar.Components
{
    [DebuggerStepThrough]
    public static class DataSelect2OptionsData
    {
        public static IEnumerable<DataSelect2DTO> GetOptionsSelect2(this IEnumerable<DataSelect2DTO> otions, int? pagina = 1)
        {
            try
            {
                if (otions == null)
                    return new List<DataSelect2DTO>();

                if (!otions.Any())
                    return new List<DataSelect2DTO>();

                var _grupo = false;
                var _selectOptions = new List<DataSelect2DTO>();
                var _parent = otions
                    .Where(o => !string.IsNullOrEmpty(o.grpoption))
                    .GroupBy(g => new { g.grpoption, g.label })
                    .ToArray();

                if (_parent == null)
                    return otions;

                var _selecione1 = new DataSelect2DTO
                {
                    id = "null",
                    text = "⭐ (Selecione uma opção)",
                    element = "HTMLOptionElement"
                };

                if ((pagina ?? 1) == 1)
                    _selectOptions.Add(_selecione1);

                foreach (var item in _parent)
                {
                    var _item = new DataSelect2DTO
                    {
                        element = "HTMLOptGroupElement",
                        label = "📂 " + (item?.Key?.label?.Trim() ?? "").ToUpper(culture: System.Globalization.CultureInfo.CurrentCulture),
                        text = "📌 " + (item?.Key?.grpoption?.Trim() ?? "").ToUpper(culture: System.Globalization.CultureInfo.CurrentCulture) + string.Format(" (Página: {0})", pagina ?? 1),
                        id = (item?.Key?.grpoption?.Trim() ?? "").ToUpper(culture: System.Globalization.CultureInfo.CurrentCulture),
                    };
                    var _childrem = otions.Where(o => o.grpoption == item?.Key?.grpoption.Trim()).ToList();
                    _item.children = _childrem;

                    _selectOptions.Add(_item);
                    _grupo = true;
                }

                if (!_grupo)
                    foreach (var item in otions)
                    {
                        var _item = new DataSelect2DTO
                        {
                            children = item.children,
                            element = item.element,
                            grpoption = item.grpoption,
                            id = item.id,
                            label = item.label,
                            text = "📌 " + item.text,
                        };
                        _selectOptions.Add(_item);
                    }

                if (_selectOptions.Any())
                    return _selectOptions;

                return otions;

            }
            catch { return new List<DataSelect2DTO>(); }
        }
    }
}
