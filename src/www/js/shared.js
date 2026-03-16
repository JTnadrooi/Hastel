const $ = (id) => document.getElementById(id);

const API = {
    BASE_URL: "http://127.0.0.1:5000", // change when live.

    async request(endpoint, options = {}) {
        const response = await fetch(`${this.BASE_URL}${endpoint}`, {
            credentials: 'include', // so it sends cookies.
            ...options
        });

        if (!response.ok) {
            const text = await response.text();
            throw new Error(text || `Server error: ${response.status}`); // some backend errors have text.
        }

        return options.expectJson === false ? response.text() : response.json(); // not all backend responses are JSON, will change later.
    }
};

const Auth = {
    /**
     * Stores authentication token and user data in session storage.
     * @param {string} token - Authentication token
     * @param {Object} user - User object
     */
    setSession(token, user) {
        sessionStorage.setItem("authToken", token);
        sessionStorage.setItem("userData", JSON.stringify(user));
    },

    /** Removes authentication data from session storage. */
    clearSession() {
        sessionStorage.removeItem("authToken");
        sessionStorage.removeItem("userData");
    },

    /**
     * Retrieves and parses user data from session storage.
     * @returns {Object|null} User object or null if not found/invalid
     */
    getUser() {
        const data = sessionStorage.getItem("userData");
        if (!data || data === "null" || data === "undefined") return null;
        try {
            return JSON.parse(data);
        } catch {
            return null;
        }
    },

    /** @returns {boolean} True if user has valid auth token and user data */
    isAuthenticated() {
        return !!sessionStorage.getItem("authToken") && !!this.getUser();
    },

    /**
     * Redirects to specified path if user is authenticated.
     * @param {string} [redirectTo="home.html"] - Redirect destination
     * @returns {boolean} True if redirect occurred
     */
    redirectIfAuthenticated(redirectTo = "home.html") {
        if (this.isAuthenticated()) {
            window.location.href = redirectTo;
            return true;
        }
        return false;
    }
};

const UI = {
    /**
     * Sets text content of an element.
     * @param {string} elementId - Element selector
     * @param {string} message - Text to display
     */
    setResponse(elementId, message) {
        const element = $(elementId);
        if (element) element.innerText = message;
    },

    /**
     * Displays error message and logs to console.
     * @param {string} elementId - Element selector
     * @param {Error} error - Error object
     */
    showError(elementId, error) {
        this.setResponse(elementId, "Error: " + error.message);
        console.error(error);
    }
};