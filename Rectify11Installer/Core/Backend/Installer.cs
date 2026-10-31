using Microsoft.Win32;
using Rectify11Installer.Win32;
using System;
using System.IO;
using static Rectify11Installer.Win32.NativeMethods;

namespace Rectify11Installer.Core
{
    public class Installer
    {
        public bool Install(FrmWizard frm)
        {
            frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.preparingInstall;
            Logger.WriteLine("Preparing Installation");
            Logger.WriteLine("──────────────────────");

            Directory.CreateDirectory(Variables.r11Folder);

            Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true)
                ?.CreateSubKey("Rectify11", true)
                ?.DeleteValue("x86PendingFiles", false);

            if (!Common.WriteFiles(false, false))
                return false;

            if (!Common.CreateDirs())
                return false;

            if (Variables.CreateRestorePoint)
            {
                try
                {
                    frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.creatingRestorePoint;
                    CreateSystemRestorePoint();
                }
                catch (Exception ex)
                {
                    Logger.Warn("Error creating a restore point", ex);
                }
            }

            frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.installingRt;
            Common.InstallRuntimes();
            if (Variables.vcRedist && Variables.core31)
            {
                Logger.WriteLine("InstallRuntimes() succeeded.");
            }
            else if (!Variables.vcRedist)
            {
                Logger.Warn("vcredist.exe installation failed.");
                Common.RuntimeInstallError("Visual C++ Runtime", "Visual C++ Runtime is used for MicaForEveryone and AccentColorizer.", "https://aka.ms/vs/17/release/vc_redist.x64.exe");
            }
            else if (!Variables.core31)
            {
                Logger.Warn("core31.exe installation failed.");
                Common.RuntimeInstallError(".NET Core 3.1", ".NET Core 3.1 is used for MicaForEveryone.", "https://dotnet.microsoft.com/en-us/download/dotnet/3.1");
            }
            Logger.WriteLine("══════════════════════════════════════════════");

            if (!ThemeUtil.IsUsingDarkMode) DarkMode.UpdateFrame(frm, false);

            frm.InstallerProgress = "Extracting files...";
            Helper.SafeDirectoryDeletion(Path.Combine(Variables.r11Folder, "files"), false);
            if (!Helper.SafeFileOperation(
                Path.Combine(Variables.r11Folder, "files.7z"),
                Properties.Resources.files,
                Helper.OperationType.Write))
                return false;

            Helper.SvExtract("files.7z", "files");

            if (InstallOptions.InstallThemes || InstallOptions.InstallExtras())
            {
                try
                {
                    Themes.InstallR11Cpl();
                    Logger.WriteLine("Installr11cpl() succeeded.");
                }
                catch (Exception ex)
                {
                    Logger.Warn("Installr11cpl() failed", ex);
                    return false;
                }
            }

            if (InstallOptions.InstallThemes)
            {
                frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.installingThemes;
                if (!Themes.Install()) return false;
            }

            if (InstallOptions.InstallExtras())
            {
                frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.installingExtras;
                if (!Extras.Install(frm)) return false;
            }

            if (InstallOptions.iconsList.Count > 0)
            {
                if (!Icons.Install(frm)) return false;
            }

            if (InstallOptions.iconsList.Count > 0 || InstallOptions.InstallThemes)
            {
                try
                {
                    var src = Path.Combine(Variables.r11Files, "duires.dll");
                    var dst = Path.Combine(Variables.sys32Folder, "duires.dll");
                    if (File.Exists(src))
                    {
                        if (!Helper.SafeFileDeletion(dst))
                        {
                            var pending = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            File.Copy(src, pending, true);
                            MoveFileEx(pending, dst, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT | MoveFileFlags.MOVEFILE_REPLACE_EXISTING);
                        }
                        else
                        {
                            File.Copy(src, dst, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("duires.dll copy failed", ex);
                }
            }

            InstallChangelogApp();

            frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.creatingUninstaller;
            Common.CreateUninstall();

            InstallStatus.IsRectify11Installed = true;
            Logger.WriteLine("══════════════════════════════════════════════");

            frm.InstallerProgress = Rectify11Installer.Strings.Rectify11.cleaningUp;
            Common.Cleanup();
            Logger.CommitLog();
            return true;
        }

        private void InstallChangelogApp()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key?.SetValue("RectifyStart", Path.Combine(Variables.r11Folder, "RectifyStart.exe"), RegistryValueKind.String);
            key.Close();
        }
    }
}
