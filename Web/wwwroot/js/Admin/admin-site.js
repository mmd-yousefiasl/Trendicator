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
        plugins: 'anchor autolink charmap codesample emoticons link lists media searchreplace table visualblocks wordcount',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        language_url: '/adminAssets/tinymce/js/tinymce/langs/fa.js',
        language: "fa",

        spellchecker_languages: "Arabic=ar,Azerbaijani=az,Bulgarian=bg,Bengali=bn,Bosnian=bs,Catalan=ca,Czech=cs,Welsh=cy,Danish=da,German=de,Greek=el,+English / British English=en,Esperanto=eo,Spanish / Argentinian Spanish / Mexican Spanish / Nicaraguan Spanish=es,Estonian=et,Basque=eu,Persian=fa,Finnish=fi,French=fr,Frisian=fy,Irish=ga,Galician=gl,Hebrew=he,Hindi=hi,Croatian=hr,Hungarian=hu,Indonesian=id,Icelandic=is,Italian=it,Japanese=ja,Georgian=ka,Kazakh=kk,Khmer=km,Kannada=kn,Korean=ko,Lithuanian=lt,Latvian=lv,Macedonian=mk,Malayalam=ml,Mongolian=mn,Norwegian Bokmal=nb,Nepali=ne,Dutch=nl,Norwegian Nynorsk=nn,Punjabi=pa,Polish=pl,Portuguese / Brazilian Portuguese=pt,Romanian=ro,Russian=ru,Slovak=sk,Slovenian=sl,Albanian=sq,Serbian / Serbian Latin=sr,Swedish=sv,Swahili=sw,Tamil=ta,Telugu=te,Thai=th,Turkish=tr,Tatar=tt,Ukrainian=uk,Urdu=ur,Vietnamese=vi,Simplified Chinese / Traditional Chinese=zh",
        directionality: "rtl",
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