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

  public class ConfigVars {

    public ConfigVars( int index, string name, string showas ) {
      Index = index;
      Name = name;
      ShowAs = showas;
    }
    public int Index { get; set; }
    public string Name { get; set; }
    public string ShowAs { get; set; }
  }

  public class ConfigurationController: ApiController {

    private ProductModel model;

    public ConfigurationController() {
      string path = ConfigurationManager.AppSettings["vtzPath"];
      model = ProductModel.CreateFromVT( path );
    }

    public IEnumerable<ConfigVars> Get() {
      var variables = model.AllVariables;

      // TODO: rewrite as anonymous object
      var vars = variables.Select( i => new ConfigVars( i.Index, i.Name, i.ShowAs ) );
      // TODO: add values and valueStates 

      return vars;
    }

    // GET: api/Configuration/5
    //    public IEnumerable<string> GetVariableValues( int i ) {
    public IHttpActionResult Get( int id ) {

      // select the values of the variable given in the input parameter
      var variable = model.AllVariables[id];
      var values = variable.Values;
      var vars = values.Select( v => new { v.Name, FullName = v.FullyQualifiedName } ).ToList();

      // get ValueStates
      // TODO: create linq instead
      List<ValueState> valueStates = new List<ValueState>();
      foreach ( Value value in values ) {
        var valState = variable.GetState( value );
        valueStates.Add(valState);
      }

      var finalValues = new object();
      if ( vars.Count == valueStates.Count ) {
        finalValues = vars.Zip( valueStates, ( x, y ) => new { x.FullName, x.Name, State = y } );
      }

      return Ok( finalValues );
     // return Ok( values.Select( v => v.Name ) );

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
