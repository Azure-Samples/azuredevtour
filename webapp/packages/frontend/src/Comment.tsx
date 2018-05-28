import React from "react";
import { distanceInWordsToNow } from "date-fns";
import { Link } from "react-router-dom";
import styled from "react-emotion";

const Container = styled("div")`
  margin: 15px 8px;
`;

const Text = styled("div")`
  margin-bottom: 5px;

  a {
    margin-right: 5px;
    font-weight: bold;
    color: #333;
    text-decoration: none;
  }
`;

const Time = styled("div")`
  color: #999;
  font-size: 10px;
  text-transform: uppercase;
`;

function Comment(props) {
  return (
    <Container>
      <Text>
        <Link to={`/feed/${props.displayName}`}>{props.displayName}</Link>
        {props.text}
      </Text>
      <Time>{distanceInWordsToNow(props.date)}</Time>
    </Container>
  );
}

export default Comment;
