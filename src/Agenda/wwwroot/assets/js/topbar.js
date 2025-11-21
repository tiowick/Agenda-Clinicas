
    $("#silenciar-som").on("click", function () {
        // Obtenha o elemento de áudio
        var audioElement = document.getElementById("som_notificacao");
        audioElement.pause();
        audioElement.currentTime = 0;
        $("#silenciar-som").css("display", "none")
    });

    $(document).ready(function () {
        $(".switch-escolha").trigger('change');
    })

    $(".switch-escolha").on("change", function (e) {
        console.log($(this).prop("checked"))
        //gravar no bd
        var temaEscuro = $(this).prop("checked");
        $.ajax({
            url: '/Tema/AlterarTema',
            data: { modoEscuro: temaEscuro },
            type: 'POST',
            beforeSend: function () {

            },
            success: function (data) {

                if (!temaEscuro) {
                    $("#light-mode-switch").prop("checked", true);
                    $("#light-mode-switch").trigger("change");
                } else {
                    $("#dark-mode-switch").prop("checked", true);
                    $("#dark-mode-switch").trigger("change");
                }

            }, error: function (data) {
                $(this).prop("checked", !temaEscuro);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Falha ao alterar o tema, tente novamente!'
                })
            }, complete: function (data) {

            }
        });
    });
