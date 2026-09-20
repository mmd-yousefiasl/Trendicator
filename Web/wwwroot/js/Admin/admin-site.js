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

const imageInput = document.querySelector(".image-input");

if (imageInput) {
    imageInput.addEventListener('change', function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var previewImage = document.querySelector('.image-preview');
                previewImage.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
}

const imageInput2 = document.querySelector(".image-input2");

if (imageInput2) {
    imageInput2.addEventListener('change', function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var previewImage = document.querySelector('.image-preview2');
                previewImage.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
}

const imageInput3 = document.querySelector(".image-input3");

if (imageInput3) {
    imageInput3.addEventListener('change', function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var previewImage = document.querySelector('.image-preview3');
                previewImage.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
}

function FillPageId(id) {
    $("#Page").val(id);

    searchForm();
}

function searchForm() {
    document.getElementById("search-form").submit();
}

function showLargeModal(title, url) {

    document.querySelector("#LargeModalHeader").innerText = title;

    $("#LargeModalBody").load(url);

    $("#LargeModal").modal("show");
}

function showLargeModal2(title, url) {

    document.querySelector("#LargeModalHeader-2").innerText = title;

    $("#LargeModalBody-2").load(url);

    $("#LargeModal-2").modal("show");
}

$(document).ready(function () {
    $('.select2').select2();
});

document.addEventListener('DOMContentLoaded', function () {
    tinymce.init({
        selector: 'textarea.ck-editor',
        // Added 'code' to plugins
        plugins: 'anchor autolink charmap codesample emoticons link lists media searchreplace table visualblocks wordcount image code',
        // Added 'code' to the toolbar (placed it near undo/redo)
        toolbar: 'code undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',

        relative_urls: false,
        remove_script_host: false,

        language_url: '/lexaAssets/tinymce/js/tinymce/langs/fa.js',
        language: "fa",
        directionality: "rtl",

        image_title: true,
        automatic_uploads: false,
        file_picker_types: 'image',
        file_picker_callback: function (cb, value, meta) {
            if (meta.filetype === 'image') {
                var url = prompt('Enter image URL:');
                if (url) {
                    cb(url, { title: 'User provided image' });
                }
            }
        }
    });
});

$(document).ready(function () {
    jalaliDatepicker.startWatch({
        minDate: "attr",
        maxDate: "attr",
        time: false,
        // date:false
    });
});

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

function handleMediumUpload() {
    const form = document.getElementById('imageForm');
    const formData = new FormData(form);

    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                ReloadMedia();
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

function ReloadMedia() {
    const page = document.getElementById("media-page").value;
    $("#LargeModalBody").load("/admin/media-list?page=" + page);
}

function removeMedium(formElement) {
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
                        ReloadMedia(); // optionally pass productId if needed
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

function CopyUrlToClipboard(inputUrl) {
    const url = inputUrl;
    navigator.clipboard.writeText(url)
        .then(() => {
            showSuccessMessage("کپی شد!");
        })
        .catch(err => {
            console.error("خطا در کپی کردن:", err);
        });
}

function CustomFillPageId(id) {
    $("#media-page").val(id);

    ReloadMedia();
}

document.addEventListener('DOMContentLoaded', function () {
    const menu = document.querySelector('.admin-vertical-menu');
    const activeLink = menu?.querySelector('.active');

    if (menu && activeLink) {
        const scrollTop =
            activeLink.offsetTop -
            (menu.clientHeight / 2) +
            (activeLink.clientHeight / 2);

        menu.scrollTo({
            top: scrollTop,
            behavior: 'smooth'
        });
    }
});

function toggleRow(id) {
    var element = document.getElementById(id);
    if (!element) return;

    if (element.style.maxHeight === "0px" || element.style.maxHeight === "") {
        element.style.maxHeight = element.scrollHeight + "px";
    } else {
        element.style.maxHeight = "0px";
    }
}

function submitFormOnChange(form) {
    if (form) {
        form.submit();
    }
}

document.addEventListener('DOMContentLoaded', function () {

    // Listen for 'submit' on ALL forms on the page
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function (event) {

            // Find the submit button inside this specific form
            // This handles both <button> and <input type="submit">
            const button = form.querySelector('button[type="submit"], input[type="submit"], button:not([type])');

            if (!button) return;

            // If already loading, cancel the submission!
            if (button.classList.contains('is-loading')) {
                event.preventDefault();
                return false;
            }

            // Save original text (works for both <button> and <input>)
            const isInputTag = button.tagName === 'INPUT';
            const innerText = isInputTag ? button.value : button.innerText;

            // Apply loading state
            button.classList.add('is-loading');
            button.style.pointerEvents = 'none'; // Stops iOS double-taps

            if (isInputTag) {
                // <input type="submit"> can't have child elements like a spinner
                button.value = "⏳ Loading...";
            } else {
                // <button> can have a spinner
                button.innerText = "";
                const spinner = document.createElement('span');
                spinner.classList.add('spinner');
                spinner.style.display = 'inline-block';
                button.appendChild(spinner);
            }

            setTimeout(function () {
                button.classList.remove('is-loading');
                button.style.pointerEvents = 'auto';

                if (isInputTag) {
                    button.value = innerText;
                } else {
                    const existingSpinner = button.querySelector('.spinner');
                    if (existingSpinner) existingSpinner.remove();
                    button.innerText = innerText;
                }
            }, 3000);
        });
    });

});