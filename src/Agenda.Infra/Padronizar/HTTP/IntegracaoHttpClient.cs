using Agenda.Infra.Padronizar.Texto;
using System.Diagnostics;
using System.Text;

namespace Agenda.Infra.Padronizar.HTTP
{
    [DebuggerStepThrough]
    public abstract class IntegracaoHttpClient : HttpClient
    {
        public Tuple<int, string> ResponseHTTP { get; protected set; } = default!;
        private IDictionary<string, string> _headers { get; set; } = default!;
        private string _bodyJson { get; set; } = default!;
        public Uri _uriBase { get; set; }

        public IntegracaoHttpClient(string uriBase)
        {
            _uriBase = new Uri(uriBase, UriKind.Absolute);
            client.BaseAddress = _uriBase;
            base.BaseAddress = _uriBase;
        }
        public IntegracaoHttpClient(string uriBase, IDictionary<string, string> headers)
            : this(uriBase)
        {
            AddHeaders(headers);
        }
        public IntegracaoHttpClient(Uri uriBase)
        {
            _uriBase = uriBase;
            client.BaseAddress = _uriBase;
            base.BaseAddress = _uriBase;
        }
        public IntegracaoHttpClient(Uri uriBase, IDictionary<string, string> headers)
            : this(uriBase)
        {
            AddHeaders(headers);
        }

        private HttpContent _content = new StringContent("");

        public HttpClient client { get; private set; } = new HttpClient();

        /// <summary>
        /// Atribuição de cabeçalhos as chamadas http
        /// </summary>
        /// <param name="headers">composto por um dicionário de Key/Value</param>
        /// <example>Authorization , xxxxxxx </example>
        public void AddHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
                return;

            foreach (var item in headers)
            {
                var _key = item.Key ?? "";
                var _value = item.Value ?? "";

                client.DefaultRequestHeaders.Add(_key, _value);
            }
        }

        /// <summary>
        /// Atribuição de valores no corpo da requisição no formato Json
        /// </summary>
        /// <param name="bodyJson">CustomerId, 5</param>
        public HttpRequestMessage AddBodyJson(IDictionary<string, string> bodyJson)
        {
            var _json = bodyJson.SerializeObjectJson();
            _content = new StringContent(_json, Encoding.UTF8, "application/json");

            HttpRequestMessage httpRequest =
                new
                (
                    method: HttpMethod.Post,
                    requestUri: _uriBase
                );
            httpRequest.Content = _content;
            return httpRequest;
        }
        /// <summary>
        /// Atribuição de valores no corpo da requisição no formato Json
        /// </summary>
        /// <param name="bodyJson">CustomerId, 5</param>
        public HttpRequestMessage AddBodyJson(string bodyJson)
        {
            _content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            HttpRequestMessage httpRequest =
                new
                (
                    method: HttpMethod.Post,
                    requestUri: _uriBase
                );
            httpRequest.Content = _content;
            return httpRequest;
        }
        public async Task<HttpResponseMessage> SendDataAsync(HttpRequestMessage request)
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(true); //.GetAwaiter().GetResult();
                ResponseHTTP = new Tuple<int, string>((int)response.StatusCode, response.StatusCode.ToString());
                return await Task.FromResult(response).ConfigureAwait(true);
            }
            catch (HttpRequestException e)
            {
                if (string.IsNullOrEmpty(e?.InnerException?.Message))
                    ResponseHTTP = new Tuple<int, string>(-5, e?.Message ?? "");
                else
                    ResponseHTTP = new Tuple<int, string>(-5, e.InnerException.Message);
                return new HttpResponseMessage();
            }
            catch (Exception e) when (e is TaskCanceledException || e is OperationCanceledException)
            {
                ResponseHTTP = new Tuple<int, string>(-4, e.Message);
                return new HttpResponseMessage();
            }
            catch (Exception e)
            {
                ResponseHTTP = new Tuple<int, string>(-1, e.Message);
                return new HttpResponseMessage();
            }
        }

        public async Task<HttpResponseMessage> GetDataAsync(HttpRequestMessage request)
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(true); //.GetAwaiter().GetResult();
                ResponseHTTP = new Tuple<int, string>((int)response.StatusCode, response.StatusCode.ToString());
                return await Task.FromResult(response).ConfigureAwait(true);
            }
            catch (HttpRequestException e)
            {
                if (string.IsNullOrEmpty(e?.InnerException?.Message))
                    ResponseHTTP = new Tuple<int, string>(-5, e?.Message ?? "");
                else
                    ResponseHTTP = new Tuple<int, string>(-5, e.InnerException.Message);
                return new HttpResponseMessage();
            }
            catch (Exception e) when (e is TaskCanceledException || e is OperationCanceledException)
            {
                ResponseHTTP = new Tuple<int, string>(-4, e.Message);
                return new HttpResponseMessage();
            }
            catch (Exception e)
            {
                ResponseHTTP = new Tuple<int, string>(-1, e.Message);
                return new HttpResponseMessage();
            }
        }
    }
}
