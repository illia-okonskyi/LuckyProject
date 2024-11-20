import { filter } from "lodash";
import { useEffect } from "react";

export const useTitle = (subtitle = null) => {
  useEffect(() => {
    document.title = filter(["LP Auth Server", subtitle]).join(" | ");
  }, [subtitle]);
};
