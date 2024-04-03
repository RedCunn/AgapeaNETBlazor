
self.onconnect = function (e) {
    const port = e.ports[0];

    port.addEventListener("message", (e) => {
        const workerResult = `Result : ${e.data}`;
        port.postMessage(workerResult);
    })

    port.start();
    
};
