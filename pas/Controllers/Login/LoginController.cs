using System;
using Microsoft.AspNetCore.Mvc;
using pas.ViewModels;
using pas.Interfaces;
using pas.Repository;
using Microsoft.AspNetCore.Authorization;

namespace pas.Controllers
{
    public class LoginController : Controller
    {
        private LoginRepository _loginRepo;
        IPasSession _session;

        public LoginController(IPasSession session)
        {
            _session = session;
            _loginRepo = new LoginRepository();
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            _session.ClearSession();
            return View();
        }

        [AllowAnonymous]        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {            
            string messages = string.Empty;
            pas.Models.User user = null;
            try
            {
                user = _loginRepo.LoginValidation(model);
                if (user == null)
                {
                    _session.ClearSession();
                    messages = "Please Input Valid Username And Password";
                }
                else
                {
                    //Set The Session
                    UserSession _userSession = new UserSession();
                    _userSession.UserId = user.UserId;
                    _userSession.Username = user.Username;
                    _userSession.RoleId = user.RoleId;
                    _userSession.RoleName = user.RoleName;
                    _userSession.FullName = user.FullName;                    
                    _session.SetContract = _userSession;
                    messages = "Username And Password Match";
                }
            }
            catch (Exception ex)
            {
                user = null;
                messages = ex.Message;
            }

            return RedirectToAction("Index", "Home");            
        }

        public IActionResult Logout()
        {
            _session.ClearSession();
            return RedirectToAction("Index");
        }

    }
}
