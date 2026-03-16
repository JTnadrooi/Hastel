async function handleRegister() {
    const username = $("usernameInput").value.trim();
    const password = $("passwordInput").value;

    if (!username || !password) {
        return UI.setResponse("registerResponse", "Please enter both username and password.");
    }

    try {
        const data = await API.request("/auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
            expectJson: false,
        });

        setTimeout(() => { // give user time to read response.
            window.location.href = "login.html";
        }, 1500);

    } catch (error) {
        UI.showError("registerResponse", error);
    }
}