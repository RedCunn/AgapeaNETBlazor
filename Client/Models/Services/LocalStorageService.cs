using Agapea_Blazor.Client.Models.Services.Interfaces;
using Agapea_Blazor.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor.Client.Models.Services
{
    public class LocalStorageService : IStorageService
    {
        #region ...PROPS
        private IJSRuntime _jsService;

#pragma warning disable CS0067 // El evento 'LocalStorageService.RetrievedClientEvent' nunca se usa
        public event EventHandler<Cliente> RetrievedClientEvent;
        public event EventHandler<List<Item>> ModifiedItemsEvent;
#pragma warning restore CS0067 // El evento 'LocalStorageService.RetrievedClientEvent' nunca se usa
        #endregion

#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public LocalStorageService(IJSRuntime jsService)
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            this._jsService = jsService;
        }
        #region ... metodos ASYNC ...

        public async Task<Cliente> RetrieveClientDataAsync()
        {
            return await this._jsService.InvokeAsync<Cliente>("adminLocalStorage.retrieveValue","ClientData");
        }


        public async Task<string> RetrieveJWTAsync()
        {
            return await this._jsService.InvokeAsync<string>("adminLocalStorage.retrieveValue", "JWT");
        }

        public async Task StoreClientDataAsync(Cliente clientdata)
        {
            //tendria q ejecutar esto desde js : localStorage.setItem("datoscliente", JSON.stringfy(clientdata))
            await _jsService.InvokeVoidAsync("adminLocalStorage.storageValue", "ClientData", clientdata);
        }

       public async Task StoreJWTAsync(string jwt)
        {
            await _jsService.InvokeVoidAsync("adminLocalStorage.storageValue", "JWT", jwt);
        }
        #endregion

        #region ... metodos SINCRONOS ...
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

        public Task<List<Item>> RetrieveOrderItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task OperateOrderItemsAsync(Libro libro, int quantity, string operation)
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
