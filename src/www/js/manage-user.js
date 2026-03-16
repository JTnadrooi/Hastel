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
            window.location.href = "login.html";
        }, 1000);

    } catch (error) {
        UI.showError("logoutResponse", error);
    }
}

async function handleUserDelete() {
    const username = Auth.getUser().username;

    if (!confirm("Are you sure you want to delete your account? This action cannot be undone.")) {
        return;
    }

    try {
        const data = await API.request("/auth/delete-user", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username }),
            expectJson: false,
        });

        Auth.clearSession();

        UI.setResponse("deleteUserResponse", "Account deleted successfully. Redirecting...");

        setTimeout(() => { // give user time to read response.
            window.location.href = "login.html";
        }, 1500);

    } catch (error) {
        UI.showError("deleteUserResponse", error);
    }
}

async function handleChangePassword() {
    const username = Auth.getUser().username;

    const newPassword = document.getElementById("newPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    if (!newPassword || !confirmPassword) {
        UI.setResponse("changePasswordResponse", "Please fill in both password fields");
        return;
    }

    if (newPassword !== confirmPassword) {
        UI.setResponse("changePasswordResponse", "Passwords do not match");
        return;
    }

    if (newPassword.length < 1) {
        UI.setResponse("changePasswordResponse", "Password must be at least a characters long");
        return;
    }

    try {
        const data = await API.request("/auth/change-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, newPassword }),
            expectJson: false,
        });

        document.getElementById("newPassword").value = "";
        document.getElementById("confirmPassword").value = "";

        UI.setResponse("changePasswordResponse", "Password changed successfully!");
    } catch (error) {
        UI.showError("changePasswordResponse", error);
    }
}

if (Auth.isAuthenticated()) {
    displayUserInfo();
} else {
    window.location.href = "login.html";
}