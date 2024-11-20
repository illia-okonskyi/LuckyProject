import { useCallback } from "react";

export const useAuthDetails = (searchParams) => {
  return useCallback(() => {
    const clientId = searchParams.get("client_id");
    const clientDisplayName = searchParams.get("lp_cdn");
    const userId = searchParams.get("lp_uid");
    const userFullName = searchParams.get("lp_ufn");
  
    return {
        clientId: clientId? clientId : null,
        clientDisplayName: clientDisplayName? clientDisplayName : null,
        userId: userId? userId : null,
        userFullName: userFullName? userFullName : null,
      };    
  },
  [searchParams]);
};