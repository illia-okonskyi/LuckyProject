import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";

export function buildCrumbs(crumbs, navigate, closeMobile) {
  return crumbs.map((c, i) => {
    if (c.to === null) {
      const color = (i < (crumbs.length - 1)) ? "primary" : "secondary";
      return (<Typography key={c.id} variant="body1" color={color}>{c.title}</Typography>);
    }

    return (
      <Link
        variant="body1"
        key={c.id}
        style={{ cursor: "pointer" }}
        onClick={() => {
          navigate(c.to);
          if (closeMobile) {
            closeMobile();
          }
        }}>
        {c.title}
      </Link>
    );
  });
}
