import React from "react";
import styled from "react-emotion";
import { distanceInWordsToNow } from "date-fns";
import { Link } from "react-router-dom";
import axios from "axios";
import Spinner from "react-spinkit";
import Comment from "./Comment";
import isLoggedIn from "./isLoggedIn";

const Container = styled("div")`
  border: 1px solid #666;
  max-width: 600px;
  width: 100%;
  border-radius: 5px;
  margin: 0 auto 15px auto;
`;

const PosterImage = styled("img")`
  border-radius: 50%;
  width: 50px;
  height: 50px;
  background-color: #999;
  color: #999;
  border: 1px solid #999;
`;

const PosterName = styled("div")`
  font-size: 30px;
  font-weight: lighter;
  margin-left: 15px;

  a {
    color: #333;
    text-decoration: none;
  }
`;

const PosterContainer = styled("div")`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
`;

const MainImage: any = styled("img")`
  width: 100%;
  transition: transform 0.5s ease-in-out;
  transform: rotateZ(
    ${(props: any) => props.rotationMultiplier * props.rotationBase}deg
  );
`;

const VotesContainer = styled("div")`
  margin: 10px 0;
  display: flex;
  align-items: center;
  justify-content: space-evenly;

  button {
    font-size: 30px;
    background-color: transparent;
    border: none;
    cursor: pointer;
    opacity: 0.5;

    &.active,
    &:hover {
      opacity: 1;
    }
  }
`;

const Voted = styled("div")`
  font-size: 30px;
`;

const CommentBox = styled("input")`
  font-size: 16px;
  padding: 5px 0;
  border: 2px solid #666;
  border-top-left-radius: 10px;
  border-bottom-left-radius: 10px;
  width: 80%;
  margin-left: 8px;
  text-indent: 15px;
`;

const CommentBtn = styled("button")`
  font-size: 28px;
  padding: 0;
  background-color: #0074d9;
  color: white;
  border: none;
  text-align: center;
  width: 10%;
  border-top-right-radius: 10px;
  border-bottom-right-radius: 10px;
  margin-right: 8px;
`;

const CommentForm = styled("form")`
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 5px;
`;

class Post extends React.Component<any> {
  public state = {
    votingState: isLoggedIn() ? "idle" : "loggedout",
    score: 0,
    comment: "",
    commentingState: isLoggedIn() ? "idle" : "loggedout",
    comments: [],
    rotationBase: 45,
    rotationMultiplier: 0
  };

  constructor(props) {
    super(props);
    this.state.comments = props.comments;
    this.state.score = props.upVotes - props.downVotes;
  }

  public requestImageRotation = () => {
    axios({
      url: "http://viking.westus2.cloudapp.azure.com:5000/api/v1.0/predict",
      method: "post",
      responseType: "json",
      data: {
        url: this.props.photoUrl
      }
    })
      .then(res => {
        console.log(res);

        const rotationMultiplier = Object.keys(res.data.predictions)
          .sort()
          .map(key => [key, res.data.predictions[key]])
          .reduce(
            (greatest, current) =>
              greatest[1] > current[1] ? greatest : current
          )[0];

        this.setState({
          rotationBase: res.data.angle,
          rotationMultiplier
        });
      })
      .catch(console.error);
  };

  public upvote = () => {
    this.vote(true);
  };
  public downvote = () => {
    this.vote(false);
  };
  public handleComment = e => {
    this.setState({ comment: e.target.value });
  };
  public handleCommentSubmit = () => {
    this.setState({
      commentingState: "loading"
    });
    const formData = new FormData();
    formData.set("url", this.props.photoUrl);
    formData.set("comment", this.state.comment);
    axios
      .post("/api/member/comment", formData)
      .then(res => {
        this.setState({
          commentingState: "idle",
          comments: res.data.comments
        });
      })
      .catch(() => {
        this.setState({
          commentingState: "error"
        });
      });
  };
  public vote(isUpVote) {
    this.setState({
      votingState: "loading"
    });
    const formData = new FormData();
    formData.set("photoUrl", this.props.photoUrl);
    formData.set(isUpVote ? "upVote" : "downVote", "true");
    axios
      .post("/api/member/vote", formData)
      .then(res => {
        this.setState({
          votingState: isUpVote ? "upvoted" : "downvoted",
          score: res.data.upVotes - res.data.downVotes
        });
      })
      .catch(() => {
        this.setState({
          votingState: "error"
        });
      });
  }
  public render() {
    let votingComponent;

    if (this.state.votingState === "loggedout") {
      votingComponent = (
        <VotesContainer>
          <Voted>{this.state.score}</Voted>
          <span>log in to vote</span>
        </VotesContainer>
      );
    } else if (this.state.votingState === "loading") {
      votingComponent = (
        <VotesContainer>
          <Spinner name="line-scale-pulse-out-rapid" />
        </VotesContainer>
      );
    } else if (this.state.votingState === "idle") {
      votingComponent = (
        <VotesContainer>
          <Voted>{this.state.score}</Voted>
          <button onClick={this.upvote}>😎</button>
          <button onClick={this.downvote}>💩</button>
        </VotesContainer>
      );
    } else {
      votingComponent = (
        <VotesContainer>
          <Voted>{this.state.score}</Voted>
          <Voted>
            {this.state.votingState === "upvoted"
              ? "😎"
              : this.state.votingState === "downvoted"
                ? "💩"
                : "🚫"}
          </Voted>
        </VotesContainer>
      );
    }

    const commentingBox =
      this.state.commentingState === "idle" ? (
        <CommentForm onSubmit={this.handleCommentSubmit}>
          <CommentBox
            onChange={this.handleComment}
            placeholder="Comment on this Picture"
          />
          <CommentBtn type="submit">+</CommentBtn>
        </CommentForm>
      ) : this.state.commentingState === "loading" ? (
        <Spinner name="line-scale-pulse-out-rapid" />
      ) : null;
    return (
      <Container>
        <PosterContainer>
          <PosterImage
            onClick={this.requestImageRotation}
            src={`http://placecorgi.com/50/50?id=${Math.floor(
              Math.random() * 100
            )}`}
            alt="profile"
          />
          <PosterName>
            <Link to={`/feed/${this.props.displayName}`}>
              {this.props.displayName}
            </Link>
          </PosterName>
          <div>{distanceInWordsToNow(new Date(this.props.uploadDate))}</div>
        </PosterContainer>
        <MainImage
          src={this.props.photoUrl}
          rotationBase={this.state.rotationBase}
          rotationMultiplier={this.state.rotationMultiplier}
        />
        <div>
          {votingComponent}
          {this.state.comments.map((c, index) => (
            <Comment key={index} {...c} />
          ))}
        </div>
        {commentingBox}
      </Container>
    );
  }
}

export default Post;
