using System.Linq;
using System.Net;
using System.Web;

namespace Teeyoot.Localization.IpAddress
{
    public class WebIpAddressProvider : IIpAddressProvider
    {
        public string GetIpAddress()
        {
            var forwarded = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            var userHostAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(forwarded))
                return userHostAddress;

            var forwardedIpAddresses = forwarded.Split(',');
            var publicForwardedIpAddresses = forwardedIpAddresses.Where(ip => !IsPrivateIpAddress(ip)).ToList();

            return publicForwardedIpAddresses.Any() ? publicForwardedIpAddresses.Last() : userHostAddress;
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock)
                return true;

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock)
                return true;

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock)
                return true;

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }
    }
}
