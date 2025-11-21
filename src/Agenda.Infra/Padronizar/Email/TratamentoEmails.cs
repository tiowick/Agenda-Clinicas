using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Agenda.Infra.Padronizar.Email
{
    [DebuggerStepThrough]
    public static class TratamentoEmails
    {
        /// <summary>
        /// Confirma a validade de um email
        /// </summary>
        /// <param name="enderecoEmail">Email a ser validado</param>
        /// <returns>Retorna True se o email for valido</returns>
        public static bool ValidaEnderecoEmail(this string enderecoEmail)
        {
            try
            {
                if(string.IsNullOrEmpty(enderecoEmail))
                    return false;

                string texto_Validar = enderecoEmail;
                Regex expressaoRegex = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");
                if (expressaoRegex.IsMatch(texto_Validar))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
