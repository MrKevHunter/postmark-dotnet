﻿using Xunit;
using PostmarkDotNet;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Postmark.Tests
{
    public abstract class ClientBaseFixture
    {
        private static string AssemblyLocation = typeof(ClientBaseFixture).GetTypeInfo().Assembly.Location;

        private static void AssertSettingsAvailable()
        {
            Assert.NotNull(READ_INBOUND_TEST_SERVER_TOKEN);
            Assert.NotNull(READ_SELENIUM_TEST_SERVER_TOKEN);
            Assert.NotNull(READ_SELENIUM_OPEN_TRACKING_TOKEN);
            Assert.NotNull(READ_LINK_TRACKING_TEST_SERVER_TOKEN);
            
            Assert.NotNull(WRITE_ACCOUNT_TOKEN);
            Assert.NotNull(WRITE_TEST_SERVER_TOKEN);
            Assert.NotNull(WRITE_TEST_SENDER_EMAIL_ADDRESS);
            Assert.NotNull(WRITE_TEST_EMAIL_RECIPIENT_ADDRESS);
            Assert.NotNull(WRITE_TEST_SENDER_SIGNATURE_PROTOTYPE);
            Assert.NotNull(BASE_URL);
        }

        /// <summary>
        /// Retrieve the config variable from the environment, 
        /// or app.config if the environment doesn't specify it.
        /// </summary>
        /// <param name="variableName">The name of the environment variable to get.</param>
        /// <returns></returns>
        private static string ConfigVariable(string variableName)
        {
            string retval = null;
            //this is here to allow us to have a config that isn't committed to source control, but still allows the project to build
            try
            {   
                var location = Path.GetFullPath(AssemblyLocation);
                var pathComponents = location.Split(Path.DirectorySeparatorChar).ToList();
                var componentsCount = pathComponents.Count;
                var keyPath = "";
                while(componentsCount > 0){
                    keyPath = Path.Combine(new string[]{ Path.DirectorySeparatorChar.ToString() }
                        .Concat(pathComponents.Take(componentsCount)
                        .Concat(new string[] { "testing_keys.json" })).ToArray());
                    if(File.Exists(keyPath)){
                        break;
                    }
                    componentsCount--;
                }

                var values = JsonConvert.DeserializeObject<Dictionary<String, String>>(File.ReadAllText(keyPath));
                retval = values[variableName];
            }
            catch
            {
                //This is OK, it just doesn't exist.. no big deal.
            }
            return string.IsNullOrWhiteSpace(retval) ? Environment.GetEnvironmentVariable(variableName) : retval;
        }

        public static readonly DateTime TESTING_DATE = DateTime.Now;

        public static readonly string READ_INBOUND_TEST_SERVER_TOKEN = ConfigVariable("READ_INBOUND_TEST_SERVER_TOKEN");
        public static readonly string READ_SELENIUM_TEST_SERVER_TOKEN = ConfigVariable("READ_SELENIUM_TEST_SERVER_TOKEN");
        public static readonly string READ_LINK_TRACKING_TEST_SERVER_TOKEN = ConfigVariable("READ_LINK_TRACKING_TEST_SERVER_TOKEN");
        public static readonly string READ_SELENIUM_OPEN_TRACKING_TOKEN = ConfigVariable("READ_SELENIUM_OPEN_TRACKING_TOKEN");

        public static readonly string WRITE_ACCOUNT_TOKEN = ConfigVariable("WRITE_ACCOUNT_TOKEN");
        public static readonly string WRITE_TEST_SERVER_TOKEN = ConfigVariable("WRITE_TEST_SERVER_TOKEN");
        public static readonly string WRITE_TEST_SENDER_EMAIL_ADDRESS = ConfigVariable("WRITE_TEST_SENDER_EMAIL_ADDRESS");
        public static readonly string WRITE_TEST_EMAIL_RECIPIENT_ADDRESS = ConfigVariable("WRITE_TEST_EMAIL_RECIPIENT_ADDRESS");
        public static readonly string WRITE_TEST_SENDER_SIGNATURE_PROTOTYPE = ConfigVariable("WRITE_TEST_SENDER_SIGNATURE_PROTOTYPE");
        
        public static readonly string BASE_URL = ConfigVariable("BASE_URL");

        protected PostmarkClient _client;

        public ClientBaseFixture()
        {
            AssertSettingsAvailable();
            Setup();
        }

        protected abstract void Setup();
    }
}
