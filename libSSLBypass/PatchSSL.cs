using HarmonyLib;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libSSLBypass
{
    public class PatchSSL
    {
        private static bool isPatched = false;
        
        public static void Log(string message)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
            catch { }
        }

        /// <summary>
        /// Used to initialice Harmony to patch SSL Pinning as System.Net.Security.dll level
        /// Only executed once to avoid errors
        /// </summary>
        public static void Apply()
        {
            if (isPatched)
            {
                Log("[SSLBypass] Patch already applied...");
                return;
            }
            try
            {
                var harmony = new Harmony("com.davidc96.ssl.pinning.bypass");

                Type secureChannelType = Type.GetType(Constants.SSLBYPASS_DLL_QNAME);
                var originalMethod = secureChannelType.GetMethod(Constants.SSLBYPASS_FUNCTION, BindingFlags.Instance | BindingFlags.NonPublic);
              
                if (originalMethod == null)
                {
                    // If method is not found, maybe Microsoft change the function name, better look it manually
                    Log("[SSLBypass] Cannot Find VerifyRemoteCertificate... Maybe the name has changed in a new update System.Net.Security.SecureChannel");
                    Log("[SSLBypass] It is recommended to use Utility tool to find the correct function name and change it in Constants.cs");
                    return;
                }

                // Get postfixMethod to patch it
                var postfixMethod = new HarmonyMethod(typeof(SecureChannelPatches), nameof(SecureChannelPatches.VerifyRemoteCertificate_Postfix));

                harmony.Patch(originalMethod, postfix: postfixMethod);

                isPatched = true;
                Log("[SSLBypass] Successfully Patched!");
            }
            catch (Exception ex)
            {
                Log($"[SSLBypass] Something goes wrong: {ex}");
            }
        }
    }

    /// <summary>
    /// Internal class used by Harmony to patch the function
    /// </summary>
    internal static class SecureChannelPatches
    {
        /// <summary>
        /// Method executed after VerifyRemoteCertificate is called
        /// It sets the return function to True despite the result
        /// </summary>
        /// <param name="__result">Value original returned and then modified</param>
        internal static void VerifyRemoteCertificate_Postfix(ref bool __result)
        {

            if (!__result)
            {
                PatchSSL.Log("[SSLBypass] Patching the result to True!");
                __result = true;
            }
        }
    }
}
