using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Windows.Controls;

using UserActivity.CL.WPF.Behaviors;
using UserActivity.CL.WPF.Extensions;

namespace UserActivity.CL.WPF.Tests.Behaviors
{
    [TestClass]
    public class BehaviorBaseBehavior
    {
        [TestMethod]
        public void ShouldFindSameRegion()
        {
            var childChild = new ContentControl();
            var child = new ContentControl() { Content = childChild };
            var expectedRegion = new Grid();
            expectedRegion.Children.Add(child);
            UserActivityBehavior.SetRegionName(expectedRegion, "Тестовый Регион");

            var actualRegion = UserActivityBehavior.GetRegionInVisualTree(childChild);

            Assert.IsNotNull(actualRegion, "Должен быть найден тот же регион.");
            Assert.AreEqual(expectedRegion, actualRegion, "Должен быть найден тот же регион.");
        }
    }
}
