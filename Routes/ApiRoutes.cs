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
            //Customer Routes
            public const string addCustomer = Base + "/customer/addCustomer";
            public const string updateCustomer = Base + "/customer/updateCustomer";
            public const string deleteCustomer = Base + "/customer/deleteCustomer";
            public const string getCustomer = Base + "/customer/getCustomer";
            public const string findCustomerById = Base + "/customer/findCustomerById";

            //Product routes
            public const string addProduct = Base + "/product/addProduct";
            public const string updateProduct = Base + "/product/updateProduct";
            public const string deleteProduct = Base + "/product/deleteProduct";
            public const string getProduct = Base + "/product/getProduct";
            public const string findProductById = Base + "/product/findProductById";

            //Sale routes
            public const string addSale = Base + "/sale/addSale";
            public const string updateSale = Base + "/sale/updateSale";
            public const string deleteSale = Base + "/sale/deleteSale";
            public const string getSale = Base + "/sale/getSale";
            public const string findSaleById = Base + "/sale/findSaleById";
            public const string findSaleHeaderById = Base + "/sale/findSaleHeaderById";
        }
    }
}