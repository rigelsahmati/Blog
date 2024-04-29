
document.addEventListener("DOMContentLoaded", function () {
    const copyButtons = document.querySelectorAll(".copyLinkBtn");

    copyButtons.forEach((copyButton) => {
        const url = document.querySelector(`#postUrlId-${copyButton.dataset.urlid}`).href;
        copyButton.addEventListener("click", function () {
            copyToClipboard(url);
            copyButton.innerHTML = "Copied!";
            setTimeout(function () {
                copyButton.innerHTML = `<i class="fa-solid fa-share"></i> Share`;
            }, 1000);
        });
    });
});
function copyToClipboard(text) {
    const textarea = document.createElement("textarea");
    textarea.value = text;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand("copy");
    document.body.removeChild(textarea);
}
