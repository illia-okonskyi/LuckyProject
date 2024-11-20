import Typography from "@mui/material/Typography";

export function customColumn({
  id,
  title,
  renderer,
  target = null,
  details = true,
  headerLeft = false,
  headerRight = false
}) {
  let destinations = [];
  if (details) {
    destinations.push("details");
  }
  if (headerLeft) {
    destinations.push("header-left");
  }
  if (headerRight) {
    destinations.push("header-right");
  }

  return {
    id,
    title,
    target,
    destinations,
    renderer
  };
}

export function textColumn({
  id,
  title,
  target,
  details = true,
  headerLeft = false,
  headerRight = false
}) {
  function renderer(value) {
    return (<Typography variant="body1" color="secondary" textAlign="left">{value}</Typography>);
  }

  return customColumn({ id, title, renderer, target, details, headerLeft, headerRight });
}