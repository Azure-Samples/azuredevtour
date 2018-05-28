import React from "react";
import {
  BrowserRouter as Router,
  NavLink,
  Route,
  Switch
} from "react-router-dom";
import styled, { injectGlobal } from "react-emotion";
import Feed from "./Feed";
import Upload from "./Upload";
import Login from "./Login";
import EasyAuthUser from "./EasyAuthUser";
import isLoggedIn from "./isLoggedIn";

// tslint:disable-next-line:no-unused-expression
injectGlobal`
  * {
    box-sizing: border-box;
  }

  body {
    font-family: "Helvetica Neue", "Helvetica", sans-serif;
  }
`;

const NavLi = styled("li")`
  font-weight: lighter;
  font-size: 20px;
  list-style: none;
  a {
    text-decoration: none;
    color: #333;
  }
`;

const NavList = styled("ul")`
  display: flex;
  justify-content: space-between;
  width: 95%;
  max-width: 600px;
  margin: 0 auto;
  align-items: center;
  padding: 0;
`;

class App extends React.Component {
  public render() {
    return (
      <Router>
        <div>
          <NavList>
            <NavLi>
              <NavLink to="/">Home</NavLink>
            </NavLi>
            <NavLi>
              {isLoggedIn() ? (
                <NavLink to="/upload">Upload</NavLink>
              ) : (
                <NavLink to="/login">Log In</NavLink>
              )}
            </NavLi>
          </NavList>
          <EasyAuthUser />
          <hr />
          <Switch>
            <Route exact path="/" component={Feed} />
            <Route exact path="/feed/:username" component={Feed} />
            <Route path="/upload" component={Upload} />
            <Route path="/login" component={Login} />
          </Switch>
        </div>
      </Router>
    );
  }
}

export default App;
