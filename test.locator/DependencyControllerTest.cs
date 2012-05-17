using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyLocation;

namespace Test.Locator
{
    //[Ignore]
    //[TestClass]
    public class DependencyControllerTest : DependencyLocatorTest
    {
        private static DependencyController controller = new DependencyController();

        public override object MakeLocator()
        {
            return controller;
        }
    }
}
