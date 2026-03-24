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

async function parseSpellscript() {
    const input = document.getElementById('input');
    const output = document.getElementById('output');

    if (!input.value.trim()) {
        output.value = '';
        return;
    }

    try {
        const splEnvironment = {
            spellbooks: [
                {
                    namespace: "core",
                    print: (...args) => {
                        output.value += args.join(' ') + '\n';
                        return args.join(' ');
                    },
                    add: (a, b) => Number(a) + Number(b),
                    subtract: (a, b) => Number(a) - Number(b),
                    multiply: (a, b) => Number(a) * Number(b),
                    divide: (a, b) => Number(a) / Number(b),
                    concat: (...args) => args.join(''),
                    toString: (val) => String(val),
                    toNumber: (val) => Number(val),
                    close: () => { }
                }
            ],
            memorySize: 32,
            memory: [],
            initialArgs: []
        };

        const script = new SpellScript(input.value);
        output.value = '';
        await script.evaluate(splEnvironment);

    } catch (error) {
        output.value = `Error: ${error.message}`;
    }
}

async function updateLoadSelect() {
    try {
        const username = Auth.getUser().username;

        const scripts = await API.request("/scripts/list", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username }),
            expectJson: true
        });

        const select = document.getElementById('loadSelect');

        select.innerHTML = '<option value="">Select a script to load...</option>';

        scripts.forEach(script => {
            const option = document.createElement('option');
            option.value = script;
            option.textContent = script;
            select.appendChild(option);
        });

        select.onchange = async function () {
            if (this.value) {
                await loadScript(this.value);
                this.value = '';
            }
        };
    } catch (error) {
        console.error('Failed to load scripts:', error);
        UI.setResponse("scriptActionResponse", "Failed to load scripts list: " + error.message);

        const select = document.getElementById('loadSelect');
        select.innerHTML = '<option value="">Error loading scripts</option>';
    }
}

async function saveScript() {
    const input = document.getElementById('input');
    const username = Auth.getUser().username;

    const text = input.value;
    const name = document.getElementById('scriptName').value;

    if (!text) {
        UI.setResponse("scriptActionResponse", "Cannot save empty script.");
        return;
    }

    if (!name) {
        UI.setResponse("scriptActionResponse", "Cannot save unnamed script.");
        return;
    }

    try {
        const data = await API.request("/scripts/save-script", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, text, name }),
            expectJson: false,
        });

        UI.setResponse("scriptActionResponse", "Script saved successfully!");
    } catch (error) {
        UI.showError("scriptActionResponse", error);
    }

    await updateLoadSelect();
}


async function loadScript() {
    const name = document.getElementById("loadSelect").value;
    const input = document.getElementById('input');
    const output = document.getElementById('output');
    const nameElement = document.getElementById('scriptName');

    try {
        const data = await API.request("/scripts/load-script", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name }),
            expectJson: true,
        });

        input.value = data.text;
        nameElement.value = data.name;

        UI.setResponse("scriptActionResponse", "Script loaded successfully!");
    } catch (error) {
        UI.showError("scriptActionResponse", error);
    }
}

document.addEventListener('DOMContentLoaded', async function () {
    const inputElement = document.getElementById('input');
    if (inputElement) {
        inputElement.addEventListener('input', parseSpellscript);
        document.getElementById('input').addEventListener('keydown', function (e) {
            if (e.key === 'Tab') {
                e.preventDefault();

                const start = this.selectionStart;
                const end = this.selectionEnd;

                this.value = this.value.substring(0, start) + '\t' + this.value.substring(end);

                this.selectionStart = this.selectionEnd = start + 1;
            }
        });
    }

    if (typeof SpellScript === 'undefined') {
        console.error('Spellscript library not loaded');
        document.getElementById('output').value = 'Error: Spellscript library failed to load';
    }

    await updateLoadSelect();
});

if (Auth.isAuthenticated()) {
    displayUserInfo();
} else {
    window.location.href = "login.html";
}