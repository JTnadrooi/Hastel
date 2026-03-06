async function handleLogin() {
    const username = $("usernameInput").value.trim();
    const password = $("passwordInput").value;

    if (!username || !password) {
        return UI.setResponse("loginResponse", "Please enter both username and password.");
    }

    try {
        const data = await API.request("/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        if (!data.token) {
            throw new Error("Invalid response from server");
        }

        const user = { username: username };

        Auth.setSession(data.token, user);
        window.location.href = "home.html";

    } catch (error) {
        UI.showError("loginResponse", error);
    }
}

if (Auth.isAuthenticated()) {
    window.location.href = "home.html";
}