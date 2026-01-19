using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace Windows_Update_Delayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 应用程序启动时检查是否以管理员身份运行
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!IsRunAsAdministrator())
            {
                MessageBox.Show("请右键单击程序，选择\"以管理员身份运行\"此程序。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                Shutdown();
            }
        }

        /// <summary>
        /// 检查程序是否以管理员权限运行
        /// </summary>
        /// <returns>如果是管理员权限返回 true，否则返回 false</returns>
        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
