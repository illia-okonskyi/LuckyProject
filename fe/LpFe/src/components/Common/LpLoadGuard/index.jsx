import LpLoadSpinner from "../LpLoadSpinner";

export default function LpLoadGuard({ children, loading = false, spinner = {} }) {
  if (loading) {
    return (<LpLoadSpinner {...spinner} />);
  } else {
    return children;
  }
}
