//fichero js que define un obj que cuelga directamente de "window" para hacerlo global a todos los componentes blazor
// con props y metodos para manejar datos usando indexedDB del navegador
/*
cada operacion sobre una DB indexada es un obj que implemente la intefaz IDBRequest , susceptible de eventos "success"/"error"
    dominio ---> conjunto de DB ---> db (nombre, version) <---{ objectStore-1 <== datos : {key, value} (search index)
                                                                objectStore-2 <== datos : {key, value} (search index)
                                                                . . .
                                                                / toda operacion objectStore necesita una Transaction / 
Toda esta estructura se crea cuando te creas la DB, 
cuando abres conexion a una DB, se lanza evento especial a parte del success y el error => upgradeneeded
    se produce siempre que :
        - intentas abrir una DB q no existe (se crea por 1ª vez) <== aprovechas aqui para crear objectStores, indexes..etc.
        - intentas abrir una version de la DB superior a la q esta creada 
 */
// https://learn.microsoft.com/es-es/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript?view=aspnetcore-8.0
// como las op. son async, cuando finalice la op. sobre la api del navegador, debemos llamar a metodos de .net
// servicio, componente, etc
// basicamente consiste en pasar una ref. del servicio, componente, etc, a los metodos js desde los q te interese
// invocar a metodos .net, y con esa ref, invicar dichos metodos
// -> esta ref se define en el servicio o comp obj. clase : DotNetObjectReference<clase_servicio>
// a los metodos .NET invocables desde JS, se les pone el atributo : JSInvocable
function _createIndexedDBSchema(ev) {
        var _db = ev.target.result; //<---- objeto IDBDatabase
        var _clientsObjectStore = _db.createObjectStore('clientdata', { keypath: 'email' }); //<--- objeto IDBObjectstore
        _clientsObjectStore.createIndex('email', 'email', { unique: true });

        var _tokensObjectStore = _db.createObjectStore('tokens', { keypath: 'email' }); //<--- objeto IDBObjectstore
        _clientsObjectStore.createIndex('email', 'email', { unique: true });

        var _loggedClientObjectStore = _db.createObjectStore('loggedclient', { keypath: 'email' }); //<--- objeto IDBObjectstore
        _loggedClientObjectStore.createIndex('email', 'email', { unique: true });

}

window.adminIndexedDB = {
    storeClientData: (clientdata) => {
        var _reqDB = indexedDB.open('agapea2024', 1);
        _reqDB.addEventListener('upgradeneeded', (ev) => _createIndexedDBSchema(ev));
        _reqDB.addEventListener('success', (ev) => {

            var _db = ev.target.result;
            var _transaction = _db.transaction(['clientdata', 'loggedclient'], 'readwrite');

            // 1ª op. transaction almacenamiento en clientdata : 
            var _reqInsertClientData = _transaction.objectStore('clientdata').add(clientdata, clientdata.Credentials.email);
            _reqInsertClientData.addEventListener('success', (event) => {
                var _clientdata = event.target.result;
                console.log('datos insertados correctamente en el objectstore clientdata...', _clientdata);
            })
            _reqInsertClientData.addEventListener('error', (err) => console.log('error al insertar clientdata...', err));

            // 2ª op. transaction almacenamiento en loggedclient : 
            var _reqInsertLoggedClient = _transaction.objectStore('loggedclient').add(clientdata.Credentials.email, true);
            _reqInsertLoggedClient.addEventListener('success', (ev2) => {
                var _clientdata = ev2.target.result;
                console.log('datos insertados correctamente en el objectstore loggedclient...', _clientdata);
            })
            _reqInsertLoggedClient.addEventListener('error', (err2) => console.log('error al insertar clientdata...', err2));


        });
        _reqDB.addEventListener('error', (error) => {
            console.log('error al abrir db agapea2024 de indexedDB ...', error);
        });
    },
    retrieveClientData: (refNETservice, email) => {
        var _reqDB = indexedDB.open('agapea2024', 1);
        _reqDB.addEventListener('upgradeneeded', (ev) => _createIndexedDBSchema(ev));
        _reqDB.addEventListener('success', (ev) => {
            // para recuperar datos asociados a un cliente, del objectStore
            // me creo obj IDBTransaction de tipo readonly

            var _db = ev.target.result;
            var _transaction = _db.transaction(['clientdata', 'loggedclient'], 'readonly');

            // le dices sobre qué objectStore quieres operar, y le dices la clave del documento que quieres recuperar con .get()
            // devuelve una peticion tipo IDBRequest con funciones manejadoras de eventos
            var _reqSelectLoggedClient = _transaction.objectStore('loggedclient').get('loggedinclient');
            _reqSelectLoggedClient.addEventListener('success', (ev2) => {
                var _clientemail = ev2.target.result;

                var _reqSelectClientData = _transaction.objectStore('clientdata').get(email);
                _reqSelectClientData.addEventListener('success', (ev3) => {
                    var _clientdata = ev3.target.result;
                    // paso datos del cliente recuperado desde indexedDB al servicio blazor usando la ref
                    refNETservice.invokeMethodAsync('CallbackIndexedDBBlazorService', _clientdata);
                })
                _reqSelectClientData.addEventListener('error', (err3) => console.log(`error al recuperar clientdata del email ${email}...`, err3));


            }); // CIERRE SUCCESS _reqSelectLoggedClient
            _reqSelectClientData.addEventListener('error', (err2) => console.log(`error al recuperar clientdata del email ${email}...`, err2));

        }); 
        _reqDB.addEventListener('error', (error) => {
            console.log('error al abrir db agapea2024 de indexedDB ...', error);
        });
    },
    storeToken: (token) => {
        var _reqDB = indexedDB.open('agapea2024', 1);
        _reqDB.addEventListener('upgradeneeded', (ev) => _createIndexedDBSchema(ev));
        _reqDB.addEventListener('success', (ev) => {

            var _db = ev.target.result;
            var _transactionClientData = _db.transaction(['clientdata'], 'readonly');
            var _transactionClientToken = _db.transaction(['tokens'], 'readwrite');

            var _reqInsertClientToken = _transactionClientToken.objectStore('tokens').add(token, _transactionClientData.Credentials.email);
            _reqInsertClientToken.addEventListener('success', (event) => {
                var _clientdata = event.target.result;
                console.log('JWT insertado correctamente en el objectstore jwt...', _clientdata);
            })
            _reqInsertClientToken.addEventListener('error', (err) => console.log('error al insertar jwt...', err));

        });
        _reqDB.addEventListener('error', (error) => {
            console.log('error al abrir db agapea2024 de indexedDB ...', error);
        });
    },
    retrieveClientToken: (refNETservice, email) => {

    }
}