import React from "react";
import { render } from "react-dom";
import App from "./App";
import axios from "axios";

// allows us to change the base URL of API calls based on if we're
// in dev or if we're in production
axios.defaults.baseURL = process.env.API_URL;

render(<App />, document.getElementById("root"));
