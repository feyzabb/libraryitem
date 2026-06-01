/* ==========================================================
   KIRALABUNU - MODERN MINIMALIST EXPERIENCE ENGINE (JS)
   ========================================================== */

document.addEventListener("DOMContentLoaded", () => {
    // 1. Trigger subtle page fade-in and slide-up transition on load
    const pageWrapper = document.getElementById("page-wrapper");
    if (pageWrapper) {
        // Soft timeout to ensure DOM styles are ready for transition
        setTimeout(() => {
            pageWrapper.classList.add("loaded");
        }, 80);
    }

    // 2. Initialize Dynamic Session/Auth Management
    initAuthManagement();

    // 3. Initialize Clean Simple SPA Page Transitions
    initSmoothSPATransitions();

    // 4. Initialize Hover Micro-interactions
    initMicroInteractions();

    // 5. Initialize Kirala Button Auth Check
    initKiralaButtonAuth();
});

/* ==========================================================
   1. DINAİK OTURUM YÖNETİMİ (LOGIN STATE UI)
   ========================================================== */
function initAuthManagement() {
    // Sync client-side session with backend state
    const serverAuthEl = document.getElementById("server-auth-state");
    if (serverAuthEl) {
        const isServerAuthenticated = serverAuthEl.getAttribute("data-authenticated") === "true";
        if (isServerAuthenticated) {
            localStorage.setItem("isLoggedIn", "true");
        }
    }

    // Update UI components according to session state
    updateAuthUI();

    // Intercept login form submission to simulate login state in local storage
    document.addEventListener("submit", (e) => {
        const loginForm = e.target.closest("form");
        if (loginForm && loginForm.action.includes("Login")) {
            // Set session key in local storage so client-side remembers it
            localStorage.setItem("isLoggedIn", "true");
        }
    });
}

function updateAuthUI() {
    const isLoggedIn = localStorage.getItem("isLoggedIn") === "true";
    
    // Auth containers on layouts & homepage
    const loginHomeBtn = document.getElementById("btn-login-home");
    const loginLayoutContainer = document.getElementById("auth-actions-layout");

    // The beautiful "Hesabım" dropdown markup
    const dropdownHtml = `
        <div class="dropdown" id="user-menu-dropdown">
            <button class="btn btn-link dropdown-toggle text-dark fw-bold d-flex align-items-center gap-2 text-decoration-none py-1 px-3" 
                    type="button" id="dropdownUserMenu" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-person-circle fs-4" style="color: var(--primary);"></i>
                <span class="d-none d-md-inline">Hesabım</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end border-0 shadow-lg mt-2" aria-labelledby="dropdownUserMenu" 
                style="border-radius: var(--border-radius-md); min-width: 170px;">
                <li>
                    <a class="dropdown-item fw-bold py-2" href="/Account/MyItems">
                        <i class="bi bi-collection-fill me-2" style="color: var(--primary);"></i> İlanlarım
                    </a>
                </li>
                <li>
                    <a class="dropdown-item fw-bold py-2" href="/Account/Basket">
                        <i class="bi bi-basket-fill me-2" style="color: var(--primary);"></i> Sepetim
                    </a>
                </li>
                <li><hr class="dropdown-divider"></li>
                <li>
                    <button class="dropdown-item fw-bold py-2 text-danger" id="btn-logout-sim">
                        <i class="bi bi-box-arrow-right me-2"></i> Çıkış Yap
                    </button>
                </li>
            </ul>
        </div>
    `;

    if (isLoggedIn) {
        // A. Home Page Navbar Button handling
        if (loginHomeBtn) {
            loginHomeBtn.style.display = "none";
            // Check if dropdown already exists, if not, insert it
            if (!document.getElementById("user-menu-dropdown-home")) {
                const tempDiv = document.createElement("div");
                tempDiv.innerHTML = dropdownHtml.replace('id="user-menu-dropdown"', 'id="user-menu-dropdown-home"');
                loginHomeBtn.parentNode.insertBefore(tempDiv.firstElementChild, loginHomeBtn);
            }
        }

        // B. Layout Navbar container handling
        if (loginLayoutContainer) {
            loginLayoutContainer.innerHTML = dropdownHtml.replace('id="user-menu-dropdown"', 'id="user-menu-dropdown-layout"');
        }
    } else {
        // If logged out: restore default visual states
        if (loginHomeBtn) {
            loginHomeBtn.style.display = "inline-block";
            const currentDrop = document.getElementById("user-menu-dropdown-home");
            if (currentDrop) currentDrop.remove();
        }

        if (loginLayoutContainer) {
            loginLayoutContainer.innerHTML = `
                <button type="button" class="btn btn-outline-secondary rounded-pill px-3 fw-bold btn-sm" data-bs-toggle="modal" data-bs-target="#loginModal">
                    <i class="bi bi-person-circle me-1"></i> Giriş Yap
                </button>
            `;
        }
    }

    // Attach click events to all "Çıkış Yap" buttons
    const logoutButtons = document.querySelectorAll("#btn-logout-sim");
    logoutButtons.forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            // Clear local storage session
            localStorage.removeItem("isLoggedIn");
            
            // Redirect to backend logout endpoint or reload
            window.location.href = "/Account/Logout";
            
            // Fallback reload in case logout route differs
            setTimeout(() => {
                window.location.href = "/";
            }, 500);
        });
    });
}

