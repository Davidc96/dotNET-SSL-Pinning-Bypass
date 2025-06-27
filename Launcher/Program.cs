using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using libSSLBypass;

namespace Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Binary .NET Launcher and Patcher";
            if (args.Length == 0)
            {
                Console.WriteLine("usage launcher.exe <binary>");
                return;
            }
            try
            {
                string targetExePath = args[0];

                if (!File.Exists(targetExePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERR] Cannot find specific binnary '{targetExePath}'");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("[+] Applying all patches to the main program");

                // Apply all patches
                PatchSSL.Apply();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] All patches applied!!");
                Console.ResetColor();

                // --- Parte 2: Cargar y ejecutar el .exe en un hilo separado ---

                Console.WriteLine($"[+] Loading Assembly '{Path.GetFileName(targetExePath)}' into memory...");
                // Load Binary as an Assembly (Like Reference)
                Assembly targetAssembly = Assembly.LoadFrom(targetExePath);

                // Find the EntryPoint
                MethodInfo targetMain = targetAssembly.EntryPoint;


                if (targetMain == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERR] Cannot find the entryPoint MAIN");
                    Console.ReadKey();
                    return;
                }
                ParameterInfo[] parameters = targetMain.GetParameters();
                object[] argumentsToPass;

                if (parameters.Length == 0)
                {
                    argumentsToPass = null;
                }
                else
                {
                    argumentsToPass = new object[] { new string[0] };
                }

                Console.WriteLine("[+] Creating a thread to execute the binary main function....");

                // Let's create a Thread which will execute our Main function
                Thread workerThread = new Thread(() =>
                {
                    try
                    {
                        // Invoke the entrypoint with the arguments.
                        targetMain.Invoke(null, argumentsToPass);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[EXCEPTION] Failed to create a thread: {ex.InnerException}");
                    }
                });

                workerThread.IsBackground = true; // Send a thread into the background to avoid being closed
                workerThread.Start(); // Init Thread

                Console.WriteLine("\n=======================================================");
                Console.WriteLine("Binary Launched successfully.");
                Console.WriteLine("Press ENTER to end it");
                Console.WriteLine("=======================================================");
                Console.ReadLine();

                Console.WriteLine("[+] Launched ended ");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Something goes wrong: {ex.Message}");
            }
        }
    }
}
