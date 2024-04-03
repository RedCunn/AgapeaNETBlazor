
function createBillAddress() {
    var pais = document.getElementById("inputPaisFactura").value;
    var provincia= document.getElementById("inputProvinciaFactura").value;
    var calle = document.getElementById("inputCalleFactura").value;
    var cp= document.getElementById("inputCPFactura").value;
    var municipio= document.getElementById("inputMunicipioFactura").value;

    return {
        Pais: pais,
        ProvinciaDirec: provincia,
        MunicipioDirec: municipio,
        Calle: calle,
        CP: cp,
        EsPrincipal: false,
        EsFacturacion: false
    };
}
function createDeliveryAddress() {
    var pais = document.getElementById("inputPais").value;
    var provincia = document.getElementById("inputProvincia").value;
    var calle = document.getElementById("inputCalle").value;
    var cp = document.getElementById("inputCP").value;
    var municipio = document.getElementById("inputMunicipio").value;

    return {
        Pais: pais,
        ProvinciaDirec: provincia,
        MunicipioDirec: municipio,
        Calle: calle,
        CP: cp,
        EsPrincipal: false,
        EsFacturacion: false
    };
}