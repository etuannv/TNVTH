using System;
namespace HTTT.HaTangApp.Models
{
    public interface IHaTangAppRepositiory
    {
        // Bien ban giao ca
        System.Collections.Generic.IEnumerable<BienBanGiaoCa> ListBienBanGiaoCas();
        BienBanGiaoCa CreateBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToCreate);
        void DeleteBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToDelete);
        HTTT.Utilities.ReturnValue<bool> DeleteUser(aspnet_Users userToDelete);
        BienBanGiaoCa EditBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToEdit);
        BienBanGiaoCa GetBienBanGiaoCa(int id);

        //User
        aspnet_Users GetUser(Guid userId);
        System.Collections.Generic.IEnumerable<aspnet_Users> ListUsers();
    }
}
