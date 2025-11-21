$(".criarNovoChamado").click(function () {
    $("#modal").load("/Chamados/Create?paginaInterna=0", function () {
        $("#modal").modal('show');
    })
}); 