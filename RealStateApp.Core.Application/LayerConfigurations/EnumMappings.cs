using RealStateApp.Core.Application.Helpers;
using RealStateApp.Core.Domain.Common.Enums;

namespace RealStateApp.Core.Application.LayerConfigurations
{
    public class EnumMappings
    {
        public static void Initialize()
        {



            EnumMapper<AppRoles>.AddAliases(new()
            {
                { "administrador", AppRoles.ADMIN },
                { "admin", AppRoles.ADMIN },

                { "agente", AppRoles.AGENT },
                { "agent", AppRoles.AGENT },

                { "cliente", AppRoles.CLIENT },
                { "client", AppRoles.CLIENT },
                { "desarrollador", AppRoles.DEVELOPER },
                { "developer", AppRoles.DEVELOPER },


            });


        }

    }
}

