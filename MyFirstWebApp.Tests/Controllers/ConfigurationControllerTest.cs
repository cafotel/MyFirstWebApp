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

    // TODO: would a better method for testing for existing id be to get some random variable first, 
    // TODO: then try to get the variables by that variable's Id?
    [TestMethod()]
    public void GetValuesTest_IdExist() {
      var controller = new ConfigurationController();
      var guid = new Guid( "5b738403-030b-42bf-9c3a-98b2d3027f49" ); // existing ID

      var result = controller.GetValues( guid );

      Assert.IsNotNull( result );
      Assert.IsInstanceOfType( result, typeof( List<ValueWrapper> ) );
    }

    [TestMethod()]
    public void GetValuesTest_IdNotExist() {
      var controller = new ConfigurationController();
      var guid = new Guid( "5b738403-030b-42bf-9c3a-98b2d3027f40" ); //non-existent id

      var result = controller.GetValues( guid );

      Assert.IsNull( result );
    }
  }
}
