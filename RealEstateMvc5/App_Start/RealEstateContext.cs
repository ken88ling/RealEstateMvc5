using MongoDB.Driver;
using RealEstateMvc5.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateMvc5.App_Start
{
    public class RealEstateContext
    {
        public MongoDatabase Database { get; set; }

        public RealEstateContext()
        {
            var client = new MongoClient(Settings.Default.RealEstateConnectionString);
            var server = client.GetServer();
            Database = server.GetDatabase(Settings.Default.RealEstateDatabaseName);
        }
    }
}

