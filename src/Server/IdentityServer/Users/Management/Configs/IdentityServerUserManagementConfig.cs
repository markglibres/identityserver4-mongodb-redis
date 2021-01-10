namespace IdentityServer.Users.Management.Configs
{
    public class IdentityServerUserManagementConfig
    {
        public IdentityServerUserManagementConfig()
        {
            Paths = new PathConfig
            {
                CreateUserPath = "/Registration/CreateUser"
            };
        }
        public ConfirmationEmailConfig ConfirmationEmail { get; set; }
        public string BaseUrl { get; set; }
        public PathConfig Paths { get; set; }
    }

    public class ConfirmationEmailConfig
    {
        public bool Require { get; set; }
        public string Subject { get; set; }
    }

    public class PathConfig
    {
        public string CreateUserPath { get; set; }
    }
}
