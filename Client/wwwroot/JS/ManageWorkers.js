
window.adminWorkers = {
    registerWorker =  () => {
        if (window.SharedWorker) {
            const sharedWorker = new SharedWorker('sharedworker.js');
            console.log("Shared Worker registrado correctamente.");
        } else {
            console.log("El navegador no admite Shared Workers.");
        }
    },
    sendMessageToWorker = (message) => {
        var sharedWorker = new SharedWorker('sharedworker.js');
        sharedWorker.port.postMessage(message);
    }
}    
