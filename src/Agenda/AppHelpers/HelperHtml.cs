using Agenda.AbstractFactory;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;

namespace Agenda.AppHelpers
{
    public static class HelperHtml
    {
        /// <summary>
        /// HelperScriptValidadorFormulario
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="formName"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static HtmlString ValidadeFormsExtensions(this IHtmlHelper helper, string formName, IDictionary<string, IEnumerable<ValidadorFormulario>> keys)
        {
            var _html = new StringBuilder();
            var _identy = "            ";

            var _required = _identy + "            required: true";
            var _requiredMsg = _identy + "            required: 'Este campo é obrigatório'";
            var regras = _identy + "    rules: {";
            var mensagens = _identy + "    messages: {";

            formName = formName?.Replace("#", "") ?? "";
            _html.AppendLine();
            _html.AppendLine("var select2label;");
            _html.AppendLine(_identy + "var $checkoutForm = $('#" + formName + "').validate({");

            var _AllRules = new StringBuilder();
            var _AllMensagens = new StringBuilder();

            if (keys != null)
                foreach (var campos in keys)
                {
                    _AllRules.AppendLine(_identy + "        " + campos.Key + ": {");
                    _AllMensagens.AppendLine(_identy + "        " + campos.Key + ": {");
                    foreach (var item in campos.Value)
                    {
                        if (item.Regra.ToLower(culture: CultureInfo.CurrentCulture) == "regex")
                            _AllRules.AppendLine(_identy + "            " + item.Regra + ": '" + item.Condicao + "', ");
                        else
                            _AllRules.AppendLine(_identy + "            " + item.Regra + ": " + item.Condicao + ", ");
                        _AllMensagens.AppendLine(_identy + "            " + item.Regra + ": '" + item.Mensagem + "', ");
                    }
                    _AllRules.AppendLine(_required);
                    _AllMensagens.AppendLine(_requiredMsg);

                    _AllRules.AppendLine(_identy + "        },");
                    _AllMensagens.AppendLine(_identy + "        },");
                }

            _html.AppendLine(regras);
            _html.AppendLine(_AllRules.ToString());
            _html.AppendLine(_identy + "    },");
            _html.AppendLine(mensagens);
            _html.AppendLine(_AllMensagens.ToString());
            _html.AppendLine(_identy + "    },");

            _html.AppendLine("    errorPlacement: function(label, element) {");
            _html.AppendLine("        if (element.hasClass('select')) {");
            _html.AppendLine("            label.insertAfter(element.next('.select2-container')).addClass('mt-2 text-danger');");
            _html.AppendLine("            select2label = label");
            _html.AppendLine("        } else {");
            _html.AppendLine("            label.addClass('mt-2 text-danger');");
            _html.AppendLine("            label.insertAfter(element);");
            _html.AppendLine("        }");
            _html.AppendLine("    },");
            _html.AppendLine("    highlight: function(element) {");
            _html.AppendLine("        $(element).parent().addClass('is-invalid')");
            _html.AppendLine("        $(element).addClass('form-control-danger')");
            _html.AppendLine("    },");
            _html.AppendLine("    success: function(label, element) {");
            _html.AppendLine("        $(element).parent().removeClass('is-invalid')");
            _html.AppendLine("        $(element).removeClass('form-control-danger')");
            _html.AppendLine("        label.remove();");
            _html.AppendLine("    },");
            _html.AppendLine("    submitHandler: function(form) {");
            _html.AppendLine("    ");
            _html.AppendLine("    },");

            //_html.AppendLine(_identy + "    errorPlacement: function (error, element) {");
            //_html.AppendLine(_identy + "        error.insertAfter(element.parent());");
            //_html.AppendLine(_identy + "    }");


            _html.AppendLine(_identy + "});");

            return new HtmlString(_html.ToString());
        }
    }
}
