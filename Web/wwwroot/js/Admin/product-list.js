/* ---------- Shared context helpers ---------- */
function getAjaxContext($el) {
    const $container = $el.closest('[data-ajax-container]');
    return {
        $container,
        reloadUrl: $container.data('reload-url'),
        editUrl: $container.data('edit-url'),
        productIdInput: $container.data('product-id-input')
    };
}

function reloadContainer($el) {
    const ctx = getAjaxContext($el);
    const productId = document.getElementById(ctx.productIdInput)?.value;
    ctx.$container.load(ctx.reloadUrl + "?id=" + productId, function () {
        $.validator.unobtrusive.parse(ctx.$container);
    });
}

function submitAjaxForm($form) {
    if ($form.data('submitting')) return;
    if (!$form.valid()) return;

    $form.data('submitting', true);

    $.ajax({
        type: 'POST',
        url: $form.data('url'),
        data: $form.serialize(),
        headers: {
            'RequestVerificationToken': $form.find('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            if (data.success) {
                showSuccessMessage(data.message);
                reloadContainer($form);
            } else {
                showErrorMessage(data.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error submitting form:", error);
        },
        complete: function () {
            $form.data('submitting', false);
        }
    });
}

/* ---------- Delegated handlers (registered once) ---------- */
$(document).ready(function () {

    // 1) Submit button — type="button" so nothing can natively submit
    $(document).on('click', '[data-ajax-submit]', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        submitAjaxForm($(this).closest('[data-ajax-form]'));
    });

    // Enter key inside the form
    $(document).on('keydown', '[data-ajax-form] input', function (e) {
        if (e.key !== 'Enter') return;
        e.preventDefault();
        $(this).closest('[data-ajax-form]').find('[data-ajax-submit]').trigger('click');
    });

    // 2) Edit button
    $(document).on('click', '[data-ajax-edit]', function (e) {
        e.preventDefault();
        const $el = $(this);
        const ctx = getAjaxContext($el);
        ctx.$container.load(ctx.editUrl + "?id=" + $el.data('id'), function () {
            $.validator.unobtrusive.parse(ctx.$container);
        });
    });

    // 3) Delete
    $(document).on('click', '[data-ajax-delete]', function (e) {
        e.preventDefault();
        const $form = $(this).closest('form');
        const ctx = getAjaxContext($form);

        Swal.fire({
            title: 'آیا از انجام این عملیات مطمئن هستید؟',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'آره، مطمئنم',
            cancelButtonText: 'نه، منصرف شدم',
        }).then(result => {
            if (!result.isConfirmed) return;

            fetch($form.attr('action'), {
                method: $form.attr('method'),
                body: new FormData($form[0])
            })
                .then(r => r.json())
                .then(data => {
                    if (data.success) {
                        reloadContainer($form);
                        showSuccessMessage(data.message);
                    } else {
                        showErrorMessage(data.message);
                    }
                })
                .catch(err => console.error('Deletion Error:', err));
        });
    });
});