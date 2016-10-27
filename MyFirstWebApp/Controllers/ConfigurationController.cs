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
      _model.UndoList.AutoUndo = true;
    }

    /// <summary>
    /// Retrieves all variables for the model
    /// </summary>
    /// <returns>IEnumerable VariableWrapper with all variables</returns>
    [HttpGet]
    public IEnumerable<VariableWrapper> Get() {

      var variables = _model.AllVariables;
      return WrapVariables( variables );
    }


    // GET: api/Configuration/5b738403-030b-42bf-9c3a-98b2d3027f49
    /// <summary>
    /// Gets the values for a given variable by the Id. Includes the current state of the value
    /// </summary>
    /// <param name="id">id of the variable to get values for </param>
    /// <returns></returns>
    [HttpGet]
    public List<ValueWrapper> GetValues( Guid id ) {

      var variable = _model?.GetVariableFromUuid( id );

      return variable?.Values?.Select( v => WrapValue( v, 
                                                       variable.GetState( v ).ToString() 
                                                     )
                                     ).ToList();      
    }

    // POST: api/Configuration
    public void Post( [FromBody]string value ) {

    }

    // PUT: api/Configuration/
    /// <summary>
    /// Updates the model with the given assigments
    /// </summary>
    /// <param name="assignments">A list of variable values to assign to the model</param>
    /// <param name="force">Whether to force the assignments if there is a conflict. Default is false.</param>
    /// <returns></returns>
    [HttpPut]
    public IHttpActionResult Put( [FromBody] List<Assignment> assignments, bool force = false ) {

      // apply assigned value to model
      _model.ApplyAssignments(assignments);
      //_model.ApplyAssignments( assignments, AssignmentApplyMethod.ResetDontApplyDefaults );
      //_model.ApplyAssignments( assignments, AssignmentApplyMethod.ResetAndApplyDefaults );
      //var undolist = _model.UndoList;

      var modelAssignments = _model.GetAssignments();

      // check the model for conflicts
      if ( !_model.HasConflict ) {
        return Ok( new ConflictWrapper( _model.HasConflict )  );
      }

      var conflict = _model.GetConflict();
      // remove assignments of conflicts - temp for testing what happens
      //VariableCollection conflictAssign = conflict.ConflictingAssignments;
      //foreach ( Variable variable in conflictAssign ) {
      //  var orgVariable =_model.GetVariableFromUuid(variable.ElementUuid);
      //  orgVariable.RemoveAssignment();
      //}

      //if ( !force ) {
      //  _model.UndoList.Undo();
      //}

      return Ok( WrapConflict( conflict ) );
    }

    // DELETE: api/Configuration/5
    public void Delete( int id ) {
    }

    #region Private wrapper methods

    /// <summary>
    /// Transforms variables from VariableCollection to VariableWrapper IEnumerable
    /// </summary>
    /// <param name="variables">VariableCollection to transform</param>
    /// <returns>The input VariableCollection as a VariableWrapper IEnumerable</returns>
    private IEnumerable<VariableWrapper> WrapVariables( VariableCollection variables ) {

      return variables.Select( v => new VariableWrapper(

                                                v.ElementUuid,
                                                v.Index,
                                                v.Name,
                                                v.ShowAs,
                                                v.Values.Select( x =>  WrapValue( x,
                                                                                  v.GetState( x ).ToString() ) ).ToList()
                                              ) ).ToList();

    }

    /// <summary>
    /// Transforms values from a Value object to a ValueWrapper object
    /// </summary>
    /// <param name="value"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    private ValueWrapper WrapValue(Value value, string state) {

      return new ValueWrapper( value.ElementUuid,
                               value.Name,
                               value.FullyQualifiedName, 
                               state
                               );
    }

    /// <summary>
    /// Transforms a Conflict object to a ConflictWrapper object.
    /// If there is no conflict, all but HasConflict is set as an empty string.
    /// </summary>
    /// <param name="conflict">The conflict to transform. Should never be null.</param>
    /// <returns></returns>
    private ConflictWrapper WrapConflict(Conflict conflict) {

      // TODO: better handling of a possible null object
      if ( conflict == null ) {
        throw new ArgumentNullException( nameof( conflict ) );
      }

      return new ConflictWrapper( true,
                                  conflict.HeaderText,
                                  conflict.Message,
                                  conflict.ForceText,
                                  conflict.UndoText
                                  );
    }
    #endregion
  }

  #region Wrapper objects

  /// <summary>
  /// Wrapper class for Variables
  /// </summary>
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

  /// <summary>
  /// Wrapper class for Values
  /// </summary>
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

  /// <summary>
  /// Wrapper class for conflict object. Has two constructors: one with just hasConflict, one which sets all properties.
  /// </summary>
  public class ConflictWrapper {

    public ConflictWrapper( bool hasConflict ) {
      HasConflict = hasConflict;
      HeaderText = "";
      Message = "";
      ForceText = "";
      UndoText = "";
    }

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
