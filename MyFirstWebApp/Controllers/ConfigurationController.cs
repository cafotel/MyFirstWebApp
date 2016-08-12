using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Configit.Base.Collections;
using Configit.Core.Capabilities.Converters;
using Configit.Runtime;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyFirstWebApp.Controllers {

  public class ConfigVariables {

    public ConfigVariables( int index, string name, string showas, List<ConfigValues> values  ) {
      Index = index;
      Name = name;
      ShowAs = showas;
      ConfigValues = values;
    }
    public int Index { get; set; }
    public string Name { get; set; }
    public string ShowAs { get; set; }
    public List<ConfigValues> ConfigValues { get; set; }
  }

  public class ConfigValues {
    public ConfigValues(string name, string fullName, string state) {
      Name = name;
      FullName = fullName;
      State = state;
    }

    public string Name { get; set; }
    public string FullName { get; set; }
    public string State { get; set; }
  }

  public class ConfigurationController: ApiController {
    
    private readonly ProductModel _model;

    public ConfigurationController() {
      string path = ConfigurationManager.AppSettings["vtzPath"];
      _model = ProductModel.CreateFromVT( path );
    }

    public IEnumerable<ConfigVariables> Get() {
      var variables = _model.AllVariables;

      var returnVariables = variables.Select( v => new ConfigVariables(
                                                v.Index,
                                                v.Name,
                                                v.ShowAs,
                                                v.Values.Select( x => new ConfigValues(
                                                                    x.Name,
                                                                    x.FullyQualifiedName,
                                                                    v.GetState( x ).ToString()
                                                                  ) ).ToList()
                                              ) ).ToList();

      return returnVariables;
    }

    // GET: api/Configuration/5
    /* returns the values of a given variable including the value states */
    public IHttpActionResult Get( int id ) {

      // select the values of the variable given in the input parameter
      var variable = _model.AllVariables[id];
      var values = variable.Values;
      var vars = values.Select( v => new { v.Name,
                                           FullName = v.FullyQualifiedName,
                                           State = variable.GetState(v) } ).ToList();

      return Ok( vars );


   }

    // POST: api/Configuration
    public void Post( [FromBody]string value ) {
    }

    // PUT: api/Configuration/5
    public void Put( int id, [FromBody]string value ) {
    }

    // DELETE: api/Configuration/5
    public void Delete( int id ) {
    }
  }
}
