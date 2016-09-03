$(document).ready(function () {

    $(".order").ionRangeSlider({
        min: 1,
        max: 20,
        type: 'single',
        step: 1,
        prettify: false,
        hasGrid: false
    });


    $('#widgets .activate').click('click', function () {
        $(this).parent().find('.activateWidgetModal').modal('show');
        return false;
    });

    $('#widgets .delete').click('click', function () {
        $(this).parent().find('.deleteModal').modal('show');
        return false;
    });

    $('#widgets #installWidget').on('click', function () {
        $('#widgets #installWidgetModal').modal('show');
    });

    $('#installWidgetModal').on('hidden.bs.modal', function () {
        $('#WidgetInstallModel_File').val("");
    });

    $('.activateWidgetModal').on('hidden.bs.modal', function () {

        var indexWidget = $(this).closest('#list .col-md-3').index();

        var orderElement = $("#widgets #list .col-md-3").eq(indexWidget).find('.order');

        var rangeSlider = orderElement.data("ionRangeSlider");

        rangeSlider.reset();

        var themeSectionElement = $("#widgets #list .col-md-3").eq(indexWidget).find('#widget_ActivateWidget_ThemeSectionId');

        var orgValue = themeSectionElement.data("orgvalue");

        if (orgValue !== "-1") {
            $("#widgets #list .col-md-3").eq(indexWidget).find('#widget_ActivateWidget_ThemeSectionId').val(orgValue);
        }
    });

});