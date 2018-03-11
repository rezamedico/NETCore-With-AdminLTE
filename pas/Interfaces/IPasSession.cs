using pas.ViewModels;
namespace pas.Interfaces
{
    public interface IPasSession
    {
        UserSession GetContract { get; }
        UserSession SetContract { set; }
        void ClearSession();
        bool IfContractAvailable();
        string UserId { get; }
        string RoleId { get; }
        string Username { get; }
        string FullName { get; }
        string RoleName { get; }
    }
}
