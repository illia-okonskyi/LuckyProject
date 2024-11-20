import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";

import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ErrorOutlineRoundedIcon from "@mui/icons-material/ErrorOutlineRounded";
import CancelRoundedIcon from "@mui/icons-material/CancelRounded";
import ErrorRoundedIcon from "@mui/icons-material/ErrorRounded";

const LpValidationError = ({ message = null, icon = null }) => {
  return (
    <ListItem>
      <Stack direction="row" spacing={1} sx={{ alignItems: "center" }}>
        {icon ? icon : (<ErrorOutlineRoundedIcon color="error" />)}
        <Typography variant="caption" color="error">{message}</Typography>
      </Stack>
    </ListItem>
  );
};

const LpValidationSummaryFields = ({ errors, fieldNamesMap }) => {
  return Object.entries(errors).map(([vkey, errorJson]) => (
    <ListItem key={`f-${vkey}`}>
      <Stack spacing={1}>
        <Stack direction="row" spacing={1} sx={{ alignItems: "center" }}>
          <ErrorRoundedIcon color="error" />
          <Typography variant="caption" color="error">{fieldNamesMap[vkey] || vkey}</Typography>
        </Stack>
        <List dense>
          {JSON.parse(errorJson).map((e, i) => (<LpValidationError key={i} message={e} />))}
        </List>
      </Stack>
    </ListItem>
  ));
};

export function LpValidationFieldError({ validation, vkey }) {
  if (!validation?.errors[vkey]) {
    return null;
  }

  const errorJson = validation.errors[vkey];
  return (
    <List dense>
      {JSON.parse(errorJson).map((e, i) => (<LpValidationError key={i} message={e} />))}
    </List>
  );
}

export function LpValidationSummary({
  validation,
  includeCancelled = true,
  includeFields = true,
  fieldNamesMap = {},
}) {
  if (!validation) {
    return null;
  }

  return (
    <List dense>
      {(includeCancelled && validation.cancelled)
        ? (<LpValidationError
          key="c"
          message={validation.cancelled}
          icon={(<CancelRoundedIcon color="error" />)} />)
        : null
      }
      {includeFields
        ? (<LpValidationSummaryFields errors={validation.errors} fieldNamesMap={fieldNamesMap} />)
        : null
      }
    </List>
  );
}
