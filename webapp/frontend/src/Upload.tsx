import React from "react";
import ImageUploader from "react-images-upload";
import axios from "axios";
import styled from "react-emotion";
import { Redirect } from "react-router-dom";
import Spinner from "react-spinkit";

const LoadingContainer = styled("div")`
  width: 100%;
  display: flex;
  margin-top: 200px;
  justify-content: center;
`;

const Caption = styled("input")`
  font-size: 20px;
  padding: 5px 15px;
  width: 90%;
  max-width: 600px;
  display: block;
  margin: 10px auto;
  border-radius: 5px;
  border: 2px solid #666;
`;

const SubmitButton = styled("button")`
  background-color: #0074d9;
  color: white;
  display: block;
  margin: 15px auto;
  border: none;
  font-size: 30px;
  padding: 8px 25px;
  border-radius: 30px;
  cursor: pointer;

  &:hover {
    background-color: #1a8ef3;
  }

  &:active {
    background-color: #005bc0;
  }
`;

interface UploadState {
  pic: null | Blob;
  redirect: boolean | string;
  loading: boolean;
  caption: string;
}

class Upload extends React.Component<{}, UploadState> {
  public state = { pic: null, redirect: false, loading: false, caption: "" };

  public canvas = null;

  public onDrop = (pictures: Blob[]) => {
    if (pictures.length) {
      this.setState({
        pic: pictures[0]
      });
    }
  };

  public submitPicture = e => {
    e.preventDefault();

    this.setState({
      loading: true
    });

    const pic = this.state.pic || "error";

    if (pic === "error") {
      alert("Image was not chosen correctly");
      return;
    }

    const formData = new FormData();
    formData.set("caption", this.state.caption);
    formData.set("pic", pic);
    axios
      .post("/api/member/pic", formData)
      .then(() => {
        this.setState({
          redirect: true
        });
      })
      .catch(() => {
        this.setState({
          loading: false
        });
        alert("Upload failed");
      });
  };

  public handleCaption = e => {
    this.setState({
      caption: e.target.value
    });
  };

  public render() {
    if (this.state.redirect) {
      return <Redirect to="/" />;
    }
    if (this.state.loading) {
      return (
        <LoadingContainer>
          <Spinner name="line-scale-pulse-out-rapid" />
        </LoadingContainer>
      );
    }
    return (
      <div>
        <ImageUploader
          withIcon={true}
          buttonText="Choose image"
          onChange={this.onDrop}
          imgExtension={[".jpg", ".jpeg", ".png", ".gif"]}
          maxFileSize={2000000}
          label="2MB max file size; accepts jpg|jpeg|png|gif"
          singleImage
        />
        <Caption
          onChange={this.handleCaption}
          value={this.state.caption}
          placeholder="Image Caption"
        />
        {this.state.pic ? (
          <SubmitButton onClick={this.submitPicture}>submit</SubmitButton>
        ) : null}
      </div>
    );
  }
}

export default Upload;
