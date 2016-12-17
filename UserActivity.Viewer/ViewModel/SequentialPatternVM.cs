using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.Viewer.ViewModel
{
    public class SequentialPatternVM : ComponentVM
    {
        public SequentialPatternVM()
        {
        }

        public void Initialize(IEnumerable<Activity> activities)
        {
            Header = "Посл. Шаблоны";
        }
    }
}
