namespace IdentityServer.Management.Registration
{
    public class RegistrationContext
    {
        public RegistrationContext(string registrationUrl)
        {
            RegistrationUrl = registrationUrl;
        }
        public string RegistrationUrl { get; }
    }
}
