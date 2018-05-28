export default function isLoggedIn() {
  return document.cookie.includes("username=");
}
