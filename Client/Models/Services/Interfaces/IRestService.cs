using Agapea_Blazor.Shared;

namespace Agapea_Blazor.Client.Models.Services.Interfaces
{
    public interface IRestService
    {
        //interfaz que sirve como patrón para definir servicios a inyectar en Program.cs para hacer peticiones AJAX a servicios REST externos
        #region ...METODOS/PROPS ZONA CLIENTE

        Task<RestMessage> SignupClient(Cliente newClient);
        Task<RestMessage> LoginClient(Cuenta credentials);
        Task<RestMessage> LoginClient(String idcliente);
        Task<RestMessage> OperateAddress(Direccion addr , string operation);
        Task<RestMessage> UploadImage(String imgbase64, String idcliente);
        Task<RestMessage> UpdateClientData(Cliente newClient);
        #endregion

        #region ...METODOS/PROPS ZONA TIENDA
        Task <List<Libro>> RetrieveBooks(String catID);
        Task<Libro> RetrieveSingleBook(String isbn13);
        Task<List<Categoria>> RetrieveCategories(String catID);
        Task <List<Provincia>> RetrieveProvincias();
        Task<List<Municipio>> RetrieveMunicipios(String codpro);
        Task<String> CompleteOrder(OrderModel orderData, Pedido newOrder);
        #endregion
    }
}
