namespace App.Routes
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;
        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string LDAPLogin = Base + "/identity/ldapLogin";
            public const string LDAP = Base + "/identity/ldap";

            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";

            public const string GetUsers = Base + "/admin/getUsers";
            public const string GetUsersInRoleAsync = Base + "/admin/getUsersInRoleAsync";

            
            public const string GetRolesAsync = Base + "/admin/getRolesAsync";
            public const string AddPolicy = Base + "/admin/addPolicy";
            public const string CreateRole = Base + "/admin/createRole";
            public const string AddToRole = Base + "/admin/addToRole";
            public const string RemoveToRole = Base + "/admin/removeToRole";
            public const string GetRoles = Base + "/admin/getRoles";

            public const string ResetPasswordAsync = Base + "/admin/resetPasswordAsync";


        }
        public static class Business{
            public const string GetUsers = Base + "/customer/getUsers";
        }
    }
}