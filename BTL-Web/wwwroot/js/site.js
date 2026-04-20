// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

window.appEscapeHtml = function (value) {
	return String(value ?? "")
		.replace(/&/g, "&amp;")
		.replace(/</g, "&lt;")
		.replace(/>/g, "&gt;")
		.replace(/"/g, "&quot;")
		.replace(/'/g, "&#39;");
};

document.addEventListener("DOMContentLoaded", function () {
	var cookieConsentBanner = document.querySelector("[data-cookie-consent]");
	var cookieAcceptButton = document.querySelector("[data-cookie-accept]");
	var cookieDeclineButton = document.querySelector("[data-cookie-decline]");

	var setCookie = function (name, value, days) {
		var expires = "";

		if (typeof days === "number") {
			var date = new Date();
			date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
			expires = "; expires=" + date.toUTCString();
		}

		document.cookie = name + "=" + encodeURIComponent(value) + expires + "; path=/; SameSite=Lax";
	};

	var getCookie = function (name) {
		var prefix = name + "=";
		var cookies = document.cookie.split(";");

		for (var i = 0; i < cookies.length; i += 1) {
			var cookie = cookies[i].trim();
			if (cookie.indexOf(prefix) === 0) {
				return decodeURIComponent(cookie.substring(prefix.length));
			}
		}

		return "";
	};

	var hideCookieBanner = function () {
		if (!cookieConsentBanner) {
			return;
		}

		cookieConsentBanner.hidden = true;
	};

	if (cookieConsentBanner) {
		var consentValue = getCookie("btl_cookie_consent");
		if (!consentValue) {
			cookieConsentBanner.hidden = false;
		}

		if (cookieAcceptButton) {
			cookieAcceptButton.addEventListener("click", function () {
				setCookie("btl_cookie_consent", "accepted", 180);
				hideCookieBanner();
			});
		}

		if (cookieDeclineButton) {
			cookieDeclineButton.addEventListener("click", function () {
				setCookie("btl_cookie_consent", "declined", 180);
				hideCookieBanner();
			});
		}
	}

	var body = document.body;
	var sidebarToggles = document.querySelectorAll("[data-sidebar-toggle]");
	var sidebarBackdrop = document.querySelector("[data-sidebar-backdrop]");

	var openSidebar = function () {
		body.classList.add("sidebar-open");
	};

	var closeSidebar = function () {
		body.classList.remove("sidebar-open");
	};

	if (sidebarToggles.length > 0 && sidebarBackdrop) {
		sidebarToggles.forEach(function (toggleButton) {
			toggleButton.addEventListener("click", function () {
				if (body.classList.contains("sidebar-open")) {
					closeSidebar();
					return;
				}

				openSidebar();
			});
		});

		sidebarBackdrop.addEventListener("click", closeSidebar);

		document.addEventListener("keydown", function (event) {
			if (event.key === "Escape") {
				closeSidebar();
			}
		});

		window.addEventListener("resize", function () {
			if (window.innerWidth > 768) {
				closeSidebar();
			}
		});
	}

	if (typeof tinymce === "undefined") {
		return;
	}

	if (!document.querySelector("textarea.rich-editor")) {
		return;
	}

	tinymce.init({
		selector: "textarea.rich-editor",
		menubar: false,
		height: 260,
		plugins: "lists link table code",
		toolbar: "undo redo | bold italic underline | bullist numlist | link table | code",
		branding: false
	});
});
