$(document).ready(function() {

    window.setBannerEvent = function (changeElement) {
        $(changeElement).on('change', function () {
            $('#bannerFile').toggle();
            $('#bannerUrl').toggle();
        });
    }
});