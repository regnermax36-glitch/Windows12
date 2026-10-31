using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Principal;

namespace Windows12.UI_Components.Installer;

public static class InstallerEngine
{
    public static readonly string InstallFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Windows12");

    public static bool IsAdministrator()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    public static bool Install(Action<string>? progress = null)
    {
        progress?.Invoke("Creating directories...");
        Directory.CreateDirectory(InstallFolder);
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Shell"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Themes"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "OOBE"));

        progress?.Invoke("Patching OOBE...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE", "BypassNRO", 1);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\OOBE", "DisablePrivacyExperience", 1);

        progress?.Invoke("Disabling telemetry...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", 1);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0);

        progress?.Invoke("Applying visual patches...");
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0);

        progress?.Invoke("Writing install marker...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Windows12", "InstallPath", InstallFolder, RegistryValueKind.String);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Windows12", "Version", "1.0", RegistryValueKind.String);
        File.WriteAllText(Path.Combine(InstallFolder, "README.txt"), "Windows 12 Project Rebirth\nInstalled via installer.\n");

        progress?.Invoke("Installation complete.");
        return true;
    }

    private static void SetReg(RegistryKey root, string subKey, string name, object value, RegistryValueKind kind = RegistryValueKind.DWord)
    {
        try
        {
            using var key = root.CreateSubKey(subKey, true);
            key?.SetValue(name, value, kind);
        }
        catch { }
    }
}
