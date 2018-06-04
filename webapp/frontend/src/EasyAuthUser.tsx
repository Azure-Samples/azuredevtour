import axios from "axios";
import React from "react";

class EasyAuthUser extends React.Component {
  public state = {
    photoUrl: "",
    twitterHandle: ""
  };

  public componentWillMount() {
    axios
      .get("https://apac-azure-photo-tour.azurewebsites.net/.auth/me")
      .then(response => {
        const identity = Array.isArray(response.data) ? response.data[0] : null;
        if (identity) {
          const twitterHandle = identity.user_id;
          const claims = identity.user_claims;
          const photoClaim =
            Array.isArray(claims) &&
            claims.find(c => c.typ === "urn:twitter:profile_image_url_https");

          if (twitterHandle && photoClaim) {
            const newState = { twitterHandle, photoUrl: photoClaim.val };
            this.setState(newState);
          }
        }
      })
      .catch(error => console.log);
  }

  public render() {
    if (this.state.photoUrl && this.state.twitterHandle) {
      return (
        <div>
          <img src={this.state.photoUrl} /> {this.state.twitterHandle}
        </div>
      );
    } else {
      return <div />;
    }
  }
}

export default EasyAuthUser;
