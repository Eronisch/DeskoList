$(document).ready(function() {

    $('#modules .settings').on('click', function () {
        $(this).parent().find('.settingsModal').modal('show');
        return false;
    });

    $('#modules .order').ionRangeSlider({
        min: 1,
        max: 20,
        type: 'single',
        step: 1,
        prettify: false,
        hasGrid: false,
    });

    $('.settingsModal').on('hidden.bs.modal', function () {

        var indexModule = $(this).closest('.col-md-3').index();

        // Get the ion range slider
        var rangeSlider = $("#modules .col-md-3").eq(indexModule).find('.order').data("ionRangeSlider");

        // Reset its value
        rangeSlider.reset();

        // Get original value
        var orgValue = $("#modules .col-md-3").eq(indexModule).find('.themeSection').data('orgvalue');

        // Set org value back
        $("#modules .col-md-3").eq(indexModule).find('.themeSection').val(orgValue);
    });
});