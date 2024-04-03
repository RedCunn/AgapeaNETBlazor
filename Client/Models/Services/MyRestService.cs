using Agapea_Blazor.Client.Models.Services.Interfaces;
using Agapea_Blazor.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace Agapea_Blazor.Client.Models.Services
{
    public class MyRestService : IRestService
    {

        #region ... PROPS
        private HttpClient _httpClient;
        private IStorageService _storageService;
        #endregion

        public MyRestService(HttpClient httpClientDI, IStorageService storage) 
        {  
            this._httpClient = httpClientDI; 
            this._storageService = storage;
        }
        #region ... METODOS

        #region ............ LLAMADA A ENDPOINTS ZONA CLIENTE ................
        async Task<RestMessage> IRestService.LoginClient(Cuenta credentials)
        {
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cuenta>("/api/RESTCliente/LoginClient", credentials);

            RestMessage _bodyresp = await _resp.Content.ReadFromJsonAsync<RestMessage>();

            return _bodyresp!;
        }
        async Task<RestMessage> IRestService.LoginClient(String idcliente)
        {
            HttpResponseMessage _resp = await this._httpClient.GetAsync($"/api/RESTCliente/LoginClient?idcliente={idcliente}");

            RestMessage _bodyresp = await _resp.Content.ReadFromJsonAsync<RestMessage>();

            return _bodyresp!;
        }
        async Task<RestMessage> IRestService.SignupClient(Cliente newClient)
        {

            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cliente>("/api/RESTCliente/SignupClient", newClient);

            RestMessage _bodyresp = await _resp.Content.ReadFromJsonAsync<RestMessage>();

            return _bodyresp!;
        }

        public async Task<RestMessage> OperateAddress(Direccion addr, string operation)
        {
            Dictionary<String, String> _data = new Dictionary<string, string>
            {
                {"Address", JsonSerializer.Serialize<Direccion>(addr)},
                {"Operation", operation }
            };
            //tengo q mandar el token al servicio, sino ni me dejara mandar nada....
            //hay q mandarselo en una cabecera "Authorization: Bearer .....jwt...."
            String _jwt = this._storageService.RetrieveJWT();
            this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);

            HttpResponseMessage _resp = await this._httpClient
                                                  .PostAsJsonAsync<Dictionary<String, String>>("/api/RESTCliente/OperateAddress", _data);

            return await _resp.Content.ReadFromJsonAsync<RestMessage>();
        }

        public async Task<RestMessage> UploadImage(string imgbase64, string idcliente)
        {
            //tengo q mandar el token al servicio, sino ni me dejara mandar nada....
            //hay q mandarselo en una cabecera "Authorization: Bearer .....jwt...."
            String _jwt = this._storageService.RetrieveJWT();
            this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);

            Dictionary<String, String> _datos = new Dictionary<string, string> {
                { "Imagen", imgbase64 },
                { "IdCliente", idcliente }
            };

            HttpResponseMessage _resp = await this._httpClient
                                                  .PostAsJsonAsync<Dictionary<String, String>>("/api/RESTCliente/UploadImage",_datos);
            return await _resp.Content.ReadFromJsonAsync<RestMessage>();
        }

        public async Task<RestMessage> UpdateClientData(Cliente newClient)
        {
            String _jwt = this._storageService.RetrieveJWT();
            Console.WriteLine("jwt a insertar en cabecera....", _jwt);

            this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);

            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Cliente>("/api/RESTCliente/UpdateClientData", newClient);
            return await _resp.Content.ReadFromJsonAsync<RestMessage>();

        }

        #endregion

        #region ............ LLAMADA A ENDPOINTS ZONA TIENDA ................
        public async Task<List<Libro>> RetrieveBooks(string catID)
        {
            return await this._httpClient
                                .GetFromJsonAsync<List<Libro>>($"/api/RESTTienda/RetrieveBooks?catID={catID}") ??
                                new List<Libro>();
        }

        public async Task<List<Categoria>> RetrieveCategories(string catID)
        {
            if (String.IsNullOrEmpty(catID)) catID= "roots";
            return await this._httpClient
                             .GetFromJsonAsync<List<Categoria>>($"/api/RESTTienda/RetrieveCategories?catID={catID}") ?? 
                             new List<Categoria>();
        }

        public async Task<Libro> RetrieveSingleBook(string isbn13)
        {
            return await this._httpClient
                             .GetFromJsonAsync<Libro>($"/api/RESTTienda/RetrieveSingleBook?isbn13={isbn13}") ??
                             new Libro();
        }

        public async Task<List<Provincia>> RetrieveProvincias()
        {
            return await this._httpClient.GetFromJsonAsync<List<Provincia>>("/api/RESTTienda/RetrieveProvincias") ?? new List<Provincia>();
        }

        public async Task<List<Municipio>> RetrieveMunicipios(string codpro)
        {
            return await this._httpClient.GetFromJsonAsync<List<Municipio>>($"/api/RESTTienda/RetrieveMunicipios?codpro={codpro}") ?? new List<Municipio>();
        }

        public async Task<string> CompleteOrder(OrderModel orderData, Pedido newOrder)
        {
            Dictionary<string, string> _data = new Dictionary<string, string>()
            {
                {"OrderData", JsonSerializer.Serialize<OrderModel>(orderData) },
                {"Order", JsonSerializer.Serialize<Pedido>(newOrder) }
            };

            HttpResponseMessage _res = await this._httpClient
                                                 .PostAsJsonAsync<Dictionary<string,string>>("/api/RESTTienda/CompleteOrder", _data);

            return await _res.Content.ReadAsStringAsync();
        }

        #endregion

        #endregion

    }
}
