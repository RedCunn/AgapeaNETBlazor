using Agapea_Blazor.Server.Models;
using Agapea_Blazor.Server.Models.Services.Interfaces;
using Agapea_Blazor.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Agapea_Blazor.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTClienteController : ControllerBase
    {
        #region ... PROPS

        private UserManager<MiClienteIdentity> _userManagerService;
        private SignInManager<MiClienteIdentity> _signInManager;
        private IClienteCorreo _clientEmailService;
        private IConfiguration _appSettingsAccess;
        private AppDBContext _appDBContext;
        #endregion

        public RESTClienteController(UserManager<MiClienteIdentity> userManagerDI, 
                                     SignInManager<MiClienteIdentity> signInManagerDI, 
                                     IClienteCorreo emailClient, 
                                     IConfiguration appSettAccess,
                                     AppDBContext appDBContext)
        {
            this._userManagerService = userManagerDI;
            this._clientEmailService = emailClient;
            this._signInManager = signInManagerDI;
            this._appSettingsAccess = appSettAccess;
            this._appDBContext = appDBContext;
        }

        #region ... METODOSss

        #region ..... REGISTRO .....
        [HttpPost]
        public async Task<RestMessage> SignupClient([FromBody] Cliente newclient)
        {
            try
            {
                // 1º. Usando el servicio UserManager de Identity crear nueva cuenta

                MiClienteIdentity _clientToCreate = new MiClienteIdentity()
                {
                    Nombre = newclient.Nombre,
                    Apellidos = newclient.Apellidos,
                    Descripcion = newclient.Descripcion ?? "",
                    FechaNacimiento = newclient.FechaNacimiento,
                    Email = newclient.Credentials.email,
                    UserName = newclient.Credentials.login ?? newclient.Credentials.email,
                    Genero = newclient.Genero ?? "",
                    ImagenAvatarBASE64 = newclient.Credentials.accountPic ?? "",
                    PhoneNumber = newclient.Telefono

                };

                IdentityResult _registrationResult = await this._userManagerService.CreateAsync(_clientToCreate, newclient.Credentials.password);

                if (_registrationResult.Succeeded)
                {

                    // 2º. Usando el servicio UserManager de Identity crear un token de activación de la cuenta creada y mandarla
                    // por email: la url en la que se envía este token de un solo uso generado por Identity tiene que ser
                    // una url que invoque a un método de este servicio REST <-- en el mismo debo comprobar si el token es correcto
                    // o no

                    String _emailActivationToken = await this._userManagerService.GenerateEmailConfirmationTokenAsync(_clientToCreate);
                    String _urlMail = $"https://localhost:7279/api/RESTCliente/ActivateAccount?token={HttpUtility.UrlEncode(_emailActivationToken)}&idcliente={HttpUtility.UrlEncode(_clientToCreate.Id)}";

                    String _emailMessage = $@"
                        <h3>
                            <strong>Te has registrado correctamente en Agapea.com</strong>
                        </h3>
                        <p>Pulsa en el siguiente enlace <a href={_urlMail}> ACTIVAR TU CUENTA </a> para activar tu cuenta en Agapea.</p>
                 ";
                    this._clientEmailService.SendEmail(newclient.Credentials.email,
                                                "Bienvenido al portal de Agapea.com, activa tu cuenta!!!",
                                                _emailMessage,
                                                "");
                    return new RestMessage
                    {
                        Code = 0,
                        Message = "Registro OK, se ha mandado email para activar cuenta",
                        Error = "",
                        SessionToken = "",
                        ClientData = null,
                        Others = ""
                    };
                }
                else
                {
                    throw new Exception(_registrationResult.Errors.Take(1).Select((IdentityError err) => err.Description).Single<String>());
                }

            }
            catch (Exception ex)
            {

                return new RestMessage
                {
                    Code = 1,
                    Message = "Error en el registro de la cuenta del usuario",
                    Error = ex.Message,
                    SessionToken = "",
                    ClientData = null,
                    Others = null
                };
            }

            

        }

        [HttpGet]
        public async Task ActivateAccount([FromQuery] String token, [FromQuery] String clientId)
        {
            //3º usando el servicio UserManager de Identity confirmar el token de activacion y activar cuenta si el token es correcto,
            // para eso tengo que recuperar los datos del cliente Identity asociados a ese idcliente:

            try
            {
                MiClienteIdentity _client = await this._userManagerService.FindByIdAsync(clientId);
                IdentityResult _tokenCheckResult = await this._userManagerService.ConfirmEmailAsync(_client, token);

                if (!_tokenCheckResult.Succeeded)
                {
                    //EXCEPCION DE DEV , NO DE PRODUC
                    String _error = _tokenCheckResult.Errors.Take(1).Select((IdentityError err) => err.Description).Single<String>();
                    throw new Exception("token invalido, no se ha activado la cuenta..." + _error);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error al activar la cuenta de cliente: " + ex.Message);
            }
            

        }

        #endregion

        #region ..... LOGIN .....
        [HttpGet]
        public async Task<RestMessage> LoginClient([FromQuery] string idclient)
        {
            try
            {
                MiClienteIdentity _client = await this._userManagerService.FindByIdAsync(idclient);

                Cliente _clientToReturn = new Cliente
                {
                    IdCliente = _client!.Id,
                    Nombre = _client.Nombre,
                    Apellidos = _client.Apellidos,
                    Telefono = _client.PhoneNumber ?? "",
                    Genero = _client.Genero ?? "",
                    Descripcion = _client.Descripcion ?? "",
                    FechaNacimiento = _client.FechaNacimiento,
                    Credentials = new Cuenta
                    {
                        email = _client.Email ?? "",
                        login = _client.UserName ?? "",
                        password = "",
                        accountPic = _client.ImagenAvatarBASE64 ?? ""
                    }
                };

                _clientToReturn.PedidosCliente = this._appDBContext.Pedidos.Where((Pedido p)=> p.IdCliente == idclient).ToList<Pedido>();
                _clientToReturn.DireccionesCliente = this._appDBContext.Direcciones.Where((Direccion d)=> d.IdCliente == idclient).ToList<Direccion>();

                String _jwt = this.__GenerateJWT(_client.Nombre,_client.Apellidos, _client.Email,_client.Id);

                return new RestMessage
                {
                    Code = 0,
                    Message = "Login OK, se ha mandado jwt de sesion",
                    SessionToken = _jwt,
                    Error = "",
                    ClientData = _clientToReturn,
                    Others = ""
                };

            }
            catch (Exception)
            {

                return new RestMessage
                {
                    Code = 1,
                    Message = "Incorrect Login",
                    SessionToken = "",
                    Error = "Login fallido, email o password incorrectos",
                    ClientData = null,
                    Others = ""
                };
            }
        }

        [HttpPost]
        public async Task<RestMessage> LoginClient([FromBody] Cuenta creden)
        {

            try
            {
                // 1º paso : usando servicio SignInManager de Identity comprobar credenciales recibidas desde Blazor
                //usando metodo .PasswordSignInAsync()
                MiClienteIdentity _client = await _userManagerService.FindByEmailAsync(creden.email) ?? new MiClienteIdentity();

                Cliente _returnedClient = new Cliente
                {
                    IdCliente = _client.Id,
                    Nombre = _client.Nombre,
                    Apellidos = _client.Apellidos,
                    Telefono = _client.PhoneNumber ?? "",
                    Genero = _client.Genero ?? "",
                    Descripcion = _client.Descripcion ?? "" ,
                    FechaNacimiento = _client.FechaNacimiento,
                    Credentials = new Cuenta
                    {
                        email = _client.Email ?? "",
                        login = _client.UserName ?? "",
                        password = "",
                        accountPic = _client.ImagenAvatarBASE64 ?? ""
                    }
                };

                Microsoft.AspNetCore.Identity.SignInResult _resultLogin = await _signInManager.PasswordSignInAsync(_client, creden.password, true, true);

                if (_resultLogin.Succeeded && !_resultLogin.IsNotAllowed && !_resultLogin.IsLockedOut)
                {
                    // 2º paso : generar JWT como token de sesion para cliente blazor
                    String jwt = this.__GenerateJWT(_client.Nombre,_client.Apellidos,creden.email,_client.Id);

                    return new RestMessage
                    {
                        Code = 0,
                        Message = "Login OK, se ha mandado jwt de sesion",
                        SessionToken = jwt,
                        Error = "",
                        ClientData = _returnedClient,
                        Others = ""
                    };
                }
                else
                {
                    return new RestMessage
                    {
                        Code = 1,
                        Message = "Incorrect Login",
                        SessionToken = "",
                        Error = "Login fallido, email o password incorrectos",
                        ClientData = null,
                        Others = ""
                    };
                }
                
            }
            catch (Exception ex)
            {

                return new RestMessage
                {
                    Code = 1,
                    Message = "Login FAILED",
                    SessionToken = "",
                    Error = ex.Message,
                    ClientData = null,
                    Others = ""
                };
            }
            
        }
        //si quiero restringir acceso al metodo endpoint del servicio restful en funcion si cliente blazor manda JWT
        //[HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<RestMessage> OperateAddresses()
        //{
        //}


        #endregion


        #region ...... PANEL CLIENTE 

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<RestMessage> OperateAddress([FromBody] Dictionary<string,string> addrdata)
        {
            // en addrdata Dictionary : Operacion (crear | modificar | borrar) , Direccion serializada


            //puedes extraer manualmente de la cabecera de la peticion la q sea Authorization y extraer el JWT, como alternativa al decorador Authorize 
            //==> string _jwt = this.Request.Headers["Authorization"][0].Split(" ")[1].ToString();
            // de aqui puedes extraer los claims que te interesen (nombre, apellidos, email...)

            try
            {
                Direccion _addr = JsonSerializer.Deserialize<Direccion>(addrdata["Address"]) ?? new Direccion();
                
                // CURSOR para modificar o eliminar :
                IQueryable<Direccion> _queryAddrExists = this._appDBContext.Direcciones
                                                                           .Where((Direccion d) => d.IdDireccion == _addr!.IdDireccion);
                switch (addrdata["Operation"])
                {
                    case "create":
                        this._appDBContext.Direcciones.Add(_addr);
                        break;
                    case "modify":
                        /* 2 formas de hacerlo : 
                         *  - recuperas el obj Direccion del dbset q quieres modificar y vas prop. a prop. cambiando valores 
                         *  BETTER -> cambiar todo el obj con el metodo de EntityFramework .SetValues 
                         */

                        if (_queryAddrExists.Any())
                        {
                            Direccion _addrToModify = _queryAddrExists.Single<Direccion>();
                            this._appDBContext.Entry(_addrToModify).CurrentValues.SetValues(_addr);
                        }
                        else
                        {
                            throw new Exception($"no existe ninguna direccion para modificar con el id {_addr.IdDireccion}");
                        }

                        break;
                    case "remove":
                        
                        if (_queryAddrExists.Any())
                        {
                            this._appDBContext.Direcciones.Remove(_queryAddrExists.Single<Direccion>());
                        }
                        else
                        {
                            throw new Exception($"no existe ninguna direccion para eliminar con el id {_addr.IdDireccion}");
                        }
                        break;
                    default:
                        break;
                }

                this._appDBContext.SaveChanges();

                //=> retornar obj cliente actualizado , generar JWT y devolver response
                Cliente _clientdata = await this.__GenerateUpdatedClient(_addr.IdCliente);
                return new RestMessage
                {
                    Code = 0,
                    Message = $"operacion de {addrdata["Operation"]} sobre direccion con ID {_addr.IdDireccion} realizada CORRECTAMENTE",
                    Error = "",
                    ClientData = _clientdata,
                    SessionToken = this.__GenerateJWT(_clientdata.Nombre, _clientdata.Apellidos, _clientdata.Credentials.email, _clientdata.IdCliente),
                    Others = ""

                };
            }
            catch (Exception ex)
            {

                return new RestMessage
                {
                    Code = 1,
                    Message = $"operacion de {addrdata["Operation"]} direccion NO REALIZADA, FALLIDA",
                    Error = ex.Message,
                    ClientData = null,
                    SessionToken = "",
                    Others = ""

                };
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<RestMessage> UpdateClientData([FromBody] Cliente newData)
        {
            try
            {
                MiClienteIdentity _client = await this._userManagerService.FindByIdAsync(newData.IdCliente);
                _client.PhoneNumber = newData.Telefono;
                _client.Nombre = newData.Nombre;
                _client.Apellidos = newData.Apellidos;
                _client.FechaNacimiento = newData.FechaNacimiento;
                _client.Genero = newData.Genero;
                _client.Descripcion = newData.Descripcion;
                _client.UserName = newData.Credentials.login;

                // habria que hacerlo mandando por email el token de reset de password en la url junto con el idcliente a un endpoint RESTClienteController
                // donde recoja estos params y cambie la password.... hacemos apañito de momento 

                string _tokenresetpass = await this._userManagerService
                                                   .GeneratePasswordResetTokenAsync(_client);
                
                IdentityResult _reschangepass =  await this._userManagerService
                                                           .ResetPasswordAsync(_client,
                                                                               _tokenresetpass, 
                                                                               newData.Credentials.password);
                if (!_reschangepass.Succeeded)
                {
                    
                }

                IdentityResult _resupdateClient = await this._userManagerService.UpdateAsync(_client);

                if (!_resupdateClient.Succeeded)
                {
                    throw new Exception("ERROR al modificar datos del cliente........");
                }

                Cliente _updatedClient = await this.__GenerateUpdatedClient(_client.Id);

                return new RestMessage
                {
                    Code = 0,
                    Message = "DATOS DEL CLIENTE MODIFICADOS",
                    Error = "",
                    ClientData = _updatedClient,
                    SessionToken = this.__GenerateJWT(_updatedClient.Nombre,_updatedClient.Apellidos, _updatedClient.Credentials.email, _updatedClient.IdCliente),
                    Others = ""

                };
            }
            catch (Exception ex)
            {

                return new RestMessage
                {
                    Code = 1,
                    Message = "ERROR AL MODIFICAR DATOS DE LA CUENTA CLIENTE",
                    Error = ex.Message,
                    ClientData = null,
                    SessionToken = "",
                    Others = ""

                };
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<RestMessage> UploadImage([FromBody] Dictionary<string, string> data)
        {
            try
            {
                MiClienteIdentity _client = await this._userManagerService.FindByIdAsync(data["IdCliente"]);
                _client.ImagenAvatarBASE64 = data["Imagen"];

                IdentityResult _resupdate = await this._userManagerService.UpdateAsync(_client);

                if (!_resupdate.Succeeded)
                {
                    throw new Exception("FALLO al intentar subir imagen de avatar");
                }
                Cliente _updatedClient = await this.__GenerateUpdatedClient(_client.Id);

                return new RestMessage
                {
                    Code = 0,
                    Message = "IMAGEN DE CUENTA ACTUALIZADA",
                    Error = "",
                    ClientData = _updatedClient,
                    SessionToken = this.__GenerateJWT(_updatedClient.Nombre, _updatedClient.Apellidos, _updatedClient.Credentials.email, _updatedClient.IdCliente),
                    Others = ""

                };
            }
            catch (Exception ex)
            {


                return new RestMessage
                {
                    Code = 1,
                    Message = "ERROR AL SUBIR IMAGEN DE CUENTA",
                    Error = ex.Message,
                    ClientData = null,
                    SessionToken = "",
                    Others = ""

                };
            }
        }
        #endregion


        private String __GenerateJWT(String nombre, String apellidos, String email, String idcliente)
        {
            SecurityKey _signKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this._appSettingsAccess["JWT:signature"]));

            JwtSecurityToken _jwt = new JwtSecurityToken(

                                                            issuer: this._appSettingsAccess["JWT:issuer"],
                                                            audience: null,
                                                            claims: new List<Claim>
                                                                    {
                                                                        new Claim ("name",nombre),
                                                                        new Claim ("email",email),
                                                                        new Claim ("lastname", apellidos),
                                                                        new Claim ("id", idcliente)

                                                                    },
                                                            notBefore: null,
                                                            expires: DateTime.Now.AddHours(2),
                                                            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(_signKey, SecurityAlgorithms.HmacSha256)
                                                        );

            string _token = new JwtSecurityTokenHandler().WriteToken(_jwt);
            return _token;
        }

        private async Task<Cliente> __GenerateUpdatedClient(string clientid)
        {
            MiClienteIdentity _client = await this._userManagerService.FindByIdAsync(clientid);
            Cliente _clientdata = new Cliente
            {
                IdCliente = _client!.Id,
                Nombre = _client.Nombre,
                Apellidos = _client.Apellidos,
                Telefono = _client.PhoneNumber ?? "",
                Genero = _client.Genero ?? "",
                Descripcion = _client.Descripcion ?? "",
                FechaNacimiento = _client.FechaNacimiento,
                Credentials = new Cuenta
                {
                    email = _client.Email ?? "",
                    login = _client.UserName ?? "",
                    password = "",
                    accountPic = _client.ImagenAvatarBASE64 ?? ""
                }
            };

            _clientdata.PedidosCliente = this._appDBContext.Pedidos.Where((Pedido p) => p.IdCliente == clientid).ToList<Pedido>();
            _clientdata.DireccionesCliente = this._appDBContext.Direcciones.Where((Direccion d) => d.IdCliente == clientid).ToList<Direccion>();

            return _clientdata;
        }

        #endregion

    }
}

