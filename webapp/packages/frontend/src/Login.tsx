import React from "react";

class Login extends React.Component {
  public login() {
    window.location.href = `${process.env.API_URL}/auth/twitter`;
  }
  public render() {
    return (
      <div>
        <h1>Login</h1>
        <button onClick={this.login}>Twitter</button>
      </div>
    );
  }
}

export default Login;
