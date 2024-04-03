namespace Agapea_Blazor.Server.Models.Services.Interfaces
{
    public interface IClienteCorreo
    {

        #region ...props de la interface que deben implementar servicios para mandar mails
        //como props : _ User con el que te conectas al servicio de proveedor externo de correo 
        //             _ Password con la que te autentificas  
        public String UserId { get; set; }
        public String Key { get; set; }
        #endregion

        #region ...metodos de la interface que deben implementar servicios para mandar mails
        public bool SendEmail(String clientEmail, String subject, String messageBody, String? attachedFile);
        #endregion
    }
}
