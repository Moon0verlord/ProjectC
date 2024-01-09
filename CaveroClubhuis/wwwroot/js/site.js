var acc = document.getElementsByClassName("Accordion");
    var i;

    for (i = 0; i < acc.length; i++) {
        acc[i].addEventListener("click", function () {
            var panel = this.nextElementSibling;
            var isActive = this.classList.contains("active");

            // Close all accordion panels except the clicked one
            for (var j = 0; j < acc.length; j++) {
                if (j !== i) {
                    acc[j].classList.remove("active");
                    acc[j].nextElementSibling.style.display = "none";
                }
            }

            // Toggle the clicked accordion panel
            if (isActive) {
                this.classList.remove("active");
                panel.style.display = "none";
            } else {
                this.classList.add("active");
                panel.style.display = "block";
            }
        });

        // Open the first accordion panel by default
        if (i === 0) {
            acc[i].classList.add("active");
            acc[i].nextElementSibling.style.display = "block";
        }
    }

document.addEventListener("DOMContentLoaded", function () {
    // Get all elements with class name "openModalBtn" (buttons that open the modals)
    var buttons = document.querySelectorAll(".openModalBtn, .AanmeldModalBtn, .AfmeldModalBtn");

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
            var modal = this.closest('.modal');
            if (modal) {
                modal.style.display = "none";

                // If the modal is closed, remove the specific class for "Deelnemers" modal
                if (modal.classList.contains("modal-deelnemers")) {
                    modal.classList.remove("modal-deelnemers");
                }
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
}
);