/* ==========================================================
   2. SMOOTH SPA PAGE TRANSITIONS (FADE & SLIDE)
   ========================================================== */
function initSmoothSPATransitions() {
    const wrapper = document.getElementById("page-wrapper");
    if (!wrapper) return;

    // Attach link click interception
    document.addEventListener("click", (e) => {
        const link = e.target.closest("a");
        if (!link) return;

        // Skip utility, modal, external or anchor links
        if (
            link.getAttribute("target") === "_blank" ||
            link.getAttribute("data-no-transition") !== null ||
            link.href.startsWith("javascript:") ||
            link.getAttribute("data-bs-toggle") ||
            link.hash ||
            !isLocalLink(link.href)
        ) {
            return;
        }

        e.preventDefault();
        navigateTo(link.href);
    });

    // Handle Form Search GET queries smoothly
    document.addEventListener("submit", (e) => {
        const form = e.target;
        if (form.method.toLowerCase() === "get" && isLocalLink(form.action)) {
            e.preventDefault();
            const formData = new FormData(form);
            const params = new URLSearchParams(formData).toString();
            const actionUrl = form.getAttribute("action") || window.location.pathname;
            const fullUrl = actionUrl + "?" + params;
            navigateTo(fullUrl);
        }
    });

    // Handle standard browser back/forward buttons
    window.addEventListener("popstate", () => {
        navigateTo(window.location.href, false);
    });

    function isLocalLink(url) {
        try {
            const parsed = new URL(url);
            return parsed.origin === window.location.origin;
        } catch {
            return url.startsWith("/") || !url.includes("://");
        }
    }

    function navigateTo(url, pushState = true) {
        // Phase 1: Fade out current page wrapper smoothly
        wrapper.classList.remove("loaded");

        // Wait for fade-out duration (400ms) before swapping content
        setTimeout(() => {
            fetchPage(url, pushState);
        }, 400);
    }

    function fetchPage(url, pushState) {
        fetch(url)
            .then((res) => {
                if (!res.ok) throw new Error("Ağ bağlantısı başarısız.");
                return res.text();
            })
            .then((html) => {
                const parser = new DOMParser();
                const nextDoc = parser.parseFromString(html, "text/html");

                // Swap title
                document.title = nextDoc.title;

                // Swap dynamic page wrapper inner content
                const nextWrapper = nextDoc.getElementById("page-wrapper");
                if (nextWrapper) {
                    wrapper.innerHTML = nextWrapper.innerHTML;
                } else {
                    const nextBody = nextDoc.querySelector("body");
                    if (nextBody) wrapper.innerHTML = nextBody.innerHTML;
                }

                // Push new history state
                if (pushState) {
                    window.history.pushState(null, "", url);
                }

                // Reset scroll to top immediately
                window.scrollTo({ top: 0, behavior: "instant" });

                // Synchronize and update auth UI elements
                updateAuthUI();

                // Re-bind dynamic Bootstrap modals
                rebindDynamicModals();

                // Initialize hover animations for the newly loaded page
                initMicroInteractions();

                // Re-bind Kirala button auth check
                initKiralaButtonAuth();

                // Phase 2: Fade in the new page wrapper smoothly
                setTimeout(() => {
                    wrapper.classList.add("loaded");
                }, 50);
            })
            .catch((err) => {
                console.warn("Soft transition failed, hard reloading: ", err);
                window.location.href = url;
            });
    }

    function rebindDynamicModals() {
        const modals = document.querySelectorAll(".modal");
        modals.forEach((modalEl) => {
            if (window.bootstrap && window.bootstrap.Modal) {
                const instance = window.bootstrap.Modal.getInstance(modalEl);
                if (instance) instance.dispose();
            }
        });
    }
}

