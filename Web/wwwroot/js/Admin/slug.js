const title = document.getElementById("title");
const slug = document.getElementById("slug");
let canUpdate = false;

if (title && slug) {
    title.addEventListener("focusin", () => {
        if (title.value == slug.value || slug.value == "") {
            canUpdate = true;
        }
        else {
            canUpdate = false;
        }
    });

    title.addEventListener("focusout", () => {
        if (canUpdate == true) {
            slug.value = title.value.replaceAll(" ", "-");
            canUpdate = false;
        }
    })

    slug.addEventListener("focusout", () => {
        slug.value = slug.value.replaceAll(" ", "-");
    })
}

