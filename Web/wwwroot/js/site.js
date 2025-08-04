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