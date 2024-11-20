import { useLocalStorage } from "./useLocalStorage";

export const useUserInfo = () => {
    return useLocalStorage("userInfo", { isLoggedIn: false, userId: null });
};
