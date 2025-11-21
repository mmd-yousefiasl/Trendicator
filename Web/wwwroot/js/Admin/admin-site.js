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

function submitAnimation() {
    var element = document.querySelector(".btn-submit");
    var spinner = document.createElement('div');
    const innerText = element.innerText;

    element.innerText = "";
    // Add a spinner element to the button
    spinner.classList.add('spinner');

    element.appendChild(spinner);

    // Disable the button and start the animation
    element.disabled = true;
    spinner.style.display = 'inline-block';

    setTimeout(function () {
        // Enable the button and stop the animation after 3 seconds
        element.disabled = false;
        spinner.style.display = 'none';
        element.innerText = innerText;
    }, 3000);
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
        plugins: 'anchor autolink charmap codesample emoticons link lists media searchreplace table visualblocks wordcount image',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        language_url: '/adminAssets/tinymce/js/tinymce/langs/fa.js',
        language: "fa",
        directionality: "rtl",

        // Optional: allow inserting external images by URL
        image_title: true,
        automatic_uploads: false,
        file_picker_types: 'image',
        file_picker_callback: function (cb, value, meta) {
            if (meta.filetype === 'image') {
                // Prompt user for an image URL
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
    /* Below is a js demo | you don't need to use */
    setTimeout(function () {
        var elm = document.getElementsByTagName("input[data-jdp]")[0];
        elm.focus();
        jalaliDatepicker.hide();
        jalaliDatepicker.show(elm);
    }, 1000);
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

function handleMediaUpload() {
    const form = document.getElementById('imageForm');
    const formData = new FormData(form);

    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                ReloadMediaList();
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

function ReloadMediaList() {
    const page = document.getElementById("media-page").value;
    $("#LargeModalBody").load("/admin/media-list?page=" + page);
}

function remoMedia(formElement) {
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
                        ReloadMediaList(); // optionally pass productId if needed
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

    ReloadMediaList();
}