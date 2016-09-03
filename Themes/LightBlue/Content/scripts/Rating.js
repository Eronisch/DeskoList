$(document).ready(function () {
    $('#ratingList.notRated li').on('click', function () {
        $('#rateForm').submit();
        $('#ratingList.notRated li').off('mouseenter');
        $('#ratingList.notRated').off('mouseleave');
    });

    $('#ratingList.notRated li').on('mouseenter', function () {
        $(this).nextAll().removeClass('starOn').addClass('starOff');
        $(this).prevAll().andSelf().removeClass('starOff').addClass('starOn');
        $(this).closest('#rating').find('#WebsiteRating').val($(this).index() + 1);
    });

    $('#ratingList.notRated').on('mouseleave', function () {
        $('#ratingList li').removeClass('starOn').addClass('starOff');
        $(this).closest('#rating').find('#WebsiteRating').val(5);
    });
});