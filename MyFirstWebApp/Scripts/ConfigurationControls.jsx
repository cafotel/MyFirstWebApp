var data;
var values;
var apiUrl = "/api/Configuration";

var ControlBox = React.createClass({

    getInitialState: function () {
        return { data: [] };
    },

    componentDidMount: function () {
        $.ajax({
            url: apiUrl,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            success: function (data) {
                this.setState({ data: data });
            }.bind(this),
            error: function (xhr, status, err) {
                console.error(apiUrl, status, err.toString());
            }.bind(this)
        });
    },

    render: function () {
        return (
          <div className="controlBox">
             <h2>Configuration controls</h2>
             <ControlList data={this.state.data} />
          </div>
      );
    } // END render
});

/**
 * Creates a list of controls from the given property data
 */
var ControlList = React.createClass({

    render: function () {
        var nodes = this.props.data.map(function (control) {
                return (
                <div key={control.Name+"Div"}>
                    <span key={control.Name}>
                        {control.Name}:
                    </span>
                    <Control key={control.Name+"Values"} 
                             data={control.ConfigValues} 
                             index={control.Index} 
                             showAs={control.ShowAs} 
                             control={control.Name} />
                </div>
            );
        });
        return (
            <div className="controlList">
                {nodes}
            </div>
        );
    } // END render
});

/**
 * Control element 
 * Retrieves the value(s) of the control and chooses the right html input element based on showAs property
 * Properties:
 * - data : values for the control
 * - index : index of the control in the model
 * - showAs : type of element to show the control as
 * - control : name of control
 * /
 */
var Control = React.createClass({

    render: function () {

        var configControl = <div>Empty</div>;

        switch (this.props.showAs) {
            case "InputField":
                configControl = <NumberInput key={this.props.control+"Input"} values={this.props.data} control={this.props.control} /> ;
                break;
            case "DropDown":
                configControl = <DropDown key={this.props.control + "_DD"} values={this.props.data} control={this.props.control } />;
                break;
            default:
                // TODO: Define default configuration control
                break;
        }

        return (
            <span>{configControl}</span>
        );
    } // END render
});

/**
 * Creates a Dropdown with the values given as property values
 */
var DropDown = React.createClass( {
    getInitialState: function() {
    return {
        select: "None"
        }
    },
    handleSelect: function (event) {
        console.log("handle select");
        this.setState({ select: event.target.value });
        //TODO: send postback to server
    },
    render: function () {
        var selected = this.state.select;
        var optionStyle = {};
        var nodes = this.props.values.map(function (value, index) {
            var opts = {};
            switch ( value.State ) {
                case "Blocked": //Blocked
                    opts["disabled"] = "disabled";
                    optionStyle = {backgroundColor: "B1B1B1"};
                    break;
                case "Forcable": // Forcable
                    //opts["disabled"] = "disabled";
                    optionStyle = { color: "#B0B0B0" };
                    break;
                case "SystemAssigned": // SystemAssigned
                    selected = value.FullName;
                    break;
                case "UserAssigned": // UserAssigned
                    selected = value.FullName;
                    break;
                default: // Assignable
                    break;
            }
            return (
                <option value={value.FullName} 
                        key={value.FullName+"_"+index} 
                        state={value.State}
                        style={optionStyle}
                        {...opts} 
                        >
                    {value.Name}
                </option>
            );
        });
        return (
            <select key={this.props.control} defaultValue={selected}  onChange={this.handleSelect} >
                <option value="None" key="None" state="None" disabled>Make your choice...</option>
                {nodes}
             </select>
        );
     } // END render
});

/**
 * Creates a number input element
 */
var NumberInput = React.createClass({

    render : function() {
        return (
            <input type="number" key={this.props.control+"_Input"}/>
            );
    } // END render
});

ReactDOM.render(
  <ControlBox />,
  document.getElementById('controls')
);
