let loaderOverlay = document.getElementById("loading-anim");

function FillPageId(id) {
    $("#Page").val(id);

    searchForm();
}

function searchForm() {
    document.getElementById("search-form").submit();
}

async function showCustomModal(title, url) {
    loaderOverlay.classList.add("loader-overlay");
    document.querySelector("#custom-modal-label").innerText = title;

    try {
        const response = await fetch(url);
        const html = await response.text();
        document.querySelector("#custom-modal-body").innerHTML = html;

        $("#custom-modal").modal("show");
    } catch (err) {
        console.error("Failed to load modal content:", err);
    } finally {
        loaderOverlay.classList.remove("loader-overlay");
    }
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

            loaderOverlay.classList.add("loader-overlay");
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
                })
                .finally(() => {
                    // Code here will run regardless of success or error
                    loaderOverlay.classList.remove("loader-overlay");
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

            loaderOverlay.classList.add("loader-overlay");

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
                })
                .finally(() => {
                    loaderOverlay.classList.remove("loader-overlay");
                });
        });
    });
});

async function ShowQuickViewModel(url) {
    loaderOverlay.classList.add("loader-overlay");

    try {
        const response = await fetch(url);
        const html = await response.text();
        document.querySelector("#Product_Quick_View_Model_Body").innerHTML = html;

        $("#Product_Quick_View_Model").modal("show");
    } catch (err) {
        console.error("Quick view load failed:", err);
    } finally {
        loaderOverlay.classList.remove("loader-overlay");
    }
}

let isWorking = false;

async function AddToCart() {
    if (isWorking === false) {
        isWorking = true;
        toggleSubmitAnimation();
        loaderOverlay.classList.add("loader-overlay");

        try {
            let optionId = document.getElementById("Product-Option-Id").value;

            const response = await fetch('/add-to-cart/' + optionId);
            const data = await response.json();

            if (data.success) {
                showSuccessMessage(data.message);
            } else {
                showErrorMessage(data.message);
            }
        } catch (error) {
            console.error("Error adding to cart:", error);
        } finally {
            isWorking = false;
            toggleSubmitAnimation();
            loaderOverlay.classList.remove("loader-overlay");
        }
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

async function LoadCartModal() {
    loaderOverlay.classList.add("loader-overlay");

    try {
        const response = await fetch("/cart-modal");
        const html = await response.text();
        document.querySelector("#Cart-Menu").innerHTML = html;
    } catch (err) {
        console.error("Cart modal load failed:", err);
    } finally {
        loaderOverlay.classList.remove("loader-overlay");
    }
}

async function RemoveItemFromCart(itemId) {
    loaderOverlay.classList.add("loader-overlay");

    try {
        const response = await fetch('/remove-cart-modal-item/' + itemId);
        const data = await response.json();

        if (data.success) {
            LoadCartModal();
        } else {
            showErrorMessage(data.message);
        }
    } catch (error) {
        console.error("Error removing item:", error);
    } finally {
        loaderOverlay.classList.remove("loader-overlay");
    }
}

function updateProductPrice(selectElement) {
    if (!selectElement) return;

    const selected = selectElement.options[selectElement.selectedIndex];
    if (!selected) return;

    const orderPrice = selected.getAttribute("data-order-price-field");
    const price = selected.getAttribute("data-price-field");

    // Update main price field
    const priceField = document.getElementById("price-field");
    if (priceField) {
        priceField.textContent = orderPrice && orderPrice.trim() !== ""
            ? orderPrice
            : (price ?? "");
    }

    // Update original (struck-through) price field
    const orderPriceField = document.getElementById("order-price-field");
    if (orderPriceField) {
        if (orderPrice && orderPrice.trim() !== "") {
            orderPriceField.textContent = price ?? "";
        } else {
            orderPriceField.textContent = "";
        }
    }
}