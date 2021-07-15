using System;

namespace Common
{
    public class DummyData
    {
        public string[] FirstNames { get; set; } = { "mohamad", "ali", "hasan", "hosein", "navid", "saeid", "amir", "omid", "vahid", "hamid" };
        public string[] LastNames { get; set; } = { "mohamadi", "alavi", "hasani", "hoseini", "navidi", "saeidi", "amiri", "omidi", "javadi", "hamidi" };
        public string GenerateName()
        {
            return FirstNames[(new Random()).Next(0, FirstNames.Length)];
        }
        public string GenerateLastName()
        {
            return LastNames[(new Random()).Next(0, LastNames.Length)];
        }
    }

}
