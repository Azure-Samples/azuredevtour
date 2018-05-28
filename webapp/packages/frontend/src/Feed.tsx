import axios from "axios";
import React from "react";
import styled from "react-emotion";
import Post from "./Post";

const PageContainer = styled("div")`
  display: flex;
  width: 95%;
  max-width: 600px;
  margin: 15px auto;
  justify-content: space-between;
  align-items: center;
`;

const PageBtn = styled("button")`
  background-color: white;
  border: none;
  cursor: pointer;
  font-size: 16px;

  &:hover {
    text-decoration: underline;
  }
`;

class Feed extends React.Component<any> {
  public state = {
    posts: [],
    page: 0,
    hasNext: true
  };

  public componentWillMount() {
    this.request();
  }

  public nextPage = () => {
    this.setState(
      {
        page: this.state.page + 1
      },
      () => this.request()
    );
  };

  public previousPage = () => {
    this.setState(
      {
        page: this.state.page - 1
      },
      () => this.request()
    );
  };

  public componentDidUpdate(prevProps) {
    if (prevProps.match.params.username !== this.props.match.params.username) {
      this.request();
    }
    window.scrollTo(0, 0);
  }

  public request() {
    const params: any = { page: this.state.page };
    let url = "/api/feed";
    if (this.props.match.params.username) {
      url = `${url}/${this.props.match.params.username}`;
    }
    axios
      .get(url, { params })
      .then(response => {
        const newPosts = response.data.pics;
        if (newPosts) {
          const newState = Object.assign({}, this.state, {
            posts: newPosts,
            hasNext:
              response.data.pageSize * (response.data.page + 1) <
              response.data.totalResults
          });
          this.setState(newState);
        }
      })
      .catch(error => console.log);
  }

  public render() {
    return (
      <div>
        <div>
          <div>
            {this.state.posts.map((p: any) => <Post key={p.photoUrl} {...p} />)}
          </div>
        </div>
        <PageContainer>
          {this.state.page > 0 ? (
            <PageBtn onClick={this.previousPage}>⬅ Previous</PageBtn>
          ) : (
            <div />
          )}
          <div>Page {this.state.page + 1}</div>
          {this.state.hasNext ? (
            <PageBtn onClick={this.nextPage}>Next ➡</PageBtn>
          ) : (
            <div />
          )}
        </PageContainer>
      </div>
    );
  }
}

export default Feed;
