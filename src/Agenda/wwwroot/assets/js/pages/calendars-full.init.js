!function (w) {
    "use strict"; function e() { } e.prototype.init = function () {
        // Seletores para a modal (mantidos)
        var l = w("#event-modal"), t = w("#modal-title"), a = w("#form-event"), i = null, s = null, o = document.getElementsByClassName("needs-validation"), i = null, s = null, n = document.getElementById("locale-selector"), e = new Date, r = e.getDate(), d = e.getMonth(), c = e.getFullYear();

        // REMOVIDO: A linha "new FullCalendar.Draggable(...)" que causava o primeiro erro foi removida.

        // REMOVIDO: O array de eventos estáticos "var v=[...]" foi removido.

        var u = document.getElementById("calendar");

        // Função para abrir a modal (mantida)
        function m(e) { l.modal("show"), a.removeClass("was-validated"), a[0].reset(), w("#event-title").val(), w("#event-category").val(), t.text("Add Event"), s = e }

        // Inicialização do Calendário
        var g = new FullCalendar.Calendar(u, {
            editable: !0,
            droppable: !0,
            selectable: !0,
            initialView: "dayGridMonth",
            themeSystem: "bootstrap",
            weekNumbers: !0,
            locale: "pt-br", // Alterado para Português
            headerToolbar: { left: "prev,next today", right: "dayGridMonth,timeGridWeek,timeGridDay,listMonth", center: "title" },
            dayMaxEventRows: !0,
            views: { timeGrid: { dayMaxEventRows: 5 } },

            // Função de clique em um evento existente (mantida)
            eventClick: function (e) { l.modal("show"), a[0].reset(), i = e.event, w("#event-title").val(i.title), w("#event-category").val(i.classNames[0]), s = null, t.text("Edit Event"), s = null },

            // Função de clique em um dia (mantida)
            dateClick: function (e) { m(e) },

            // =================================================================
            // CARREGAMENTO DE EVENTOS (AJAX)
            // =================================================================
            events: function (fetchInfo, successCallback, failureCallback) {

                var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

                if (!tokenInput) {
                    console.error("ERRO CRÍTICO: Não foi possível encontrar o input '__RequestVerificationToken' na página.");
                    failureCallback(new Error("Token de segurança não encontrado."));
                    return;
                }

                // 2. PREPARAR OS PARÂMETROS COMO "Form Data" (URLSearchParams)
                var formData = new URLSearchParams();
                formData.append('search[value]', '');
                formData.append('start', 0);
                formData.append('length', 5000); // <-- AQUI ESTÁ O 5000
                formData.append('draw', 1);

                // 3. FAZER A CHAMADA AJAX (FETCH)
                fetch('/Agenda/Calendario/CarregarGridEnventosCalendario', {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': tokenInput.value,
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: formData.toString()
                })
                    .then(function (response) {
                        if (!response.ok) {
                            throw new Error('Erro na rede: ' + response.statusText);
                        }
                        return response.json();
                    })
                    .then(function (jsonResponse) {
                        if (!jsonResponse.data) {
                            console.error("ERRO: A resposta do JSON não contém uma propriedade 'data'. Resposta recebida:", jsonResponse);
                            failureCallback(new Error("Formato de dados inesperado."));
                            return;
                        }

                        // ===================================
                        // CORREÇÃO DO MAPEAMENTO (MAIÚSCULAS)
                        // ===================================
                        var events = jsonResponse.data.map(function (item) {

                            var corFundo = '#78909C';
                            var corBorda = '#78909C';
                            // Use 'item.AgCodStatus' (PascalCase)
                            switch (item.AgCodStatus) {
                                case 1: corFundo = '#4CAF50'; corBorda = '#4CAF50'; break;
                                case 2: corFundo = '#FFCA28'; corBorda = '#FFCA28'; break;
                                case 3: corFundo = '#FF9800'; corBorda = '#FF9800'; break;
                                case 4: corFundo = '#42A5F5'; corBorda = '#42A5F5'; break;
                                case 5: corFundo = '#EF5350'; corBorda = '#EF5350'; break;
                                case 6: corFundo = '#B0BEC5'; corBorda = '#B0BEC5'; break;
                                case 7: corFundo = '#2196F3'; corBorda = '#2196F3'; break;
                            }

                            var dataFim = null;
                            // Use 'item.AgDatahorafim' e 'item.DataHora' (PascalCase)
                            if (item.AgDatahorafim) {
                                var dataInicioStr = new Date(item.DataHora).toDateString();
                                dataFim = new Date(dataInicioStr + ' ' + item.AgDatahorafim);
                            }

                            return {
                                id: item.ID,                  // Use item.ID
                                title: item.Descricao,      // Use item.Descricao
                                start: item.DataHora,         // Use item.DataHora
                                end: dataFim,
                                backgroundColor: corFundo,
                                borderColor: corBorda
                            };
                        });

                        // 5. ENVIAR OS EVENTOS MAPEADOS PARA O CALENDÁRIO
                        successCallback(events);
                    })
                    .catch(function (error) {
                        console.error("Erro ao buscar eventos:", error);
                        failureCallback(error);
                    });
            }
            // =================================================================
            // FIM DA ALTERAÇÃO
            // =================================================================
        });

        // Renderiza o calendário (mantido)
        g.render();

        // ===================================
        // CORREÇÃO DO 'appendChild' (LINHAS COMENTADAS)
        // ===================================
        // Lógica do seletor de idiomas (desativada para corrigir o erro)
        // g.getAvailableLocaleCodes().forEach(function (e) { var t = document.createElement("option"); t.value = e, t.selected = "pt-br" == e, t.innerText = e, n.appendChild(t) });
        // n.addEventListener("change", function () { this.value && g.setOption("locale", this.value) });
        // ===================================


        // Lógica de submit do formulário da modal (mantido)
        w(a).on("submit", function (e) { e.preventDefault(); var t, a = w("#event-title").val(), n = w("#event-category").val(); !1 === o[0].checkValidity() ? (e.preventDefault(), e.stopPropagation(), o[0].classList.add("was-validated")) : (i ? (i.setProp("title", a), i.setProp("classNames", [n])) : (t = { title: a, start: s.date, allDay: s.allDay, className: n }, g.addEvent(t)), l.modal("hide")) });

        // Lógica do botão de deletar (mantido)
        w("#btn-delete-event").on("click", function (e) { i && (i.remove(), i = null, l.modal("hide")) });

        // Lógica do botão "Criar Evento" (mantido)
        w("#btn-new-event").on("click", function (e) { m({ date: new Date, allDay: !0 }) });

    }, w.CalendarPage = new e, w.CalendarPage.Constructor = e
}(window.jQuery), function () { "use strict"; window.jQuery.CalendarPage.init() }();