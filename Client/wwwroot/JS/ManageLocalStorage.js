//fichero js que define un objeto que cuelga directamente de "window" (para hacerlo global a todos los comp.blazor)
//con props y metodos para manejar datos usando LocalStorage del navegador

window.adminLocalStorage = {
    storeValue: function (key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    retrieveValue: function (key) {
        return JSON.parse(localStorage.getItem(key));
    },
}