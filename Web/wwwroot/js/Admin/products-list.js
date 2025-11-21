function handleImageUpload() {
    const form = document.getElementById('imageForm');
    const formData = new FormData(form);

    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                ReloadProductGallery();
                showSuccessMessage(data.message);
            } else {
                showErrorMessage(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showErrorMessage('An unexpected error occurred.');
        });
}

function removeImage(formElement) {
    event.preventDefault();

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
            const formData = new FormData(formElement);

            fetch(formElement.action, {
                method: formElement.method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        ReloadProductGallery(); // optionally pass productId if needed
                        showSuccessMessage(data.message);
                    } else {
                        showErrorMessage(data.message);
                    }
                })
                .catch(error => {
                    console.error('Deletion Error:', error);
                    showErrorMessage('An unexpected error occurred.');
                });
        }
    });
}

function ReloadProductGallery() {
    const productId = document.getElementById("ProductId").value;
    $("#LargeModalBody").load("/admin/product-gallery-list?id=" + productId);
}

function submitProductOptionForm(event) {
    event.preventDefault();

    const $form = $('#createOptionForm');

    // Trigger client-side validation
    if (!$form.valid()) {
        return;
    }

    $.ajax({
        type: 'POST',
        url: $form.attr('action'),
        data: $form.serialize(),
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            if (data.success) {

                showSuccessMessage(data.message);
                ReloadProductOption();

            } else {
                showErrorMessage(data.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error submitting form:", error);
        }
    });
}

function ReloadProductOption() {
    const productId = document.getElementById("ProductOptionProductId").value;
    $("#ProductOptions").load("/admin/add-product-option?id=" + productId);
}

function UpdateProductOption(stockId) {
    $("#ProductOptions").load("/admin/edit-product-option?id=" + stockId);
}

function RemoveProductOption(formElement) {
    event.preventDefault();

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
            const formData = new FormData(formElement);

            fetch(formElement.action, {
                method: formElement.method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        ReloadProductOption(); // optionally pass productId if needed
                        showSuccessMessage(data.message);
                    } else {
                        showErrorMessage(data.message);
                    }
                })
                .catch(error => {
                    console.error('Deletion Error:', error);
                });
        }
    });
}