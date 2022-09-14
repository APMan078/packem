namespace Packem.Domain.Common.Enums
{
    public enum RoleEnum
    {
        SuperAdmin = 1, // us, can do everything
        Admin = 2,// limited to the customer data
        OpsManager = 3, // login to web, cannot import
        Operator = 4, // mobile user only
        Viewer = 5 // web, view only, cannot do action. For now limit only to inventory
    }
}