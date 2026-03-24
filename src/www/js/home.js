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
    const input = document.getElementById('input').value;
    const output = document.getElementById('output');

    if (!input.trim()) {
        output.value = '';
        return;
    }

    try {
        const testEnvironment = {
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

        const script = new SpellScript(input);
        output.value = '';
        await script.evaluate(testEnvironment);

    } catch (error) {
        output.value = `Error: ${error.message}`;
    }
}

document.addEventListener('DOMContentLoaded', function () {
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
});

if (Auth.isAuthenticated()) {
    displayUserInfo();
} else {
    window.location.href = "login.html";
}