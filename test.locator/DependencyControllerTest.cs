using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyLocation;

namespace Test.Locator
{
    [TestClass]
    public class DependencyControllerTest : DependencyLocatorTest
    {
        private static DependencyController controller = new DependencyController();

        public override object GetLocator()
        {
            return controller;
        }
    }
}
