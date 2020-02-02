using Core.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration.Json;

namespace Core.Repository
{
    public class DatabaseConfiguration : ConfigurationBase
    {
        private string DataConnectionKey = "dataConnection";
        private string AuthConnectionKey = "authConnection";
        private string MongoConnectionKey = "mongoConnection";
        private string LoggingConnectionKey = "loggingConnection";

        public string GetDataConnectionString()
        {
            return GetConfiguration().GetConnectionString(DataConnectionKey);
        }

        public string GetAuthConnectionString()
        {
            return GetConfiguration().GetConnectionString(AuthConnectionKey);
        }

        public string GetMongoConnectionString()
        {
            return GetConfiguration().GetConnectionString(MongoConnectionKey);
        }

        public string GetLoggingConnectionString()
        {
            string ret = GetConfiguration().GetConnectionString(LoggingConnectionKey);
            return ret;
        }
    }
}
