using Agapea_Blazor.Shared;

namespace Agapea_Blazor.Client.Models.Services.Interfaces
{
    public interface IStorageService
    {
        #region...PROPS
        //prop publica q solo usa el servicio indexedDB 
        public event EventHandler<Cliente> RetrievedClientEvent;
        public event EventHandler<List<Item>> ModifiedItemsEvent;
        #endregion
        //como usas el motor de js para guardar en el local storage debe ser async
        #region ... METODOS ASYNC ALMACENAMIENTO DE VALORES EN SERVICIOS STORAGE (localstorage, indexeddb)
        Task StoreClientDataAsync(Cliente clientdata);
        Task StoreJWTAsync(String jwt);
        Task<Cliente> RetrieveClientDataAsync();
        Task<String> RetrieveJWTAsync();
        Task <List<Item>> RetrieveOrderItemsAsync();
        Task OperateOrderItemsAsync(Libro libro, int quantity, String operation);
        #endregion

        //almacenamiento en RAM sincrono 
        #region ... METODOS SYNC ALMACENAMIENTO DE VALORES EN SERVICIOS STORAGE (observables) 
        void StoreClientData(Cliente clientdata);
        void StoreJWT(String jwt);
        Cliente RetrieveClientData();
        String RetrieveJWT();

        List<Item> RetrieveOrderItems();
        void OperateOrderItems(Libro libro, int quantity, String operation);
        #endregion


    }
}
