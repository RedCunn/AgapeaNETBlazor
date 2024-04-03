using Agapea_Blazor.Client.Models.Services.Interfaces;
using Agapea_Blazor.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor.Client.Models.Services
{
    public class IndexedDBService : IStorageService
    {
        #region .... PROPS
        private IJSRuntime _jsService;
        private DotNetObjectReference<IndexedDBService> _refIndexedService;

        //prop. de tipo evento q vamos a usar para notificar a aquellos componentes que usen el servicio 
        // q se han recuperado desde JS (indexedDB) los datos async y ya estan disponibles para mostrar
        public event EventHandler<Cliente> RetrievedClientEvent;
        public event EventHandler<List<Item>> ModifiedItemsEvent;
        #endregion

#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public IndexedDBService(IJSRuntime jsService)
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            this._jsService = jsService;
            //Esto crea una referencia al objeto de .NET actual. El parámetro this hace referencia al contexto actual.
            //Esta referencia permite que el objeto de .NET sea accesible desde JavaScript.
            this._refIndexedService = DotNetObjectReference.Create(this);
        }

        #region ... METODOS ASYNC

        public async Task StoreClientDataAsync(Cliente clientdata)
        {
            await this._jsService.InvokeVoidAsync("adminIndexedDB.storeClientData", clientdata);
        }

        public async Task StoreJWTAsync(string jwt)
        {
            await this._jsService.InvokeVoidAsync("adminIndexedDB.storeToken", jwt);
        }

        public async Task<Cliente> RetrieveClientDataAsync()
        {
            //a la funcion de JS retrieveClientData de indexedDB le paso la ref del servicio para que cuando acabe
            // su ejecucion (es async, no se lo q va a tardar...) llame a metodo de este servicio 
            return await this._jsService.InvokeAsync<Cliente>("adminIndexedDB.retrieveClientData",
                                                              this._refIndexedService
                                                              // , falta saber cómo pasarle el email
                                                              );
        }

        public async Task<string> RetrieveJWTAsync()
        {
            return await this._jsService.InvokeAsync<string>("adminIndexedDB.retrieveToken",
                                                             this._refIndexedService
                                                             // , falta pasarle email
                                                             );
        }
        // metodo invocable desde codigo JS (nuestro fichero : ManageIndexedDB.js)
        [JSInvokable("CallbackIndexedDBBlazorService")]
        public void CallFromJS(Cliente clientIndexedDB)
        {
            //cuando recibo datos del cliente desde JS, disparo evento (osea notifico a quien esté escuchando)
            this.RetrievedClientEvent.Invoke(this, clientIndexedDB);
        }

        public Task<List<Item>> RetrieveOrderItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task OperateOrderItemsAsync(Libro libro, int quantity, string operation)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ... METODOS SYNC
        public void StoreClientData(Cliente clientdata)
        {
            throw new NotImplementedException();
        }

        public void StoreJWT(string jwt)
        {
            throw new NotImplementedException();
        }
        public Cliente RetrieveClientData()
        {
            throw new NotImplementedException();
        }
        public string RetrieveJWT()
        {
            throw new NotImplementedException();
        }

        public List<Item> RetrieveOrderItems()
        {
            throw new NotImplementedException();
        }

        public void OperateOrderItems(Libro libro, int quantity, string operation)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
