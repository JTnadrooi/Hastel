const $ = (id) => document.getElementById(id);

const API = {
    BASE_URL: "http://127.0.0.1:5000",

    async request(endpoint, options = {}) {
        const response = await fetch(`${this.BASE_URL}${endpoint}`, {
            credentials: 'include',
            ...options
        });

        if (!response.ok) {
            if (response.status === 401) throw new Error("Invalid username or password");
            throw new Error(`Server error: ${response.status}`);
        }

        return options.expectJson === false ? response.text() : response.json();
    }
};

const Auth = {
    setSession(token, user) {
        sessionStorage.setItem("authToken", token);
        sessionStorage.setItem("userData", JSON.stringify(user));
    },

    clearSession() {
        sessionStorage.removeItem("authToken");
        sessionStorage.removeItem("userData");
    },

    getUser() {
        const data = sessionStorage.getItem("userData");
        if (!data || data === "null" || data === "undefined") return null;
        try {
            return JSON.parse(data);
        } catch {
            return null;
        }
    },

    isAuthenticated() {
        return !!sessionStorage.getItem("authToken") && !!this.getUser();
    },

    requireAuth(redirectTo = "index.html") {
        if (!this.isAuthenticated()) {
            window.location.href = redirectTo;
            return false;
        }
        return true;
    },

    redirectIfAuthenticated(redirectTo = "home.html") {
        if (this.isAuthenticated()) {
            window.location.href = redirectTo;
            return true;
        }
        return false;
    }
};

const UI = {
    setResponse(elementId, message) {
        const element = $(elementId);
        if (element) element.innerText = message;
    },

    showError(elementId, error) {
        this.setResponse(elementId, "Error: " + error.message);
        console.error(error);
    }
};