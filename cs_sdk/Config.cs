using System;
namespace BlinkPayCard
{
    public class BlinkPayCardConfig
    {
        private static volatile IConfig config;
        private static object syncRoot = new object();

        public static IConfig GetConfig()
        {
            if (config == null)
            {
                lock (syncRoot)
                {
                    if (config == null)
                        config = new DemoConfig();
                }
            }
            return config;
        }
    }
}
