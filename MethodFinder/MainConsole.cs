using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MethodFinder
{
    internal class MainConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Utility Tool to find the method in System.Net.Security.SecureChannel");
            Console.WriteLine("[+] Extracting AssemblyQualifiedName from System.Net.Security.dll...");
            string qName = GetSysInternalAssemblyQualifiedName("System.Net.Security.SecureChannel");
            if(qName == null)
            {
                Console.WriteLine("[-] Something goes wrong.... Exiting...");
                return;
            }

            Console.WriteLine("[+] QualifiedName Found!, getting all methods which return bool and are private...");
            GetMethods(qName);
            Console.WriteLine("");
            Console.WriteLine("[i] From the list choose the method which could be something similar to VerifyRemoteCertificate and change it in SSLBYPASS_FUNCTION at Constants.cs");
            Console.WriteLine("");
            PrintResults(qName);
            Console.Read();
        }

        private static void PrintResults(string qName)
        {
            Console.WriteLine("[+] Copy and paste this in Constants.cs");
            Console.WriteLine("----------------------------------------\n");
            Console.WriteLine("public static class Constants\n{");
            Console.WriteLine("   public static string SSLBYPASS_DLL_QNAME = \"" + qName + "\";");
            Console.WriteLine("   public static string SSLBYPASS_FUNCTION = \"YourFunctionManuallyFinded\"; <- CHANGE THIS AND REMOVE THIS MESSAGE");
            Console.WriteLine("}\n");
            Console.WriteLine("-----------------------------------------");
        }

        private static string GetSysInternalAssemblyQualifiedName(string internalTypeName)
        {
            try
            {
                // First, let's find parent DLL, on this case is System.dll and then use Uri as an Entrypoint to find what we want.
                Assembly systemAssembly = typeof(System.Uri).Assembly;

                // Because it's internal, we need to use GetType to get the propertyu.
                Type internalClassType = systemAssembly.GetType(internalTypeName);

                if (internalClassType != null)
                {
                    // When we have it, we can now obtain it
                    string assemblyQualifiedName = internalClassType.AssemblyQualifiedName;

                    return assemblyQualifiedName;
                }
                else
                {
                    Console.WriteLine($"[-] Cannot find {internalTypeName} in {systemAssembly.GetName()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION!!] Something goes wrong: {ex.Message}");
            }

            return "";
        }

        private static void GetMethods(string qName)
        {
            Type secureChannelType = Type.GetType(qName);
            var allMethods = secureChannelType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            if (allMethods != null)
            {
                Console.WriteLine("[+] Listing methods...\n");
                Console.WriteLine(" --------------------------\n");
                foreach (var method in allMethods)
                {
                    if(method.ReturnType.Name.Contains("Bool") && !(method.Name.Contains("set") || method.Name.Contains("get")))
                    {
                        Console.WriteLine("  "+method.Name);
                    }
                }
                Console.WriteLine("\n --------------------------");
            }         
        }
    }
}
