using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HTTT.Utilities;

namespace HTTT.HaTangApp.Models
{
    public class HaTangAppRepositiory : HTTT.HaTangApp.Models.IHaTangAppRepositiory
    {
        private HaTangAppDBEntities _entities = new HaTangAppDBEntities();

        // BienBanGiaoCa methods
        public IEnumerable<BienBanGiaoCa> ListBienBanGiaoCas()
        {
            return (from c in _entities.BienBanGiaoCas
                    select c);
        }
        public BienBanGiaoCa GetBienBanGiaoCa(int id)
        {
            return (from c in _entities.BienBanGiaoCas
                    where c.Id == id
                    select c).FirstOrDefault();
        }

        public BienBanGiaoCa CreateBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToCreate)
        {
            // Save new contact
            _entities.AddToBienBanGiaoCas(bienBanBanGiaoCaToCreate);
            _entities.SaveChanges();
            return bienBanBanGiaoCaToCreate;
        }

        public BienBanGiaoCa EditBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToEdit)
        {
            // Get original contact
            var originalBienBanGiaoCa = GetBienBanGiaoCa(bienBanBanGiaoCaToEdit.Id);

            // Save changes
            _entities.ApplyCurrentValues(originalBienBanGiaoCa.EntityKey.EntitySetName, bienBanBanGiaoCaToEdit);
            _entities.SaveChanges();
            return bienBanBanGiaoCaToEdit;
        }

        public void DeleteBienBanGiaoCa(BienBanGiaoCa bienBanBanGiaoCaToDelete)
        {
            var originalBienBanGiaoCa = GetBienBanGiaoCa(bienBanBanGiaoCaToDelete.Id);
            _entities.DeleteObject(originalBienBanGiaoCa);
            _entities.SaveChanges();
        }

        
        
        // User Methods
        public IEnumerable<aspnet_Users> ListUsers()
        {
            return _entities.aspnet_Users.ToList();
        }

       
        public aspnet_Users GetUser(Guid userId)
        {
            return (from g in _entities.aspnet_Users
                    where g.UserId == userId
                    select g).FirstOrDefault();
        }

        public ReturnValue<bool> DeleteUser(aspnet_Users userToDelete)
        {
            return new ReturnValue<bool>(false, "Not implemented");

        }

    }
}