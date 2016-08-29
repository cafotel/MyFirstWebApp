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


  public class ConfigurationController: ApiController {
    
    private readonly ProductModel _model;

    public ConfigurationController() {
      string path = ConfigurationManager.AppSettings["vtzPath"];
      _model = ProductModel.CreateFromVT( path );
      //_model.AutoUndo = true;
    }

    [HttpGet]
    public IEnumerable<VariableWrapper> Get() {
      var variables = _model.AllVariables;

      var returnVariables = WrapVariables( variables );
      //var returnVariables = variables.Select( v => new VariableWrapper(

      //                                          v.ElementUuid,
      //                                          v.Index,
      //                                          v.Name,
      //                                          v.ShowAs,
      //                                          v.Values.Select( x => new ValueWrapper(
      //                                                              x.ElementUuid,
      //                                                              x.Name,
      //                                                              x.FullyQualifiedName,
      //                                                              v.GetState( x ).ToString()
      //                                                            ) ).ToList()
      //                                        ) ).ToList();

      return returnVariables;
    }


    // GET: api/Configuration/5b738403-030b-42bf-9c3a-98b2d3027f49
    /* returns the values of a given variable including the value states. Returns null if no variable or values found */
    [HttpGet]
    public List<ValueWrapper> GetValues( Guid id ) {

      var variable = _model?.GetVariableFromUuid( id );

      return variable?.Values?.Select( v => WrapValue( v , 
                                                       variable.GetState( v ).ToString() )
                                     ).ToList();      
    }

    // POST: api/Configuration
    public void Post( [FromBody]string value ) {

    }

    // PUT: api/Configuration/
    [HttpPut]
    public IHttpActionResult Put( List<Assignment> assignments, bool force = false ) {

      // apply assigned value to model
//      _model.ApplyAssignments(assignments);
      //_model.ApplyAssignments( assignments, AssignmentApplyMethod.ResetDontApplyDefaults );
      _model.ApplyAssignments( assignments, AssignmentApplyMethod.ResetAndApplyDefaults );
      //var undolist = _model.UndoList;

      // check the model for conflicts
      if ( !_model.HasConflict ) {
        return Ok( new { HasConflict = _model.HasConflict } );
      }

      var conflict = _model.GetConflict();
      var conflictAssign = conflict.ConflictingAssignments;
      //conflictAssign.
      return Ok( new {
        HasConflict = _model.HasConflict,
        HeaderText = conflict.HeaderText,
        Message = conflict.Message,
        ForceText = conflict.ForceText,
        UndoText = conflict.UndoText
      } );

    }

    public IHttpActionResult ForceAssignment(bool force) {
  /*    if ( !force ) {
        _model.Undo();
      }
      */
      return Ok(Get());
    }

    // DELETE: api/Configuration/5
    public void Delete( int id ) {
    }

    #region Private wrapper methods
    /* Takes a VariableCollection and wraps it in the output VariableWrapper IEnumerable */
    private IEnumerable<VariableWrapper> WrapVariables( VariableCollection variables ) {

      var returnVariables = variables.Select( v => new VariableWrapper(

                                                v.ElementUuid,
                                                v.Index,
                                                v.Name,
                                                v.ShowAs,
                                                v.Values.Select( x =>  WrapValue( x,
                                                                                  v.GetState( x ).ToString() ) ).ToList()
                                              ) ).ToList();

      return returnVariables;
    }

    /* Takes a Variable / ValueCollection and wraps it in the output ValueWrapper IEnumerable */
    private ValueWrapper WrapValue(Value value, string state) {

      return new ValueWrapper( value.ElementUuid,
                               value.Name,
                               value.FullyQualifiedName, 
                               state
                               );
    }

    private ConflictWrapper WrapConflict(Conflict conflict, bool hasConflict) {

      return new ConflictWrapper( hasConflict,
                                  conflict.HeaderText,
                                  conflict.Message,
                                  conflict.ForceText,
                                  conflict.UndoText
                                  );
    }
    #endregion
  }

  #region Wrapper object classes

  public class VariableWrapper {

    public VariableWrapper( Guid id, int index, string name, string showas, List<ValueWrapper> values ) {
      Id = id;
      Index = index;
      Name = name;
      ShowAs = showas;
      Values = values;
    }

    public Guid Id { get; set; }
    public int Index { get; set; }
    public string Name { get; set; }
    public string ShowAs { get; set; }
    public List<ValueWrapper> Values { get; set; }
  }

  public class ValueWrapper {

    public ValueWrapper( Guid id, string name, string fullName, string state ) {
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

  public class ConflictWrapper {

    public ConflictWrapper(bool hasConflict, string headerText, string message, string forceText, string undoText) {
      HasConflict = hasConflict;
      HeaderText = headerText;
      Message = message;
      ForceText = forceText;
      UndoText = undoText;
    }

    public bool HasConflict { get; set; }
    public string HeaderText { get; set; }
    public string Message { get; set; }
    public string ForceText { get; set; }
    public string UndoText { get; set; }
  }

  #endregion
}
