using pas.Models;
using pas.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pas.Repository
{
    public class LoginRepository
    {
        //Private Field
        private pas_db _db;
        string exception = string.Empty;
        //Default Constructor
        public LoginRepository()
        {
            _db = new pas_db();
        }

        public User LoginValidation(LoginViewModel model)
        {
            User o = new User();
            try
            {
                o = _db.Users.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
            }
            catch (Exception ex)
            {
                o = null;
                exception = ex.Message;
            }

            return o;
        }
    }
}
