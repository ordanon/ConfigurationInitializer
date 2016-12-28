# Configuration Initializer
Initialize settings instance, managing configuration decryption and type conversion.

## Available on NuGet Gallery

To install the [ConfigurationInitializer](https://www.nuget.org/packages/ConfigurationInitializer/) package,
run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

    PM> Install-Package ConfigurationInitializer
    
### Usage samples ###

    public class SchoolConfiguration : ConfigurationInitializer
    {
        // Config key name is evaluated to the property name by default.
        public string SchoolName { get; set; }

        // Example for setting the property's correlated config key.   
        [ConfigKey("School.LunchBreakPeriod")]
        public TimeSpan BreakPeriod { get; set; }

        // Conig JSON value is deseriliazed into complex objects. 
        public List<string> Students { get; set; }
    }
    
    public class School {
        public static Main(){
            // The config instance is auto populated, decrypting secrets and converting types.
            var config = new SchoolConfiguration();
            
            // The configuration is now available for the rest of the application.
            Console.Out.WriteLine($"Schole name = {config.SchoolName}, break period = {config.BreakPeriod}");
        }
    }
