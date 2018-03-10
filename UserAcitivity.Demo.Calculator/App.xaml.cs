using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UserActivity.CL.WPF.Services;

namespace UserAcitivity.Demo.Calculator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UserActivityService.Initialize(new XmlUserActivityDataContext());
            UserActivityService.Current.OpenSession();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            UserActivityService.Current.CloseSession();
            base.OnExit(e);
        }
    }
}
