//#region Imports
import { useState } from "react";
import { useTranslation } from "react-i18next";

import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import Select from "@mui/material/Select";
import ToggleButton from "@mui/material/ToggleButton";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import Pagination from "@mui/material/Pagination";
import LpDataList from "./LpDataList";
import LpLoadGuard from "../LpLoadGuard";

import FilterAltRoundedIcon from "@mui/icons-material/FilterAltRounded";
import RestartAltRoundedIcon from "@mui/icons-material/RestartAltRounded";
//#endregion

//#region Internals
function LpFilterOrder({
  fop,
  orders = null,
  children,
  header = null,
  direction = "row",
  headerProps = { variant: "h6", color: "primary" },
  defaultShowFilters = false,
  onApply = null
}) {
  const [showFilters, setShowFilters] = useState(defaultShowFilters);
  return (
    <Stack spacing={1} direction={direction} justifyContent="center" alignItems="center">
      <Stack spacing={1} direction="row" justifyItems="center" alignItems="center">
        {header && (<Typography {...headerProps}>{header}</Typography>)}
        {orders && onApply && (<Select
          size="small"
          value={fop.order}
          onChange={(e) => { fop.setOrder(e.target.value); onApply(e.target.value); }}>
          {Object.entries(orders).map(([k, v]) => (<MenuItem key={k} value={k}>{v}</MenuItem>))}
        </Select>)}

        {children &&
          (<ToggleButton size="small" value="1" selected={showFilters} onChange={() => setShowFilters((s) => !s)}>
            <FilterAltRoundedIcon />
          </ToggleButton>)}

        {onApply && (<IconButton onClick={() => onApply(fop.order)}><RestartAltRoundedIcon /></IconButton>)}

      </Stack>

      {children && showFilters && children}

    </Stack>
  );
}

function LpPagination({ pagination, onPageChanged }) {
  const { t } = useTranslation(["lp-ui-shell"]);
  const firstPageItem = pagination?.firstPageItem || 0;
  const lastPageItem = pagination?.lastPageItem || 0;
  const totalItemsCount = pagination?.totalItemsCount || 0;
  const totalPagesCount = pagination?.totalPagesCount || 1;
  const page = pagination?.page || 1;
  return (
    <Stack spacing={1} direction="row" alignItems="center" justifyContent="center">
      <Typography variant="caption">
        {t("lp-ui-shell:s.components.lpFopDataList.pagination",
          { firstPageItem, lastPageItem, totalItemsCount })}
      </Typography>
      <Pagination
        variant="outlined"
        shape="rounded"
        color="primary"
        count={totalPagesCount}
        page={page}
        showFirstButton
        showLastButton
        onChange={(e, v) => onPageChanged(v)}>
      </Pagination>
    </Stack>
  );
}
//#endregion

//#region LpFopDataList
export default function LpFopDataList({
  items,
  columns,
  fop,
  onFopChanged,
  header = null,
  headerProps = { variant: "h6", color: "primary" },
  orders = null,
  renderFilters = null,
  defaultShowFilters = false,
  filterOrderDirection = "row",
  ctx = null,
} = {}) {
  return (
    <Stack spacing={1} width="100%">
      <LpFilterOrder
        fop={fop}
        orders={orders}
        direction={filterOrderDirection}
        header={header}
        headerProps={headerProps}
        defaultShowFilters={defaultShowFilters}
        onApply={(order) => onFopChanged({
          filters: fop.filters,
          order,
          page: items?.pagination?.page || 1
        }
        )}>
        {renderFilters && renderFilters(fop.filters, fop.setFilter)}
      </LpFilterOrder>
      <LpLoadGuard loading={!items}>
        <LpDataList items={items?.items} columns={columns} fop={fop} ctx={ctx}/>
        <LpPagination
          pagination={items?.pagination}
          onPageChanged={(page) => {
            fop.setPage(page);
            onFopChanged({ filters: fop.filters, order: fop.order, page });
          }}
        />
      </LpLoadGuard>
    </Stack>
  );
}
//#endregion