using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Principal;

namespace Windows12.UI_Components.Installer;

public static class InstallerEngine
{
    public static readonly string InstallFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Windows12");
    public static readonly string System32Folder = Environment.GetFolderPath(Environment.SpecialFolder.System);

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
        progress?.Invoke("Creating C:\\Windows\\Windows12 directories...");
        Directory.CreateDirectory(InstallFolder);
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Shell"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Themes"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "OOBE"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Icons"));
        Directory.CreateDirectory(Path.Combine(InstallFolder, "Backup"));

        progress?.Invoke("Backing up system files...");
        BackupSystemFile("shell32.dll.mun");
        BackupSystemFile("imageres.dll.mun");

        progress?.Invoke("Patching OOBE...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE", "BypassNRO", 1);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\OOBE", "DisablePrivacyExperience", 1);

        progress?.Invoke("Disabling telemetry...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", 1);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0);

        progress?.Invoke("Applying visual and shell patches...");
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton", 0);
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", 1);

        progress?.Invoke("Registering shell path...");
        SetReg(Registry.LocalMachine, @"SOFTWARE\Windows12", "InstallPath", InstallFolder, RegistryValueKind.String);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Windows12", "ShellPath", Path.Combine(InstallFolder, "Shell", "UI_Components.exe"), RegistryValueKind.String);
        SetReg(Registry.LocalMachine, @"SOFTWARE\Windows12", "Version", "1.0", RegistryValueKind.String);

        progress?.Invoke("Adding to startup...");
        SetReg(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "Windows12Shell", Path.Combine(InstallFolder, "Shell", "UI_Components.exe"), RegistryValueKind.String);

        File.WriteAllText(Path.Combine(InstallFolder, "README.txt"), "Windows 12 Project Rebirth\nInstalled to C:\\Windows\\Windows12\n");

        progress?.Invoke("Installation complete.");
        return true;
    }

    private static void BackupSystemFile(string fileName)
    {
        try
        {
            string source = Path.Combine(System32Folder, fileName);
            string dest = Path.Combine(InstallFolder, "Backup", fileName);
            if (File.Exists(source) && !File.Exists(dest))
            {
                File.Copy(source, dest, false);
            }
        }
        catch { }
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
