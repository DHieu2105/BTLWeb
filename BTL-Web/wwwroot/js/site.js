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
