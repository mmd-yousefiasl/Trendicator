function FillPageId(id) {
    $("#Page").val(id);

    searchForm();
}

function searchForm() {
    document.getElementById("search-form").submit();
}

function showCustomModal(title, url) {

    document.querySelector("#custom-modal-label").innerText = title;

    $("#custom-modal-body").load(url);

    $("#custom-modal").modal("show");
}

function ConfirmDelete(element) {
    Swal.fire({
        title: 'آیا از انجام این عملیات مطمئن هستید؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'آره، مطمئنم',
        cancelButtonText: 'نه، منصرف شدم',
    }).then((result) => {
        if (result.isConfirmed) {
            element.submit();
        }
    });
}

function submitAnimation() {
    var element = document.activeElement; // Get the button that triggered the form submission
    var spinner = document.createElement('div');
    const innerText = element.innerText;

    element.innerText = "";
    spinner.classList.add('spinner');
    element.appendChild(spinner);

    element.disabled = true;
    spinner.style.display = 'inline-block';

    setTimeout(function () {
        element.disabled = false;
        spinner.style.display = 'none';
        element.innerText = innerText;
    }, 3000);
}

function showSuccessMessage(message) {
    Swal.fire({
        toast: true,
        position: 'top-start',
        icon: 'success',
        title: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    })
}

function showErrorMessage(message) {
    Swal.fire({
        toast: true,
        position: 'top-start',
        icon: 'error',
        title: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    })
}

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".wishlist-toggle").forEach(button => {
        button.addEventListener("click", function () {
            const productId = this.getAttribute("data-product-id");
            const icon = this.querySelector("i");
            const label = this.querySelector("span");

            const isInWishlist = icon.classList.contains("fa");
            const action = isInWishlist ? "Remove" : "Add";

            // Update UI optimistically
            if (action === "Add") {
                icon.classList.replace("far", "fa");
                if (label) {
                    label.textContent = "حذف از علاقه مندی ها";
                }
            } else {
                icon.classList.replace("fa", "far");
                if (label) {
                    label.textContent = "افزودن به علاقه مندی ها";
                }
            }

            // Send POST request to server
            fetch("/product/toggle-wishlist", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    productId: parseInt(productId),
                    action: action
                })
            })
                .then(response => {
                    if (response.ok) {
                        if (action === "Add") {
                            showSuccessMessage("به لیست علاقه مندی ها اضافه شد");
                        }
                        else {
                            showSuccessMessage("از لیست علاقه مندی ها حذف شد");
                        }
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                });
        });
    });
});

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".compare-toggle").forEach(button => {
        button.addEventListener("click", function () {
            const productId = this.getAttribute("data-product-id");
            const label = this.querySelector("span");
            const currentTitle = this.getAttribute("title");

            const isInCompare = currentTitle?.includes("حذف");
            const action = isInCompare ? "Remove" : "Add";

            // Update UI optimistically
            if (action === "Add") {
                if (label) label.textContent = "حذف از لیست مقایسه";
                this.setAttribute("title", "حذف از لیست مقایسه");
            } else {
                if (label) label.textContent = "اضافه کردن به لیست مقایسه";
                this.setAttribute("title", "اضافه کردن به لیست مقایسه");

            }

            // Send POST request
            fetch("/product/toggle-compare", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    productId: parseInt(productId),
                    action: action
                })
            })
                .then(response => {
                    if (response.ok) {
                        if (action === "Add") {
                            showSuccessMessage("به لیست مقایسه اضافه شد");
                        }
                        else {
                            showSuccessMessage("از لیست مقایسه حذف شد");
                        }
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                });
        });
    });
});

function ShowQuickViewModel(url) {
    $("#Product_Quick_View_Model_Body").load(url);

    $("#Product_Quick_View_Model").modal("show");
}

let isWorking = false;

function AddToCart() {
    if (isWorking == false) {
        isWorking = true;
        toggleSubmitAnimation();

        let optionId = document.getElementById("Product-Option-Id").value;

        fetch('/add-to-cart/' + optionId)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showSuccessMessage(data.message);
                } else {
                    showErrorMessage(data.message);
                }

                isWorking = false;
                toggleSubmitAnimation()
            })
            .catch(error => {
                isWorking = false;
                toggleSubmitAnimation()
            });
    }
}

let spinnerActive = false; // Tracks spinner state
let spinnerRef = null;
let originalText = "";

function toggleSubmitAnimation() {
    const button = document.querySelector(".btn-submit");

    if (!spinnerActive) {
        // First call: start spinner
        spinnerActive = true;
        originalText = button.innerText;
        button.innerText = "";

        spinnerRef = document.createElement("div");
        spinnerRef.classList.add("spinner");
        spinnerRef.style.display = "inline-block";

        button.appendChild(spinnerRef);
        button.disabled = true;
    } else {
        // Second call: stop spinner
        spinnerActive = false;
        button.disabled = false;

        if (spinnerRef) {
            spinnerRef.style.display = "none";
            button.removeChild(spinnerRef);
            spinnerRef = null;
        }

        button.innerText = originalText;
    }
}

function LoadCartModal() {
    // Clear the Cart-Menu content
    $("#Cart-Menu").html("");

    // Then load the cart modal
    $("#Cart-Menu").load("/cart-modal");
}

function RemoveItemFromCart(itemId) {
    fetch('/remove-cart-modal-item/' + itemId)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                LoadCartModal();
            } else {
                showErrorMessage(data.message);
            }
        })
        .catch(error => {
        });
}