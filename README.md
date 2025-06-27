# .NET-SSL-Pinning-Bypass
This project borns as a result of my investigation about how can bypass an SSL Pinning in a .NET Framework application.

It is true that it can be easily bypassable if we use a tool like DNSpy which allows us to edit the binary directly, but, what if the binary is ofuscated?

This tool patches on-fly System.Net.Security.dll used by HttpClient and HttpClientHandler to always return True whenever the server certificate is legit or not (SSL Pinning)

# Requirements
- Harmony: Used to patch our System.Net.Security.dll
- .NET Framework 4.7 or above
- Visual Studio 2022 or above

# Technical details

During the investigation, it is found that the parent function which returns the result into the HttpClient is called VerifiedRemoteCertificate() which is in System.Net.Security.SecureChannel path.
![image](https://github.com/user-attachments/assets/d3c623ea-6511-4156-8f31-b12987271f30)


Using Reflection and Harmony we are allowed to "Hook" this function to always return True during the Postfix execution.

The entire write up is found here
# Project structure
- *SSLPinningBypass:* Test program to probe that it works
- *libSSLPinningBypass:* This is where the magic is, it patches VerifyRemoteCertificate() using Harmony
- *MethodFinder:* Utility to find the QNAME and the function which verifies the server certificate in System.Net.Security.dll
- *Launcher:* The second important project, it launches the binary as a reference and patches the DLL using libSSLPinningBypass.dll
  
# How to use it
1. Clone this repository in your local storage
2. Open the solution and compile it (Download Harmony using NuGet)
3. Go to Launcher binary and grab any .NET binary you wish you want to bypass SSL Pinning
4. Profit!

![image](https://github.com/user-attachments/assets/fa331b33-5644-4cb2-b98a-e3d754c41034)

