function reloadPage() {
    location.reload();
}

window.NavigateTo = (url) => {
    window.location.replace(url);
};

// 他のタブでのサインインを検知
window.registerSignOutListener = function (dotNetObject) {
    window.dotNetTabHandler = dotNetObject;

    window.addEventListener("storage", function (event) {
        if (event.key === "userSignedIn") {
            if (window.dotNetTabHandler) {
                window.dotNetTabHandler.invokeMethodAsync('OnUserSignedIn');
            }
        }
    });
};

function goBackToHome() {
    window.history.pushState(null, "", window.location.href);

    window.onpopstate = function (event) {
        // 戻るボタンが押された場合、ホーム画面に強制遷移
        window.location.href = "/";
    };
}