// Events
//------------------------------------------------------------------------------------------------------------------
document.addEventListener("DOMContentLoaded", function () {
    // Get all elements with class name "openModalBtn" (buttons that open the modals)
    var buttons = document.querySelectorAll(".openModalBtn");

    // Loop through each button and attach click event listeners
    buttons.forEach(function (btn) {
        btn.addEventListener("click", function () {
            var modalId = this.getAttribute("data-modal-id");
            var modal = document.getElementById(modalId);

            if (modal) {
                modal.style.display = "block";
            }
        });
    });

    // Get all elements with class name "close" (close buttons for modals)
    var closeButtons = document.querySelectorAll(".close");

    // Loop through each close button and attach click event listeners
    closeButtons.forEach(function (closeBtn) {
        closeBtn.addEventListener("click", function () {
            var modal = this.parentElement.parentElement;
            if (modal) {
                modal.style.display = "none";
            }
        });
    });

    // When the user clicks anywhere outside of the modal, close it
    window.addEventListener("click", function (event) {
        var modals = document.querySelectorAll(".modal");
        modals.forEach(function (modal) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        });
    });
});
//------------------------------------------------------------------------------------------------------------------