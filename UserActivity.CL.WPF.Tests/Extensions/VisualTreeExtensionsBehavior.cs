using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Windows.Controls;

using UserActivity.CL.WPF.Extensions;

namespace UserActivity.CL.WPF.Tests.Extensions
{
    [TestClass]
    public class VisualTreeExtensionsBehavior
    {
        [TestMethod]
        public void ShouldReturnOnlySameElement()
        {
            var elem = new Grid();
            var elems = VisualTreeExtensions.GetAscendantsAndSelf(elem);
            Assert.AreEqual(1, elems.Count(), "Должен вернуться только один элемент.");
            Assert.AreEqual(elem, elems.FirstOrDefault(), "Должен вернуться только тот же элемент.");
        }
    }
}
