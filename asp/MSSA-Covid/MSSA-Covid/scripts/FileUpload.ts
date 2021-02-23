
$(document).ready(function () {
    $('.custom-file-input').on("change", function () {
        var fileName = ($(this).val() as string).split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});