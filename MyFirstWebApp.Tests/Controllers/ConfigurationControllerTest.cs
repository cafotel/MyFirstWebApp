using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFirstWebApp;
using MyFirstWebApp.Controllers;

namespace MyFirstWebApp.Tests.Controllers {

  [TestClass]
  public class ConfigurationControllerTest {

    [TestMethod]
    public void GetTest() {
      var controller = new ConfigurationController();

      var result = controller.Get();

      Assert.IsNotNull( result );
      Assert.IsInstanceOfType( result, typeof( IEnumerable<VariableWrapper> ) );
    }
  }
}
