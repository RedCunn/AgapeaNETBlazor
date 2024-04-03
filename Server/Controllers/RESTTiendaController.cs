using Agapea_Blazor.Server.Models;
using Agapea_Blazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Agapea_Blazor.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTTiendaController : ControllerBase
    {

        #region ...PROPS
        private AppDBContext _dbContext; // <-- variable que encapsula el obj. AppDBContext definido en Program.cs
        private IConfiguration _configuration;
        private HttpClient _httpClient = new HttpClient();
        private UserManager<MiClienteIdentity> _userManagerService;
        #endregion

        public RESTTiendaController(AppDBContext dbContext, IConfiguration configuration, UserManager<MiClienteIdentity> userManagerDI)
        {
            this._dbContext = dbContext;
            this._configuration = configuration;
            this._userManagerService = userManagerDI;
        }

        #region ...METODOS

        [HttpGet]
        public List<Categoria> RetrieveCategories([FromQuery] String catID)
        {
            //si catID esta vacio , quiero recuperar categoria raiz -> idCategoria = "un solo digito"
            // si no, quiero recuperar subcategoria de una categoria q pasan : idCategoria = catID - "digito"
            try
            {
                Regex _searchPattern;
                if (String.IsNullOrEmpty(catID) || catID == "roots")
                {
                    _searchPattern = new Regex("^[0-9]{1,}$");
                }
                else
                {
                    _searchPattern = new Regex("^"+catID+"-[0-9]{1,}$");
                }

                /*
                EN SQL SERVER NO PUEDES USAR PATTERNS DIRECTAMENTE EN LA CONSULTA LINQ,
                 -> al intentar traducir esta consulta a lenguaje SQL , como no hay operadores de tipo Regex no puede convertirlo y salta Exception
                2 opciones : 
                    -> operador LIKE => se mapea contra metodo .Contains() de LINQ
                    -> descargar toda la tabla en memoria y luego filtrar con operadores LINQ ,
                        para ello usas el metodo .AsEnumerable() tras el nombre del DbSet 
                 
                 */
                //WRONG : 
                //return this._dbContext
                //           .Categorias
                //           .Where((Categoria oneCat) => _searchPattern.IsMatch(oneCat.IdCategoria))
                //           .ToList<Categoria>();

                return this._dbContext
                           .Categorias
                           .AsEnumerable<Categoria>() // ==> SELECT * FROM CATEGORIAS , construyendo obj.Categoria por cada row
                           .Where((Categoria oneCat) => _searchPattern.IsMatch(oneCat.IdCategoria))
                           .ToList<Categoria>();

            }
            catch (Exception)
            {

                return new List<Categoria>();
            }
        }

        [HttpGet]
        public Libro RetrieveSingleBook([FromQuery] String isbn13)
        {
            try
            {
                return this._dbContext
                           .Libros
                           .Where((Libro onebook) => onebook.ISBN13 == isbn13).Single<Libro>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al recuperar libro por isbn13 : ", ex.Message);
                return new Libro();
            }
        }


        [HttpGet]
        public List<Libro> RetrieveBooks([FromQuery] String catID)
        {
            try
            {
                //si catID esta vacio recuperas libros en oferta, por defecto usamos cat. '2-10'
                if (String.IsNullOrEmpty(catID)) catID = "2-10";

                // para hacer la query SELECT sobre tabla libros, necesito DBset<Libro> Libros de AppDBContext
                // y para ello tenemos que usar LINQ para hacer la query
                return this._dbContext
                           .Libros
                           .Where((Libro onebook) => onebook.idCategoria.StartsWith(catID))
                           .ToList<Libro>();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al recuperar libros por categoria : ",ex.Message);
                return new List<Libro>();
            }
        }

        [HttpGet]
        public List<Provincia> RetrieveProvincias()
        {
            try
            {
                return this._dbContext.Provincias.AsEnumerable<Provincia>().OrderBy((Provincia p) => p.PRO).ToList<Provincia>();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al recuperar Provincias ...." +ex.Message);
                return new List<Provincia>();
            }
        }

        [HttpGet]
        public List<Municipio> RetrieveMunicipios([FromQuery] String codpro)
        {
            try
            {
                return this._dbContext.Municipios.Where((Municipio muni) => muni.CPRO == codpro).ToList<Municipio>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al recuperar Municipios con codpro : "+codpro+" " + ex.Message);
                return new List<Municipio>();
            }
        }

        #region /////// PEDIDO //////////
        [HttpPost]
        public async Task<String> CompleteOrder([FromBody] Dictionary<string, string> data)
        {
            try
            {

                OrderModel orderData = JsonSerializer.Deserialize<OrderModel>(data["OrderData"]);
                Pedido newOrder = JsonSerializer.Deserialize<Pedido>(data["Order"]);

                if (orderData.metodoPago == "paypal")
                {
                  return  await this.PayPalPayment(newOrder);    
                }

                if (orderData.metodoPago == "tarjeta")
                {
                   bool correctStripePayment = await this.StripePayment(orderData, newOrder);
                }
                return "";

            }
            catch (Exception)
            {

                return "";
            }
        }

        private async Task<bool> StripePayment(OrderModel orderData, Pedido newOrder)
        {
            try
            {
                // Antes de realizar un nuevo pago, verifica si ya existe un registro de pago para este pedido
                bool isPaymentAlreadyProcessed = this._dbContext.PagosStripe.Any(p => p.IdPedido == newOrder.IdPedido);

                if (isPaymentAlreadyProcessed)
                {
                    // Ya se ha procesado un pago para este pedido, así que no hagas nada más
                    return false; // O lanza una excepción, dependiendo de tu flujo de trabajo
                }

                //------------------------------------------------------------------------------
                //1º paso) crear un objeto Customer en stripe via api...hay q pasar en cabecera Authentication: Bearer secretKey
                //https://stripe.com/docs/api/customers/create
                //------------------------------------------------------------------------------

                Dictionary<string,string> customerValues = new Dictionary<string, string>
                {
                    {"name", orderData.NombreDestinatario+" "+orderData.ApellidosDestinatario},
                    {"email", orderData.EmailDestinatario},
                    {"phone", orderData.TelefonoDestinatario },
                    {"address[city]",orderData.DireccionEnvio.MunicipioDirec.DMUN50},
                    {"address[state]",orderData.DireccionEnvio.ProvinciaDirec.PRO },
                    {"address[country]",orderData.DireccionEnvio.Pais },
                    {"address[postal_code]",orderData.DireccionEnvio.CP.ToString()},
                    {"address[line1]",orderData.DireccionEnvio.Calle }
                };
                
                string secretKey = this._configuration["Stripe:SecretKey"]!;
                HttpRequestMessage _requestCustomer = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/customers");
                _requestCustomer.Headers.Add("Authorization", $"Bearer {secretKey}");
                _requestCustomer.Content = new FormUrlEncodedContent(customerValues);

                HttpResponseMessage _responseCustomer = await this._httpClient.SendAsync(_requestCustomer);

                if (_responseCustomer.IsSuccessStatusCode)
                {
                    JsonNode _resJSON = JsonNode.Parse(await _responseCustomer.Content.ReadAsStringAsync())!;

                    //recupero prop."id" del objeto json devuelto por STRIPE sin deserializar a una clase propia
                    //https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization?pivots=dotnet-8-0#deserialize-without-a-net-class
                    //o usas JsonDocument o JSON-DOM <---mas rapido, permite JPath

                    string _idCustomer = _resJSON["id"]!.ToString();

                    //--------------------------------------------------------------------
                    //2º paso) crear Card asociada al customer-id q me devuelve stripe...
                    //https://stripe.com/docs/api/cards/create?lang=curl
                    //---------------------------------------------------------------------

                    Dictionary<string, string> _cardOptions = new Dictionary<string, string>
                    {
                        {"source","tok_visa" }
                    };

                    HttpRequestMessage _requestCard = new HttpRequestMessage(HttpMethod.Post, $"https://api.stripe.com/v1/customers/{_idCustomer}/sources");
                    _requestCard.Headers.Add("Authorization", $"Bearer {secretKey}");
                    _requestCard.Content = new FormUrlEncodedContent(_cardOptions);

                    HttpResponseMessage _responseCard = await this._httpClient.SendAsync(_requestCard);

                    if (_responseCard.IsSuccessStatusCode)
                    {
                        JsonNode _cardJson = JsonNode.Parse(await _responseCard.Content.ReadAsStringAsync())!;

                        string _idCard = _cardJson["id"]!.ToString();

                        //3º paso) crear un charge con el customer-id y el card-id
                        Dictionary<string, string> _chargeOptions = new Dictionary<string, string>
                        {
                            { "customer",_idCustomer },
                            { "source", _idCard },
                            { "currency","eur"},
                            { "amount",((int)(newOrder.Total*100)).ToString()},
                            { "description",newOrder.IdPedido }

                        };

                        HttpRequestMessage _requestCharge = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/charges");
                        _requestCharge.Headers.Add("Authorization", $"Bearer {secretKey}");
                        _requestCharge.Content = new FormUrlEncodedContent(_chargeOptions);

                        HttpResponseMessage _responseCharge = await _httpClient.SendAsync(_requestCharge);

                        if (_responseCharge.IsSuccessStatusCode)
                        {
                            //tengo q leer la propiedad status del json de la response para ver si efectivamente ha succeeded
                            JsonNode _chargeJson = JsonNode.Parse(await _responseCharge.Content.ReadAsStringAsync())!;
                            string _chargeID = _chargeJson["id"]!.ToString();
                            string _status = _chargeJson["status"]!.ToString();

                            if (_status == "succeeded")
                            {
                                this._dbContext.Pedidos.Add(newOrder);
                                this._dbContext.PagosStripe.Add(new PagoStripe
                                {
                                    IdCliente = newOrder.IdCliente,
                                    IdPedido = newOrder.IdPedido,
                                    ChargeId = _chargeID
                                });

                                this._dbContext.SaveChanges();
                                //redirigir a PedidoFinalizado OK
                                return true;
                            }
                            else
                            {
                                throw new Exception("BAD STATUS AL HACER CHARGE EN STRIPE");
                            }
                        }
                        else
                        {
                            throw new Exception("FALLO AL HACER CHARGE EN STRIPE");
                        }


                    }
                    else
                    {
                        throw new Exception("FALLO AL HACER CARD EN STRIPE");
                    }
                }
                else
                {
                    throw new Exception("FALLO AL HACER CUSTOMER EN STRIPE");
                }


            }
            catch (Exception)
            {

                return false;
            }
        }

        #region ...... PAYPAL ..........

        private async Task<String> PayPalPayment(Pedido newOrder)
        {
            try
            {
                //1º paso )) conseguir con claves de desarrollador de paypal token acceso a la api
                //https://developer.paypal.com/api/rest/authentication/
                //REQUISISTOS:
                //-1- cabecera Authorization Basic con clientId : clientSecret en base64
                //-2- en body de peticion en formato x-www-form-urlencoded variable : grant_type 

                HttpRequestMessage _requestToken = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token");

                string clientId = this._configuration["PayPal:CLIENT_ID"]!;
                string clientSecret = this._configuration["PayPal:CLIENT_SECRET"]!;
                string _base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

                _requestToken.Headers.Add("Authorization", $"Basic {_base64Auth}");
                _requestToken.Content = new StringContent("grant_type=client_credentials",
                                                          Encoding.UTF8,
                                                          "application/x-www-form-urlencoded");

                HttpResponseMessage _responseToken = await this._httpClient.SendAsync(_requestToken);

                if (_responseToken.IsSuccessStatusCode)
                {
                    String _serializedJSONresponse = await _responseToken.Content.ReadAsStringAsync();
                    JsonNode _deserializedJSONresponse = JsonNode.Parse(_serializedJSONresponse);
                    string _accessToken = _deserializedJSONresponse["access_token"].ToString();
                    //-- este es el token que añado a la cabecera Authorization Bearer -------

                    // 2º paso )) me creo la orden de pedido paypal : https://developer.paypal.com/docs/api/orders/v2/#orders_create
                    //REQUISISTOS:
                    //-1- cabecera Authorization Bearer con token recien creado en paso 1º
                    //-2- en body de peticion en formato json variable : {intent : ..., purchase_units : [ ... ]}

                    HttpRequestMessage _requestOrder = new HttpRequestMessage(HttpMethod.Post,
                                                                              "https://api-m.sandbox.paypal.com/v2/checkout/orders");
                    _requestOrder.Headers.Add("Authorization", $"Bearer {_accessToken}");

                    var _listitems = newOrder.ElementosPedido
                                             .Select((Item it) => new {
                                                 name = it.LibroItem.Titulo,
                                                 quantity = it.CantidadItem.ToString(),
                                                 unit_amount = new
                                                 {
                                                     currency_code = "EUR",
                                                     value = it.LibroItem.Precio.ToString().Replace(",", ".")
                                                 }
                                             }).ToList();

                    // ... obj json order para mandar en el body de la peticion
                    var order = new
                    {
                        intent = "CAPTURE",
                        purchase_units = new[]
                        {
                                new
                                {
                                    items = _listitems,
                                    amount = new
                                    {
                                        currency_code = "EUR",
                                        value = newOrder.Total.ToString().Replace(",","."),
                                        breakdown = new
                                        {
                                            shipping = new
                                            {
                                                currency_code = "EUR",
                                                value = newOrder.GastosEnvio.ToString().Replace(",",".")
                                            },
                                            item_total = new
                                            {
                                                currency_code = "EUR",
                                                value = newOrder.Subtotal.ToString().Replace(",",".")
                                            }
                                        }
                                    }
                                }
                            },
                        application_context = new
                        {
                            return_url = $"https://localhost:7279/api/RESTTienda/PaypalCallBack?idcliente={newOrder.IdCliente}&idpedido={newOrder.IdPedido}",
                            cancel_url = $"https://localhost:7279/api/RESTTienda/PaypalCallBack?idcliente={newOrder.IdCliente}&idpedido={newOrder.IdPedido}&Cancel=true"
                        }
                    };


                    _requestOrder.Content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
                    HttpResponseMessage _responseOrder = await this._httpClient.SendAsync(_requestOrder);

                    if (_responseOrder.IsSuccessStatusCode)
                    {
                        string _serializedJSONOrderResponse = await _responseOrder.Content.ReadAsStringAsync();
                        JsonNode _deserializedJSONOrderResponse = JsonNode.Parse(_serializedJSONOrderResponse);
                        // del JSON de la respuesta me interesa : id <--- id del pago para meterlo en DB junto con idcliente y idpedido
                        // del array de links <--- el que contiene approve para mandarselo al cliente blazor para q le redireccione a la pasarela 

                        String _urlSalto = _deserializedJSONOrderResponse["links"].AsArray()
                                            .Where((JsonNode link) => link["rel"].ToString() == "approve")
                                            .Select((JsonNode link) => link["href"].ToString())
                                            .Single<String>();

                        this._dbContext.Pedidos.Add(newOrder);
                        this._dbContext.PagosPaypal.Add(new PagoPaypal
                        {
                            IdCliente = newOrder.IdCliente,
                            IdPedido = newOrder.IdPedido,
                            IdPagoPayPal = _deserializedJSONOrderResponse["id"].ToString()
                        });
                        this._dbContext.SaveChanges();
                        return _urlSalto;

                    }
                    else
                    {
                        throw new Exception("error al crear la ORDEN de pago en Paypal...." + _responseOrder.StatusCode);
                    }

                }
                else
                {
                    throw new Exception("fallo en acceso a API de paypal...." + _responseToken.StatusCode);
                }

            }
            catch (Exception)
            {

                return "";
            }
        }

        [HttpGet]
        public async Task<ActionResult> PayPalCallback([FromQuery] string idcliente,
                                                        [FromQuery] string idpedido,
                                                        [FromQuery] Boolean? cancel)
        {
            //!!! devolver un ActionResult no es para nada ideal como respuesta de servicio restful, que deberia ser un json y no una redireccion
            // pero como el cliente blazor ha perdido toda ref al cargar la url de paypal, si devolviese un RestMessage el navegador del cliente
            // no lo podría acceder <<- por eso estamos usando el popup, que tampoco es chachipistachi
            /*
             formato url de vuelta de paypal:
                https://localhost:7279/api/RESTTienda/PaypalCallBack ? idcliente=da40e3ea-a602-4f9c-a544-2e590e7bf508 & 
                                                                       idpedido=883f2fbf-0afc-4211-8df2-aed2696bffac & 
                                                                       cancel=true ? optional
                                                                       token=31V84989DC5331204 & 
                                                                       PayerID=W59CS27V2S8US
             */

            try
            {
                // 1º paso )) tengo que volver a obtener un token de acceso a la API de paypal
                HttpRequestMessage _requestToken = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token");

                string clientId = this._configuration["PayPal:CLIENT_ID"]!;
                string clientSecret = this._configuration["PayPal:CLIENT_SECRET"]!;
                string _base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

                _requestToken.Headers.Add("Authorization", $"Basic {_base64Auth}");
                _requestToken.Content = new StringContent("grant_type=client_credentials",
                                                          Encoding.UTF8,
                                                          "application/x-www-form-urlencoded");

                HttpResponseMessage _responseToken = await this._httpClient.SendAsync(_requestToken);

                if (_responseToken.IsSuccessStatusCode)
                {
                    String _serializedJSONresponse = await _responseToken.Content.ReadAsStringAsync();
                    JsonNode _deserializedJSONresponse = JsonNode.Parse(_serializedJSONresponse);
                    string _accessToken = _deserializedJSONresponse["access_token"].ToString();

                    // 2º paso )) para finalizar el pago tengo que capturar la aprovacion por parte del cliente :
                    //https://developer.paypal.com/docs/api/orders/v2/#orders_capture
                    // parametros que necesito : 
                    // - cabecera Authorization Bearer token
                    // - en url parametro orderId de paypal

                    String _orderId = this._dbContext.PagosPaypal
                                                        .Where((PagoPaypal p) => p.IdPedido == idpedido && p.IdCliente == idcliente)
                                                        .Select((PagoPaypal p) => p.IdPedido)
                                                        .Single<String>();

                    HttpRequestMessage _reqOrderCapture = new HttpRequestMessage(HttpMethod.Post, $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{_orderId}/capture");
                    _reqOrderCapture.Headers.Add("Authorization", $"Bearer{_accessToken}");

                    HttpResponseMessage _resOrderCapture = await this._httpClient.SendAsync(_reqOrderCapture);

                    if (_resOrderCapture.IsSuccessStatusCode)
                    {
                        //formato json de la respuesta : ... mirar RESPONSE en link de arriba
                        //seria conveniente preguntar por propiedad status == "APPROVED"
                        String _serializedJSONOrderCaptureResponse = await _responseToken.Content.ReadAsStringAsync();
                        JsonNode _deserializedJSONOrderCaptureResponse = JsonNode.Parse(_serializedJSONOrderCaptureResponse);

                        if (_deserializedJSONOrderCaptureResponse["status"].ToString() == "APPROVED")
                        {
                            return Redirect($"https://localhost:7279/Tienda/FinalizarPedidoOK?idpedido={idpedido}$idcliente={idcliente}");
                        }
                        else
                        {
                            return Redirect($"https://localhost:7279/Tienda/FinalizarPedidoOK?idpedido={idpedido}$idcliente={idcliente}");
                        }



                    }
                    else
                    {
                        throw new Exception($"fallo al intentar capturar orden de cobro : {_orderId} desde paypal del pedido {idpedido} del cliente {idcliente}");
                    }

                }
                else
                {
                    throw new Exception("fallo en acceso a API de paypal...." + _responseToken.StatusCode);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        #endregion




        #endregion

        #endregion

    }
}
