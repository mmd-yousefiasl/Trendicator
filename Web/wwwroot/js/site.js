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