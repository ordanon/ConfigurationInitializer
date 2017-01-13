namespace ConfigurationUtils.Azure
{
    public class AzureRoleConfigurationInitializer : ConfigurationInitializer
    {
        public AzureRoleConfigurationInitializer() : base(new RoleConfigurationReader())
        {
        }
    }
}
