using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
//using System.Web.Mvc;
using System.Web.Script.Serialization;
using Configit.Base;
using Configit.Base.Collections;
using Configit.Core.Capabilities.Converters;
using Configit.Runtime;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyFirstWebApp.Controllers {

  public class ConfigVariable {

    public ConfigVariable( Guid id, int index,  string name, string showas, List<ConfigValue> values  ) {
      Id = id;
      Index = index;
      Name = name;
      ShowAs = showas;
      ConfigValues = values;
    }
    public Guid Id { get; set; }
    public int Index { get; set; }
    public string Name { get; set; }
    public string ShowAs { get; set; }
    public List<ConfigValue> ConfigValues { get; set; }
  }

  public class ConfigValue {
    public ConfigValue(Guid id, string name, string fullName, string state) {
      Id = id;
      Name = name;
      FullName = fullName;
      State = state;
    }

    public Guid Id { get; set; }
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

    // TODO: maybe change this into an IHTTPActionResult, so as to avoid the ConfigVariable and ConfigValue classes
    [HttpGet]
    public IEnumerable<ConfigVariable> Get() {
      var variables = _model.AllVariables;

      var returnVariables = variables.Select( v => new ConfigVariable(

                                                v.ElementUuid,
                                                v.Index,
                                                v.Name,
                                                v.ShowAs,
                                                v.Values.Select( x => new ConfigValue(
                                                                    x.ElementUuid,
                                                                    x.Name,
                                                                    x.FullyQualifiedName,
                                                                    v.GetState( x ).ToString()
                                                                  ) ).ToList()
                                              ) ).ToList();

      return returnVariables;
    }

    // GET: api/Configuration/5
    /* returns the values of a given variable including the value states */
    [HttpGet]
    public IHttpActionResult GetValues( int id ) {

      // avoid out of bounds exception
      if( id >= _model.AllVariables.Count ) {
        return NotFound();
      }

      var variable = _model.AllVariables[id];
      var values = variable.Values;

      var valueResult = values.Select( v => new {
                                      Uuid = v.ElementUuid,
                                      v.Name,
                                      v.FullyQualifiedName,
                                      State = variable.GetState( v )
      } ).ToList();

      return Ok( valueResult );
    }

    // POST: api/Configuration
    public void Post( [FromBody]string value ) {

    }

    // PUT: api/Configuration/
    [HttpPut]
    public IHttpActionResult Put( List<Assignment> assignments, bool force = false ) {

      // apply assigned value to model
      _model.ApplyAssignments(assignments);

      var result = new object();
      // check the model for conflicts
      if ( !_model.HasConflict ) {
        return Ok( new { HasConflict = _model.HasConflict } );
      }

      var conflict = _model.GetConflict();
      return Ok( new {
        HasConflict = _model.HasConflict,
        HeaderText = conflict.HeaderText,
        Message = conflict.Message,
        ForceText = conflict.ForceText,
        UndoText = conflict.UndoText
      } );

    }

    public IHttpActionResult ForceAssignment() {
      return Ok(Get());
    }

    // DELETE: api/Configuration/5
    public void Delete( int id ) {
    }
  }
}
