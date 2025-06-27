using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSSLBypass
{
    public static class Constants
    {
        public static string SSLBYPASS_DLL_QNAME = "System.Net.Security.SecureChannel, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public static string SSLBYPASS_FUNCTION = "VerifyRemoteCertificate";
    }
}
