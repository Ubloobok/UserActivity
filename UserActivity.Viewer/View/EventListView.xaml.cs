using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using UserActivity.Viewer.ViewModel;

namespace UserActivity.Viewer.View
{
    /// <summary>
    /// Event list view.
    /// </summary>
    public partial class EventListView : UserControl
    {
        /// <summary>Ctor.</summary>
        public EventListView()
        {
            InitializeComponent();
        }

        /// <summary>View model from data context.</summary>
        public EventListVM ViewModel => DataContext as EventListVM;
    }
}
