using Agapea_Blazor.Shared;
using Microsoft.JSInterop;

namespace Agapea_Blazor.Client.Models.Services
{
    public class NavigatorService
    {
        private IJSRuntime _jsRuntime;
        private DotNetObjectReference<NavigatorService> _refNavigatorService;

        //prop. de tipo evento q vamos a usar para notificar a aquellos componentes que usen el servicio 
        // q se han recuperado desde JS (indexedDB) los datos async y ya estan disponibles para mostrar
        public event EventHandler<Cliente> RetrievedClientEvent;
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public NavigatorService(IJSRuntime jsRuntime)
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            this._jsRuntime = jsRuntime;
            this._refNavigatorService = DotNetObjectReference.Create(this);
        }

        public async Task registerWorker()
        {
            await this._jsRuntime.InvokeVoidAsync("adminWorkers.registerWorker");
        }

        // metodo invocable desde codigo JS (nuestro fichero : ManageIndexedDB.js)
        [JSInvokable("CallbackNavigatorBlazorService")]
        public void CallFromJS(Cliente clientIndexedDB)
        {
            //cuando recibo datos del cliente desde JS, disparo evento (osea notifico a quien esté escuchando)
            this.RetrievedClientEvent.Invoke(this, clientIndexedDB);
        }

    }
}
