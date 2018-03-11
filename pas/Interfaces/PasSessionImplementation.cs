using Microsoft.AspNetCore.Http;
using pas.Extensions;
using pas.ViewModels;

namespace pas.Interfaces
{
    public class PasSessionImplementation : IPasSession
    {
        readonly IHttpContextAccessor _accessor;
        UserSession _sessionContract;

        public PasSessionImplementation(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            _sessionContract = _accessor.HttpContext.Session.GetSession<UserSession>("PasSession");
        }

        public UserSession GetContract => _accessor.HttpContext.Session.GetSession<UserSession>("PasSession");

        public UserSession SetContract
        {
            set
            {
                ClearSession();

                _sessionContract = value;
                _accessor.HttpContext.Session.SetSession("PasSession", _sessionContract);
            }
        }

        public string UserId
        {
            get
            {
                if (IfContractAvailable())
                {
                    return _sessionContract.UserId;
                }
                return null;
            }
        }

        public string RoleId
        {
            get
            {
                if (IfContractAvailable())
                {
                    return _sessionContract.RoleId;
                }
                return null;
            }
        }

        public string Username
        {
            get
            {
                if (IfContractAvailable())
                {
                    return _sessionContract.Username;
                }
                return null;
            }
        }

        public string FullName
        {
            get
            {
                if (IfContractAvailable())
                {
                    return _sessionContract.FullName;
                }
                return null;
            }
        }

        public string RoleName
        {
            get
            {
                if (IfContractAvailable())
                {
                    return _sessionContract.RoleName;
                }
                return null;
            }
        }

        public void ClearSession()
        {
            _accessor.HttpContext.Session.Clear();
        }

        public bool IfContractAvailable()
        {
            _sessionContract = _accessor.HttpContext.Session.GetSession<UserSession>("PasSession");
            if (_sessionContract == null) { return false; }
            return true;
        }

    }
}
