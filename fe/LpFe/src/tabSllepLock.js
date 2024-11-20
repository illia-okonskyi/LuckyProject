let lockResolver;
if (navigator && navigator.locks && navigator.locks.request) {
    const promise = new Promise((res) => {
        lockResolver = res;
    });

    navigator.locks.request(`lp-tabsleep-lock-${crypto.randomUUID()}`, { mode: "shared" }, () => {
        return promise;
    });
}

window.SHARED = {
  lockResolver
};