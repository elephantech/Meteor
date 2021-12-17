using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

using Ttp.Meteor;

namespace MeteorInkJet.SdCardTestApp {
    /// <summary>
    /// Object which deals with locating the Meteor API components.
    /// 
    /// There are two situations which commonly arise:
    /// 
    ///  (1) None of the components can be found
    ///  (2) Only the managed components can be found
    /// 
    /// Situation #1 should be the normal situation in a Meteor SDK installation,
    /// unless the Meteor installation folder has been added to system paths.
    /// 
    /// Situation #2 arises if Visual Studio has copied the referenced assemblies
    /// locally (controlled by the "Copy Local" property in the assmebly reference)
    /// 
    /// To locate managed components, a handler for the AppDomain's AssemblyResolve
    /// event is installed.
    /// 
    /// To locate unmanaged components which are in a separate folder from the
    /// managed PrinterInterfaceCLS, the path to the installed Meteor 
    /// PrinterInterface library is added to the local environment path.
    ///
    /// For both of the above, the 32-bit path (API\x86) or the 64-bit path (API\amd64) 
    /// is selected depending on whether the current application is 32-bit or 64-bit.
    /// </summary>
    class MeteorPath {
        /// <summary>
        /// Retrieve the Meteor API path from the registry.
        /// 
        /// This is complicated by the Windows registry redirection and its 
        /// "Wow6432Node" registry key
        /// 
        /// For example, if we're running as a 32 bit application on a 64 bit PC,
        /// we need the 64 bit registry view to pick up the 64 bit Meteor install;
        /// by default we'd get the 32 bit view.
        /// </summary>
        private static string GetMeteorAPIPath() {
            RegistryView[] regviews = { RegistryView.Registry64, RegistryView.Registry32 };
            foreach (RegistryView v in regviews) {
                RegistryKey basekey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, v);
                if (basekey != null) {
                    RegistryKey meteorkey = basekey.OpenSubKey(@"SOFTWARE\TTP\Meteor", false);
                    if (meteorkey != null) {
                        string installdir = (string)meteorkey.GetValue("InstallPath");
                        if (installdir != null && Directory.Exists(installdir)) {
                            // Adjust the path to include the 32 or 64 bit Meteor API folder
                            string apipath = installdir + (IntPtr.Size == 4 ? @"Api\x86" : @"Api\amd64");
                            return apipath;
                        }
                    }
                }
            }
            return null;
        }

        static List<string> MeteorAssemblies = new List<string>{
            "PrinterInterfaceCLS", "MeteorCLS",  "UtilityClasses"
        };

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            AssemblyName Name = new AssemblyName(args.Name);
            string BaseName = Name.Name;
            if (MeteorAssemblies.Find(x => string.Compare(x, BaseName, true) == 0) != null) {
                string path = GetMeteorAPIPath();
                if (path != null) {
                    path += "\\" + BaseName + ".dll";
                    if (File.Exists(path)) {
                        Console.WriteLine("MeteorPath: Loading " + path);
                        Assembly a = Assembly.LoadFrom(path);
                        return a;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Attempt to locate the Meteor PrinterInterface.  If it cannot be loaded using the default 
        /// environment path, the installed Meteor API folder is added to the local environment path.
        /// Returns true if successful.
        /// </summary>
        static public bool LocatePrinterInterface() {
            // Install handler for an unresolved assembly.
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            // Now call the handler method.  If we call into PrinterInterfaceCLS directly in this
            // method, the assmebly load happens during the method call, before we get a chance to 
            // install the AssemblyResolve event
            return DoLocatePrinterInterface();
        }

        static private bool DoLocatePrinterInterface() {
            // See if we can call into the Printer Interface.  This will drill down into the
            // underlying unmanaged PrinterInterface.dll.
            try {
                PrinterInterfaceCLS.PiGetBuildNumber();
            }

            // If both the managed (PrinterInterfaceCLS.dll) and unmanaged (PrinterInterface.dll) components
            // were found, there's nothing to do.  If just the managed component is found, we must add the
            // API folder to the environment path of the current process.
            catch (DllNotFoundException) {
                string apipath = GetMeteorAPIPath();
                if (apipath != null) {
                    string envpath = Environment.GetEnvironmentVariable("PATH");
                    Console.WriteLine("MeteorPath: Adding '" + apipath + "' to local environment PATH");
                    envpath = apipath + ";" + envpath;
                    Environment.SetEnvironmentVariable("PATH", envpath);
                    
                    // Try again
                    try {
                        PrinterInterfaceCLS.PiGetBuildNumber();
                    }
                    catch (Exception e) {
                        Console.WriteLine("MeteorPath: Exception + " + e.Message);
                        return false;
                    }

                } else {
                    Console.WriteLine("MeteorPath: Install folder not found");
                    return false;
                }
            }

            // Unhandled exception
            catch (Exception e) {
                Console.WriteLine("MeteorPath: Exception + " + e.Message);
                return false;
            }

            return true;
        }
    }
}
