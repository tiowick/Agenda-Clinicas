using Agenda.Dominio.Entidades.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Agenda.Infra.Padronizar.Components
{
    [DebuggerStepThrough]
    public static class DataSelect2OptionsData
    {
        public static IEnumerable<DataSelect2DTO> GetOptionsSelect2(this IEnumerable<DataSelect2DTO> options, int? pagina = 1)
        {
            try
            {
                // 1. Validação inicial rápida
                if (options == null || !options.Any())
                    return new List<DataSelect2DTO>();

                var resultList = new List<DataSelect2DTO>();

                // 2. Verifica se existem grupos configurados nos dados
                var grupos = options
                    .Where(o => !string.IsNullOrEmpty(o.grpoption))
                    .GroupBy(g => new { g.grpoption, g.label })
                    .ToArray();

                bool temGrupo = grupos.Any();
                if (pagina.GetValueOrDefault(1) == 1)
                {
                    resultList.Add(new DataSelect2DTO
                    {
                        id = "",
                        text = "⭐ (Selecione uma opção)",
                        // element = "HTMLOptionElement" // REMOVIDO: Isso causava o erro "Searching..."
                    });
                }

                // 4. Lógica para Lista COM Grupos
                if (temGrupo)
                {
                    foreach (var grupo in grupos)
                    {
                        var groupLabel = (grupo.Key.label?.Trim() ?? "").ToUpper(CultureInfo.CurrentCulture);
                        var groupOption = (grupo.Key.grpoption?.Trim() ?? "").ToUpper(CultureInfo.CurrentCulture);

                        var itemGrupo = new DataSelect2DTO
                        {
                            // element = "HTMLOptGroupElement", // REMOVIDO: Desnecessário e perigoso
                            label = $"📂 {groupLabel}",
                            text = $"📌 {groupOption} (Página: {pagina.GetValueOrDefault(1)})",
                            id = groupOption
                        };

                        // Filtra os filhos e GARANTE que não levamos sujeira
                        var children = options
                            .Where(o => o.grpoption?.Trim() == grupo.Key.grpoption?.Trim())
                            .Select(c => new DataSelect2DTO
                            {
                                id = c.id,
                                text = c.text,
                                label = c.label,
                                grpoption = c.grpoption,
                                children = c.children
                            })
                            .ToList();

                        itemGrupo.children = children;
                        resultList.Add(itemGrupo);
                    }
                }
                // 5. Lógica para Lista SEM Grupos (Simples)
                else
                {
                    foreach (var item in options)
                    {
                        resultList.Add(new DataSelect2DTO
                        {
                            id = item.id,
                            text = $"📌 {item.text}",
                            label = item.label,
                            grpoption = item.grpoption,
                            children = item.children,
                        });
                    }
                }

                return resultList;
            }
            catch (Exception)
            {
                // Em caso de erro grave, retorna lista vazia para não quebrar a tela
                return new List<DataSelect2DTO>();
            }
        }
    }
}