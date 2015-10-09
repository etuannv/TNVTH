using System;
using HTTT.HaTangApp.Models.ViewModel;
namespace HTTT.HaTangApp.Models
{
    public interface IHaTangAppServices
    {
        //Bien ban giao ca
        bool CreateBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToCreate);
        bool DeleteBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToDelete);
        bool EditBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToEdit);
        BienBanGiaoCa GetBienBanGiaoCa(int id);
        System.Collections.Generic.IEnumerable<BienBanGiaoCa> ListBienBanGiaoCa();

        //User
        bool CreateUser(UserViewModel userToCreate);
        aspnet_Users GetUser(Guid userId);
        System.Collections.Generic.IEnumerable<aspnet_Users> ListUsers();
    }
}