/* ==========================================================
   3. PROFESSIONAL HOVER MICRO-INTERACTIONS
   ========================================================== */
function initMicroInteractions() {
    // Dynamic soft hover animations for cards
    const cards = document.querySelectorAll(".card, .item-card");
    cards.forEach((card) => {
        card.addEventListener("mouseenter", () => {
            card.style.transform = "translateY(-5px)";
        });
        card.addEventListener("mouseleave", () => {
            card.style.transform = "translateY(0px)";
        });
    });
}

/* ==========================================================
   4. KIRALA BUTTON AUTH CHECK & TOAST NOTIFICATION
   ========================================================== */
function initKiralaButtonAuth() {
    // Find all "Kirala" buttons (both layout and any page-level ones)
    const kiralaButtons = document.querySelectorAll('[data-bs-target="#mülkEkleModal"]');
    
    kiralaButtons.forEach(btn => {
        // Remove existing Bootstrap data-bs-toggle/target so we control it
        btn.removeAttribute('data-bs-toggle');
        btn.removeAttribute('data-bs-target');

        btn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';

            if (!isLoggedIn) {
                // Show toast notification
                showKiralaToast(
                    'Giriş Gerekli',
                    'Lütfen eşya kiralamak için giriş yapın veya kayıt olun.',
                    'warning'
                );
            } else {
                // User is logged in — open the modal programmatically
                const modalEl = document.getElementById('mülkEkleModal');
                if (modalEl && window.bootstrap) {
                    const modal = new bootstrap.Modal(modalEl);
                    modal.show();
                }
                // Add glow pulse to the button
                btn.classList.add('kirala-btn-active');
                setTimeout(() => btn.classList.remove('kirala-btn-active'), 3000);
            }
        });
    });
}

function showKiralaToast(title, message, type) {
    // Remove any existing toast
    const existingToast = document.querySelector('.kirala-toast');
    if (existingToast) existingToast.remove();

    // Determine icon based on type
    let iconClass = 'bi-info-circle-fill';
    if (type === 'warning') iconClass = 'bi-exclamation-triangle-fill';
    if (type === 'success') iconClass = 'bi-check-circle-fill';

    // Create toast element
    const toast = document.createElement('div');
    toast.className = 'kirala-toast';
    toast.innerHTML = `
        <i class="bi ${iconClass} toast-icon"></i>
        <div class="toast-body">
            <span class="toast-title">${title}</span>
            <span class="toast-msg">${message}</span>
        </div>
        <button class="toast-close" aria-label="Kapat">&times;</button>
        <div class="toast-progress"></div>
    `;

    document.body.appendChild(toast);

    // Trigger show animation
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            toast.classList.add('show');
        });
    });

    // Close button handler
    toast.querySelector('.toast-close').addEventListener('click', () => {
        toast.classList.remove('show');
        toast.classList.add('hide');
        setTimeout(() => toast.remove(), 500);
    });

    // Auto-dismiss after 3.5 seconds
    setTimeout(() => {
        if (toast.parentElement) {
            toast.classList.remove('show');
            toast.classList.add('hide');
            setTimeout(() => toast.remove(), 500);
        }
    }, 3500);
}
