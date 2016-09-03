$(document).ready(function() {
    var page;
    var categoryId;
    var searchText;
    var startIndex;
    replaceBrokenImages();

    // Load new websites
    $("#loadwebsites").click(function () {
        $(this).prop('disabled', true);
        page = document.getElementById('page').value;
        categoryId = document.getElementById('categoryID').value;
        searchText = document.getElementById('searchText').value;
        startIndex = document.getElementById('startIndex').value;

        var url = "/LoadWebsites?page=" + page + "&startIndex=" + startIndex;

        if (searchText != "" && categoryId != "") {
            url += "&search=" + searchText + "&categoryid=" + categoryId;
        } else if (searchText == "" && categoryId != "") {
            url += "&categoryid=" + categoryId;
        }
        $.get(url, function (data) {
            $('#newwebsites').append(data);

            if (data.indexOf('ranking') == -1) {
                $('#loadwebsites').parent().remove();
            }

            document.getElementById('page').value = parseInt(page) + 1;
            replaceBrokenImages();

            $(this).prop('disabled', false);
        });
    });


    // Replace broken images
    function replaceBrokenImages() {
        $(".bannerImage img").each(function() {
            $(this).error(function() {
                $(this).attr("src", "/Themes/LightBlue/Content/images/thumbnail.png");
            });
            // Reset for IE
            $(this).attr("src", $(this).attr("src"));
        });
    }

    $('body').on('click', '.stats', function() {
        $(this).closest(".ranking").find(".statsbox").toggle('slow');
    });

    $('body').on('click', '.flag', function() {
        $(this).closest('.ranking').find("#reportModal").modal('show');
    });

    $('body').on('click', '.preview', function () {
        $(this).closest('.ranking').find("#previewModal").modal('show');
    });

    // Code List click
    $('#codeWebsites li a').click(function() {
        $(this).parent().find('textarea').toggle('slow');
    });

    // Toggle statistics from website statistics page
    $('#statsWebsites .websiteName').click(function() {
        $(this).parent().find('.pageStatistics').toggle('slow');
    });

});