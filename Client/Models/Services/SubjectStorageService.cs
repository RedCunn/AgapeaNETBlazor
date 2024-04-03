using Agapea_Blazor.Client.Models.Services.Interfaces;
using Agapea_Blazor.Shared;
using System.Reactive.Subjects;

namespace Agapea_Blazor.Client.Models.Services
{
    /*
         SERVICIO :

            BehaviourSubject<Cliente>
        -----------------------------------
                    datos_cliente
        -----------------------------------
                ||                      ||
            datos_cliente              datos
                OUTPUT                 INPUT
                  |                     |
        RetrieveClientData()    StoreClientData(datos)
         

                    BehaviourSubject<String>
                ------------------------------
                            jwt
                ------------------------------
                    
                    BehaviourSubject<List<Item>>
                ------------------------------
                       elementos_pedido
                ---------------------------------------------------------------
                            || <-- variable privada             || <-- modifico variable privada, actualiza datos del observable
                    RetrieveOrderElements()             OperateOrderElements(item, cantidad, operacion)
         */
    public class SubjectStorageService : IStorageService
    {
        #region ... PROPS

        public event EventHandler<Cliente>? RetrievedClientEvent;
        public event EventHandler<List<Item>>? ModifiedItemsEvent;
        private BehaviorSubject<Cliente> _clientSubject = new BehaviorSubject<Cliente> (null);
        private BehaviorSubject<String> _jwtSubject = new BehaviorSubject<string>("");
        private BehaviorSubject <List<Item>> _orderElementsSubject = new BehaviorSubject <List<Item>>(new List<Item>());
        
        //VARIABLES PARA ALMACENAR LOS DATOS DE LOS SUBJECTS :
        private Cliente _clientData = new Cliente();
        private String _jwtData = "";
        private List<Item> _orderElementsData = new List<Item>();
        #endregion

        public SubjectStorageService()
        {
            IDisposable _subscripOrderItemsSubject = this._orderElementsSubject
                                                         .Subscribe<List<Item>>((List<Item> items) => this._orderElementsData = items);

            IDisposable _subscripClientSubject = this._clientSubject
                                                     .Subscribe<Cliente>((Cliente ObsData) => this._clientData = ObsData);

            IDisposable _jwtSubject = this._jwtSubject
                                          .Subscribe<String>((String jwtData) => this._jwtData = jwtData);
        }


        #region ... METODOS SYNC

        public Cliente RetrieveClientData()
        {
            return this._clientData;
        }


        public string RetrieveJWT()
        {
            return this._jwtData;
        }

        public void StoreClientData(Cliente clientdata)
        {
            this._clientSubject.OnNext(clientdata); // <- actualizo datos en observable Cliente
            this.RetrievedClientEvent?.Invoke(this, clientdata); // <- disparo evento de actualización de datos del cliente por si alguien está escuchando
        }

        public void StoreJWT(string jwt)
        {
            this._jwtSubject.OnNext(jwt); // <- actualizo datos en observable String
        }
        public List<Item> RetrieveOrderItems()
        {
            return this._orderElementsData;
        }

        public void OperateOrderItems(Libro libro, int quantity, string operation)
        {
            
            int _itemposition = this._orderElementsData.FindIndex((Item it) => it.LibroItem.ISBN13 == libro.ISBN13);

            switch (operation) {

                case "add":
                    // nuevo item, comprobar si no existe para añadir o aumentar cantidad
                    //Item _libroRepe = this._orderElementsData.Find((Item it) => it.LibroItem.ISBN13 == libro.ISBN13);
                    if (_itemposition != -1)
                    {
                        this._orderElementsData[_itemposition].CantidadItem = quantity;
                    }
                    else
                    {
                        _orderElementsData.Add(new Item { LibroItem = libro, CantidadItem = quantity});
                    }
                    break;
                case "remove":

                    if (_itemposition != -1 ) _orderElementsData.RemoveAt(_itemposition);
                     
                    break;
                case "modify":
                    if (_itemposition != -1) {
                        int newQuantity = _orderElementsData[_itemposition].CantidadItem + quantity;
                        if (newQuantity >= 1)
                        {
                            _orderElementsData[_itemposition].CantidadItem = newQuantity;
                        }
                        else
                        {
                            _orderElementsData.RemoveAt(_itemposition);
                        }
                    } 
                    break;

                default:
                    break;

            }

            //ACTUALIZO OBSERVABLE
            this._orderElementsSubject.OnNext(_orderElementsData);
            //disparo evento de refresco
            this.ModifiedItemsEvent?.Invoke(this, _orderElementsData);
        }

        public void TriggerRefreshingClientDataEvent(Cliente client)
        {
            Console.WriteLine("...evento para refrescar datos del cliente disparado...");
            this.RetrievedClientEvent?.Invoke(this, client);
        }
        #endregion

        #region ... METODOS ASYNC
        public Task<Cliente> RetrieveClientDataAsync()
        {
            throw new NotImplementedException();
        }
        public Task<string> RetrieveJWTAsync()
        {
            throw new NotImplementedException();
        }

        public Task StoreClientDataAsync(Cliente clientdata)
        {
            throw new NotImplementedException();
        }
        public Task StoreJWTAsync(string jwt)
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

        #endregion
    }
}
