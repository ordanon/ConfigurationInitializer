using System;
using System.Collections.Generic;

namespace ConfigurationUtils.UnitTests
{
    public class TestConfiguration : ConfigurationInitializer
    {
        public TestConfiguration()
        {
        }

        public TestConfiguration(IConfigurationReader configurationReader) : base(configurationReader)
        {
        }

        public string SchoolName { get; set; }

        [ConfigKey("School.LunchBreakPeriod")]
        public TimeSpan BreakPeriod { get; set; }

        public List<string> Students { get; set; }
    }
}
