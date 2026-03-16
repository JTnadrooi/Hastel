function displayUserInfo() {
    const user = Auth.getUser();

    $("userInfo").innerHTML = `
        <p>Logged in as: <strong>${user.username || 'Unknown'}</strong></p>
    `;
}

async function handleLogout() {
    try {
        Auth.clearSession();
        UI.setResponse("logoutResponse", "Logout successful!");

        setTimeout(() => { // give user time to read the response.
            window.location.href = "login.html";
        }, 1000);

    } catch (error) {
        UI.showError("logoutResponse", error);
    }
}

if (Auth.isAuthenticated()) {
    displayUserInfo();
} else {
    window.location.href = "login.html";
}