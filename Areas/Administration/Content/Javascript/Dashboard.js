var dashBoard = (function () {

    var userChart;
    var websiteChart;

    function init(labelUsers, labelWebsites) {
        drawUsers(labelUsers);
        drawWebsites(labelWebsites);
        getNews();
    }

    function drawUsers(label) {
        $.ajax({
            url: '/Administration/Dashboard/GetUsers'
        }).done(function (jsonData) {
            userChart = Morris.Area({
                element: 'usersChart',
                resize: false,
                data: jsonData,
                xkey: 'date',
                ykeys: ['users'],
                labels: [label],
                lineColors: ['#a0d0e0', '#3c8dbc'],
                hideHover: 'auto'
            });

            removeLoading('#usersChart');
        });
    }

    function removeLoading(element) {
        $(element).parent().find('.overlay').hide();
        $(element).parent().find('.loading-img').hide();
    }

    function drawWebsites(label) {
        $.ajax({
            url: '/Administration/Dashboard/GetWebsites'
        }).done(function (jsonData) {
            websiteChart = Morris.Area({
                element: 'websitesChart',
                resize: false,
                data: jsonData,
                xkey: 'date',
                ykeys: ['websites'],
                labels: [label],
                lineColors: ['#a0d0e0', '#3c8dbc'],
                hideHover: 'auto'
            });

            removeLoading('#websitesChart');
        });
    }

    function getNews() {
        $.ajax({
            url: '/Administration/Dashboard/GetNews'
        }).done(function (htmlData) {
            $('#news').html(htmlData);

            removeLoading('#news');
        });
    }

    $(window).resize(function() {
        if (userChart != null) {
            userChart.redraw();
        }
        if (websiteChart != null) {
            websiteChart.redraw();
        }
    });

    return {
        init : init
    };

})();