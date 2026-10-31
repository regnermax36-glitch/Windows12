using System.Diagnostics;
using Microsoft.Win32;

namespace Windows12.InstallerCmd;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Windows 12 Installer";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("========================================================");
        Console.WriteLine("          WINDOWS 12 PROJECT REBIRTH INSTALLER");
        Console.WriteLine("              No Bloat. No Telemetry. Full Control.");
        Console.WriteLine("========================================================");
        Console.WriteLine();
        Console.ResetColor();

        if (!IsAdministrator())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] This installer must be run as Administrator.");
            Console.ResetColor();
            Console.WriteLine("Right-click the EXE and select \"Run as administrator\".");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        string installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows12");

        Step(1, "Creating directories...");
        Directory.CreateDirectory(installDir);
        Directory.CreateDirectory(Path.Combine(installDir, "Shell"));
        Directory.CreateDirectory(Path.Combine(installDir, "OOBE"));
        Directory.CreateDirectory(Path.Combine(installDir, "Themes"));
        Console.WriteLine($"        Created: {installDir}");

        Step(2, "Patching OOBE / local account...");
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE", "BypassNRO", 1);
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\OOBE", "DisablePrivacyExperience", 1);
        Console.WriteLine("        Local account bypass enabled.");

        Step(3, "Disabling telemetry...");
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0);
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", 1);
        SetRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0);
        Console.WriteLine("        Telemetry disabled.");

        Step(4, "Applying visual patches...");
        SetRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
        SetRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
        SetRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", 0);
        SetRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0);
        Console.WriteLine("        Dark mode and left taskbar applied.");

        Step(5, "Writing install marker...");
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Windows12", "InstallPath", installDir, RegistryValueKind.String);
        SetRegistry(Registry.LocalMachine, @"SOFTWARE\Windows12", "Version", "1.0", RegistryValueKind.String);
        File.WriteAllText(Path.Combine(installDir, "README.txt"), "Windows 12 Project Rebirth\nInstalled via console installer.\nNo Microsoft account required.\nTelemetry disabled.\n");
        Console.WriteLine("        Marker written.");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("========================================================");
        Console.WriteLine("                    INSTALLATION COMPLETE");
        Console.WriteLine("========================================================");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"Install location: {installDir}");
        Console.WriteLine("Restart recommended for all changes to take effect.");
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void Step(int n, string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{n}/5] {text}");
        Console.ResetColor();
    }

    static bool IsAdministrator()
    {
        try
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    static void SetRegistry(RegistryKey root, string subKey, string name, object value, RegistryValueKind kind = RegistryValueKind.DWord)
    {
        try
        {
            using var key = root.CreateSubKey(subKey, true);
            key?.SetValue(name, value, kind);
        }
        catch
        {
            // Ignore individual failures
        }
    }
}
