using Agapea_Blazor.Server.Models.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace Agapea_Blazor.Server.Models.Services
{
    public class MailJetService : IClienteCorreo
    {
        #region ...props

        private IConfiguration _appSettingsAccess;
        public string UserId { get; set; }
        public string Key { get; set; }

        #endregion

        public MailJetService(IConfiguration appSettingsAccessService)
        {
            this._appSettingsAccess = appSettingsAccessService;
            this.UserId = this._appSettingsAccess.GetSection("MailJetCredentials:ApiKey").Value!;
            this.Key = this._appSettingsAccess.GetSection("MailJetCredentials:SecretKey").Value!;

        }

        #region ...metodos

        public bool SendEmail(string clientEmail, string subject, string messageBody, string? attachedFile)
        {
            try
            {
                // 1º abrir socket al servidor SMTP de mailjet con las credenciales de la api q te dan al registrarte
                // usando la clave SmtpClient
                SmtpClient _clienteSMTP = new SmtpClient("in-v3.mailjet.com");
                _clienteSMTP.Credentials = new NetworkCredential(this.UserId, this.Key);

                // 2º crear el cuerpo del mensaje de correo a mandar al cliente <-- clase MailMessage
                MailMessage _mailMessage = new MailMessage("cunnst@gmail.com", clientEmail);
                _mailMessage.Subject = subject;
                _mailMessage.IsBodyHtml = true; // si quieres incrustar en el body tags html como links, img...etc
                _mailMessage.Body = messageBody;

                if (!String.IsNullOrEmpty(attachedFile))
                {
                    MemoryStream ms = new MemoryStream();
                    using (FileStream fs = new FileStream(attachedFile, FileMode.Open, FileAccess.Read)) fs.CopyTo(ms);
                    _mailMessage.Attachments.Add(new Attachment(ms, "application/pdf"));
                }


                // 3º mandar mail usando el socket abierto con cliente smtp creado <--- metodo .send de la clase SmtpClient

                _clienteSMTP.Send(_mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            #endregion
        }
    }
}
