using Core.Data.Dtos.Security;
using Core.Repository.Base;
using Core.Repository.Extensions;

namespace Core.Repository
{
    public class AuthentificationConfiguration : ConfigurationBase
    {
        private readonly string JwtAuthentificationSection = "Jwt";

        public JwtConfiguration GetJwtSettings()
        {
            return GetConfiguration().GetConfig<JwtConfiguration>(JwtAuthentificationSection);
        }
    }
}
