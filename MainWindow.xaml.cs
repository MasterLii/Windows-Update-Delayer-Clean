using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Windows_Update_Delayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// Windows Update Delayer 主窗口类，提供暂停和管理 Windows 更新的功能
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数：初始化主窗口并设置窗口位置
        /// 将窗口水平居中，垂直方向偏上显示
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // 将窗口水平居中，垂直偏上一点
            WindowStartupLocation = WindowStartupLocation.Manual;
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;
            Left = (screenWidth - Width) / 2;
            Top = screenHeight / 4.5;
        }

        /// <summary>
        /// 超链接点击事件处理：使用默认浏览器打开链接
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">导航事件参数</param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                // 使用默认浏览器打开链接
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 按钮点击反馈处理：执行操作并显示成功提示
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="button">触发事件的按钮</param>
        private async void HandleError(Action action, Button button)
        {
            try
            {
                var successMessage = "✅ 成功！";
                var originalText = button.Content.ToString();  // 保存原始的按钮标签文字
                if (originalText == successMessage)
                    return;  // 如果按钮已经显示成功消息，则不再执行后续操作

                action();
                button.Content = successMessage;  // 设置按钮标签为“操作成功！”
                await Task.Delay(1500);  // 提示的显示时长
                button.Content = originalText;  // 恢复按钮的原始标签文字
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 设置 Windows 更新暂停时间
        /// 通过修改注册表来暂停系统更新到指定日期
        /// </summary>
        /// <param name="endDate">暂停结束时间（ISO 8601 格式）</param>
        private void UpdatePauseEndTime(string endDate)
        {
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var keyPath = @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
                    using (var key = baseKey.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("FlightSettingsMaxPauseDays", 0x2AE4, RegistryValueKind.DWord);
                            key.SetValue("PauseFeatureUpdatesStartTime", "2024-01-01T10:00:00Z", RegistryValueKind.String);
                            key.SetValue("PauseFeatureUpdatesEndTime", endDate, RegistryValueKind.String);
                            key.SetValue("PauseQualityUpdatesStartTime", "2024-01-01T10:00:00Z", RegistryValueKind.String);
                            key.SetValue("PauseQualityUpdatesEndTime", endDate, RegistryValueKind.String);
                            key.SetValue("PauseUpdatesStartTime", "2024-01-01T10:00:00Z", RegistryValueKind.String);
                            key.SetValue("PauseUpdatesExpiryTime", endDate, RegistryValueKind.String);
                        }
                    }
                }
        }

        /// <summary>
        /// 按钮事件：暂停 Windows 更新到 2027 年
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                UpdatePauseEndTime("2027-01-01T10:00:00Z");
            }, (Button)sender);
        }
        /// <summary>
        /// 按钮事件：暂停 Windows 更新到 2035 年
        /// </summary>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                UpdatePauseEndTime("2035-01-01T10:00:00Z");
            }, (Button)sender);
        }
        /// <summary>
        /// 按钮事件：暂停 Windows 更新到 2054 年
        /// </summary>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                UpdatePauseEndTime("2054-01-01T10:00:00Z");
            }, (Button)sender);
        }

        /// <summary>
        /// 按钮事件：恢复 Windows 正常更新
        /// 删除所有暂停更新的注册表项，恢复系统默认设置
        /// </summary>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var keyPath = @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
                    using (var key = baseKey.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("FlightSettingsMaxPauseDays", 0x23, RegistryValueKind.DWord);
                            key.DeleteValue("PauseFeatureUpdatesStartTime", false);
                            key.DeleteValue("PauseFeatureUpdatesEndTime", false);
                            key.DeleteValue("PauseQualityUpdatesStartTime", false);
                            key.DeleteValue("PauseQualityUpdatesEndTime", false);
                            key.DeleteValue("PauseUpdatesStartTime", false);
                            key.DeleteValue("PauseUpdatesExpiryTime", false);
                        }
                    }
                }

                // 删除整个更新策略
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var policyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";
                    baseKey.DeleteSubKeyTree(policyPath, throwOnMissingSubKey: false); 
                }

                // 删除 HwReqChk 路径
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var hwReqChkPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\HwReqChk";
                    baseKey.DeleteSubKeyTree(hwReqChkPath, throwOnMissingSubKey: false);
                }

                // 删除 MoSetup 路径
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var moSetupPath = @"SYSTEM\Setup\MoSetup";
                    baseKey.DeleteSubKeyTree(moSetupPath, throwOnMissingSubKey: false);
                }

            }, (Button)sender);
        }

        /// <summary>
        /// 标记是否正在处理打开更新页面的操作，防止重复点击
        /// </summary>
        private bool isProcessing = false;
        /// <summary>
        /// 按钮事件：打开系统更新页面
        /// 关闭现有设置窗口并重新打开，确保显示最新状态
        /// </summary>
        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (isProcessing)
                return;

            isProcessing = true;

            try
            {
                await Task.Run(() =>
                {
                    // 尝试关闭所有设置窗口
                    foreach (var proc in Process.GetProcessesByName("SystemSettings"))
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit();
                        }
                        catch
                        {
                            // 忽略杀不掉的情况
                        }
                    }
                });

                await Task.Delay(500); // 稍等确保进程关闭

                // 启动更新页面
                Process.Start("ms-settings:windowsupdate");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("打开更新页面失败:\\n{0}", ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 加一点冷却时间，防止连续点击出问题
            await Task.Delay(2000);
            isProcessing = false;
        }

        /// <summary>
        /// 按钮事件：禁止 Windows 大版本更新
        /// 通过设置注册表策略阻止功能更新（例如从 23H2 升级到 24H2）
        /// </summary>
        private void Button_BlockFeatureUpdate(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    var keyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";
                    using (var key = baseKey.CreateSubKey(keyPath)) 
                    {
                        if (key != null)
                        {
                            key.SetValue("TargetReleaseVersion", 1, RegistryValueKind.DWord);
                            key.SetValue("TargetReleaseVersionInfo", "WinUpdateDelayer", RegistryValueKind.String);
                            key.DeleteValue("ProductVersion", throwOnMissingValue: false);
                        }
                    }
                }

            }, (Button)sender);
        }

        /// <summary>
        /// 指定 Windows 11 系统最高更新版本
        /// </summary>
        /// <param name="targetVersion">目标版本号（例如 "22H2", "23H2", "24H2"）</param>
        private void SetTargetReleaseVersion(string targetVersion)
        {
            var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            {
                var keyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";
                using (var key = baseKey.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue("ProductVersion", "Windows 11", RegistryValueKind.String);
                        key.SetValue("TargetReleaseVersionInfo", targetVersion, RegistryValueKind.String);
                        key.SetValue("TargetReleaseVersion", 1, RegistryValueKind.DWord); // 启用版本锁定
                    }
                }
            }
        }
        
        /// <summary>
        /// 解除 Windows 11 升级限制
        /// 修改注册表绕过硬件兼容性检查（TPM、CPU 等）
        /// </summary>
        private void DisableUpgradeCompatibility()
        {
            var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            {
                string[] keysToDelete =
                {
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\CompatMarkers",
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Shared",
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\TargetVersionUpgradeExperienceIndicators"
                };

                foreach (var keyPath in keysToDelete)
                {
                    try
                    {
                        baseKey.DeleteSubKeyTree(keyPath, false);
                    }
                    catch
                    {
                        // 忽略删除失败异常
                    }
                }

                string hwReqChkPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\HwReqChk";
                using (var hwKey = baseKey.CreateSubKey(hwReqChkPath))
                {
                    if (hwKey != null)
                    {
                        string[] multiSzValues = new[]
                        {
                    "SQ_SecureBootCapable=TRUE",
                    "SQ_SecureBootEnabled=TRUE",
                    "SQ_TpmVersion=2",
                    "SQ_RamMB=8192",
                    "SQ_SSE2ProcessorSupport=TRUE",
                    "SQ_SSE4_2ProcessorSupport=TRUE",
                    "SQ_NXProcessorSupport=TRUE",
                    "SQ_CompareExchange128=TRUE",
                    "SQ_LahfSahfSupport=TRUE",
                    "SQ_PrefetchWSupport=TRUE",
                    "SQ_PopCntInstructionSupport=TRUE",
                    "SQ_SystemDiskSizeMB=99999",
                    "SQ_CpuCoreCount=9",
                    "SQ_CpuModel=99",
                    "SQ_CpuFamily=99",
                    "SQ_CpuMhz=9999",
                    "" 
                };

                        hwKey.SetValue("HwReqChkVars", multiSzValues, RegistryValueKind.MultiString);
                    }
                }

                string moSetupPath = @"SYSTEM\Setup\MoSetup";
                using (var moKey = baseKey.CreateSubKey(moSetupPath))
                {
                    if (moKey != null)
                    {
                        moKey.SetValue("AllowUpgradesWithUnsupportedTPMOrCPU", 1, RegistryValueKind.DWord);
                    }
                }
            }
        }


        /// <summary>
        /// 按钮事件：锁定 Win11 版本到 22H2
        /// </summary>
        private void Button_StayOn22H2(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                SetTargetReleaseVersion("22H2");
                DisableUpgradeCompatibility();
            }, (Button)sender);
        }

        /// <summary>
        /// 按钮事件：锁定 Win11 版本到 23H2
        /// </summary>
        private void Button_StayOn23H2(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                SetTargetReleaseVersion("23H2");
                DisableUpgradeCompatibility();
            }, (Button)sender);
        }

        /// <summary>
        /// 按钮事件：锁定 Win11 版本到 24H2
        /// </summary>
        private void Button_StayOn24H2(object sender, RoutedEventArgs e)
        {
            HandleError(() =>
            {
                SetTargetReleaseVersion("24H2");
                DisableUpgradeCompatibility();

            }, (Button)sender);
        }


    }
}
