var data = [
  { Author: "Daniel Lo Nigro", Text: "Hello ReactJS.NET World!*" },
  { Author: "Pete Hunt", Text: "This is one comment*" },
  { Author: "Jordan Walke", Text: "This is *another* comment*" }
];

var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    $.ajax({
      url: this.props.url,
      dataType: 'json',
      cache: false,
      success: function(data) {
        this.setState({data: data});
      }.bind(this),
      error: function(xhr, status, err) {
        console.error(this.props.url, status, err.toString());
      }.bind(this)
    });
  },
  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    this.loadCommentsFromServer();
    setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm />
      </div>
    );
  }
});

//var CommentBox = React.createClass({
//    render: function() {
//        return (
//          <div className="commentBox">
//        <h1>Comments</h1>
//        <CommentList data={this.props.data} />
//        <CommentForm />
//          </div>
//      );
//    }
//});

var CommentList = React.createClass({
    render: function () {
        var commentNodes = this.props.data.map(function (mycomment) {
            return (
                <Comment author={mycomment.Author}>
                    {mycomment.Text}
                </Comment>
            );
        });
        return (
          <div className="commentList">
            {commentNodes}
          </div>
        );
    }
});

var CommentForm = React.createClass({
    render: function() {
        return (
          <div className="commentForm">
            Hello, world! I am a CommentForm.
          </div>
      );
    }
});

var Comment = React.createClass({
    render: function() {
        var converter = new Showdown.converter();
        var rawMarkup = converter.makeHtml(this.props.children.toString());
        return (
          <div className="comment">
            <h2 className="commentAuthor">
              {this.props.author}
            </h2>
              <span dangerouslySetInnerHTML={{__html: rawMarkup}} />
        </div>
      );
    }
});

ReactDOM.render(
  <CommentBox url="/api/comments" pollInterval={2000} />,
  document.getElementById('content')
);

//ReactDOM.render(
//  <CommentBox data={data} />,
//  document.getElementById('content')
//);