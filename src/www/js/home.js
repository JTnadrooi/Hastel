function displayUserInfo() {
    if (!Auth.requireAuth()) return;

    const user = Auth.getUser();
    if (!user) {
        window.location.href = "index.html";
        return;
    }

    $("userInfo").innerHTML = `
        <p>Logged in as: <strong>${user.username || 'Unknown'}</strong></p>
    `;
}

async function handleLogout() {
    try {
        Auth.clearSession();
        UI.setResponse("logoutResponse", "Logout successful!");

        setTimeout(() => {
            window.location.href = "index.html";
        }, 1000);

    } catch (error) {
        UI.showError("logoutResponse", error);
    }
}

if (Auth.isAuthenticated()) {
    displayUserInfo();
} else {
    window.location.href = "index.html";
}