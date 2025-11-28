// Swagger Auth - Inject JWT token from localStorage into all Swagger API requests
(function () {
    // Wait for Swagger UI to load
    const originalFetch = window.fetch;

    window.fetch = function (...args) {
        let [resource, config] = args;

        // Get token from localStorage
        const token = localStorage.getItem('swagger_token');

        if (token) {
            // Add Authorization header to all requests
            config = config || {};
            config.headers = config.headers || {};
            config.headers['Authorization'] = `Bearer ${token}`;
        }

        return originalFetch(resource, config);
    };

    // Also inject into Swagger's request interceptor
    if (window.ui) {
        window.ui.getConfigs().requestInterceptor = (req) => {
            const token = localStorage.getItem('swagger_token');
            if (token) {
                req.headers['Authorization'] = `Bearer ${token}`;
            }
            return req;
        };
    }

    // Add logout button
    setTimeout(() => {
        const topbar = document.querySelector('.topbar');
        if (topbar && !document.getElementById('swagger-logout')) {
            const logoutBtn = document.createElement('button');
            logoutBtn.id = 'swagger-logout';
            logoutBtn.textContent = 'Logout';
            logoutBtn.style.cssText = `
                position: absolute;
                right: 20px;
                top: 15px;
                padding: 8px 20px;
                background: #f44336;
                color: white;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                font-size: 14px;
                font-weight: 600;
            `;
            logoutBtn.onclick = () => {
                localStorage.removeItem('swagger_token');
                // Clear cookie
                document.cookie = "swagger_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                window.location.href = '/swagger-login.html';
            };
            topbar.appendChild(logoutBtn);
        }
    }, 1000);
})();
