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
                    <Control key={control.Name+"Values"} index={control.Index} showAs={control.ShowAs} control={control.Name} />
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
 * /
 */
var Control = React.createClass({

    getInitialState: function () {
        return { values: [] };
    },

    componentDidMount: function () {
        var url = apiUrl + "/" + this.props.index;
        $.ajax({
            url: apiUrl + "/" + this.props.index,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            success: function (data) {
                console.log("Control - success");
                this.setState({ values: data });
                console.log(this.state.values);
            }.bind(this),
            error: function (xhr, status, err) {
                console.log("Control - ERROR");
                console.error(apiUrl, status, err.toString());
            }.bind(this)
        });
    },

    render: function () {

        var configControl = <div>Empty</div>;

        switch (this.props.showAs) {
            case "InputField":
                configControl = <NumberInput key={this.props.control+"Input"} values={this.state.values} control={this.props.control} /> ;
                break;
            case "DropDown":
                configControl = <DropDown key={this.props.control + "_DD"} values={this.state.values} control={this.props.control } />;
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
var DropDown = React.createClass({

    render: function () {
        // todo: create onclick event so it is possible to change value
        var selected;
        var optionStyle = {};
        var nodes = this.props.values.map(function (value, index) {
            var opts = {};
            switch ( value.State ) {
                case 1: //Blocked
                    opts["disabled"] = "disabled";
                    optionStyle = {backgroundColor: "grey"};
                    break;
                case 2: // Forcable
                    //opts["disabled"] = "disabled";
                    optionStyle = { color: "#B0B0B0" };
                    break;
                case 3: // SystemAssigned
                    selected = value.Name;
                    break;
                case 4: // UserAssigned
                    selected = value.Name;
                    break;
                default: // Assignable
                    break;
            }
            return (
                <option key={value.FullName+"_"+index} 
                        {...opts} 
                        state={value.State}
                        style={optionStyle}>
                    {value.Name}
                </option>
            );
        });
        return (
            <select key={this.props.control} value={selected}>
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
