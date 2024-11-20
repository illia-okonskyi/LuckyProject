using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;

namespace LuckyProject.Lib.Web.Helpers
{
    public static class LpApiClientTypeHelper
    {
        public static LpApiClientType GetClientType(string clientId)
        {
            return clientId.StartsWith(LpWebConstants.ApiClients.WebPrefix)
                ? LpApiClientType.Web
                : LpApiClientType.Machine;
        }

        public static string GetClientId(LpApiClientType type, string name)
        {
            return $"{GetClientTypePrefix(type)}{name}";
        }

        public static string GetClientTypePrefix(LpApiClientType type)
        {
            return type == LpApiClientType.Web
                ? LpWebConstants.ApiClients.WebPrefix
                : LpWebConstants.ApiClients.MachinePrefix;
        }

        public static string GetClientName(string clientId)
        {
            return clientId[4..];
        }
    }
}
