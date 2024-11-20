//#region Imports
import { useState } from "react";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import Grid2 from "@mui/material/Grid2";
import Paper from "@mui/material/Paper";
import IconButton from "@mui/material/IconButton";

import { useTranslation } from "react-i18next";

import ArrowDropDown from "@mui/icons-material/ArrowDropDown";
import ArrowDropUp from "@mui/icons-material/ArrowDropUp";
//#endregion

//#region Internals
function LpDataListItemColumnRender({ item, column, fop, ctx }) {
  const value = column.target !== null ? item[column.target] : null;
  return column.renderer(value, item, fop, ctx);
}

function LpDataListItemColumn({ item, column, fop, ctx }) {
  return (
    <>
      <Grid2 size={1}>
        <Typography variant="body1" fontWeight="700" color="primary" textAlign="left">
          {column.title}
        </Typography>
      </Grid2>
      <Grid2 size={2}>
        <LpDataListItemColumnRender item={item} column={column} fop={fop} ctx={ctx}/>
      </Grid2>
    </>
  );
}

function LpDataListItemHeader({ item, columns, showDetails, toggleShowDetails, fop, ctx }) {
  const showDetailsButton = columns.filter((c) => c.destinations.includes("details")).length > 0;

  return (
    <Stack
      direction="row"
      justifyContent="space-between"
      alignItems="center"
      width="100%"
      >
      <Stack
        direction="row"
        spacing={1}
        alignItems="center"
        divider={(<Divider flexItem orientation="vertical" />)}>
        {columns
          .filter((c) => c.destinations.includes("header-left"))
          .map((c, i) => (<LpDataListItemColumnRender
            key={c.id || i}
            item={item}
            column={c}
            fop={fop}
            ctx={ctx}/>))
        }
      </Stack>
      <Stack
        direction="row"
        spacing={1}
        alignItems="center"
        divider={(<Divider flexItem orientation="vertical" />)}>
        {columns
          .filter((c) => c.destinations.includes("header-right"))
          .map((c, i) => (<LpDataListItemColumnRender
            key={c.id || i}
            item={item}
            column={c}
            fop={fop}
            ctx={ctx}/>))
        }
        {showDetailsButton
          ? (
          <IconButton size="small" onClick={toggleShowDetails}>
          {showDetails ? (<ArrowDropUp />) : (<ArrowDropDown />)}
          </IconButton>
          )
          : null
        }
      </Stack>
    </Stack>
  );
}

function LpDataListItem({ item, columns, elevation, defaultShowDetails, fop, ctx }) {
  const [showDetails, setShowDetails] = useState(defaultShowDetails);

  const toggleShowDetails = () => {
    setShowDetails((d) => !d);
  };

  return (
    <Paper elevation={elevation}>
      <Stack spacing={1} padding={1}>
        <LpDataListItemHeader
          item={item}
          columns={columns}
          showDetails={showDetails}
          fop={fop}
          ctx={ctx}
          toggleShowDetails={toggleShowDetails} />
        <Paper elevation={elevation + 2} sx={{ display: showDetails ? "block" : "none" }} >
          <Grid2
            container
            padding={1}
            rowSpacing={1}
            columnSpacing={1}
            alignItems="center"
            columns={3}>
            {columns
              .filter((c) => c.destinations.includes("details"))
              .map((c, i) => (<LpDataListItemColumn
                key={c.id || i}
                item={item}
                column={c}
                fop={fop}
                ctx={ctx}/>))}
          </Grid2>

        </Paper>
      </Stack>

    </Paper>
  );
}
//#endregion

//#region LpDataList
export default function LpDataList({
  items,
  columns,
  elevation = 2,
  defaultShowDetails = false,
  fop = null,
  ctx = null
} = {}) {
  const { t } = useTranslation(["lp-ui-shell"]);
  return (
    <Stack spacing={1} useFlexGap>
      {(items && items.length > 0)
        ? items.map((item) => (<LpDataListItem
          key={item.id}
          item={item}
          columns={columns}
          elevation={elevation}
          defaultShowDetails={defaultShowDetails}
          fop={fop}
          ctx={ctx}
        />))
        : (<Paper elevation={elevation}>
            <Typography variant="body1" textAlign="center">
              {t("lp-ui-shell:s.components.lpDataList.noItems")}
            </Typography>
          </Paper>)
      }
    </Stack>);
}
//#endregion