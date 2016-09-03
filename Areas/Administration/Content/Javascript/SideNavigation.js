$(document).ready(function () {

    $('#searchText').on('keyup', function () {

        $('#sideBarNavigation').find('*').stop(true, true);

        var searchText = $('#searchText').val().toLowerCase();

        if (searchText.length === 0) {

            $('#sideBarNavigation > li ul').slideUp(300);

            $('#sideBarNavigation > li').slideDown(350);

            $('#sideBarNavigation > li > a i:last-of-type').each(function () {
                console.log('sdsds');
                if ($(this).hasClass('fa-chevron-up')) {
                    $(this).removeClass('fa-chevron-up').addClass('fa-chevron-down');
                }
            });
        } else {
            $('#sideBarNavigation li a').each(function () {

                if ($(this).find('span').text().toLowerCase().indexOf(searchText) > -1 || $(this).attr('href') != null && $(this).attr('href').toLowerCase().indexOf(searchText) > -1) {

                    var navigationItem = $(this).closest('li');
                    var navigationItemsBlock = navigationItem.closest('ul');

                    // Is child element?
                    if (navigationItemsBlock.length > 0 && navigationItemsBlock.attr('id') !== 'sideBarNavigation') {

                        console.log($(this).find('span').text());

                        // Shows the parent li
                        navigationItemsBlock.closest('li').slideDown(350);

                        // Shows the ul
                        navigationItemsBlock.slideDown(350);

                        navigationItemsBlock.closest('li').find('a').first().find('i').last().removeClass('fa-chevron-down').addClass('fa-chevron-up');
                    }

                    $(this).closest('li').slideDown(350);

                } else {
                    $(this).closest('li').slideUp(350);
                }
            });
        }
    });

    $('#sideBarNavigation > li > ul > li:first-child').each(function () {
        var parentLi = $(this).closest('ul').parent('li');
        $('<i class="fa fa-chevron-down pull-right"></i>').insertAfter(parentLi.find('> a span'));
        parentLi.find('ul').hide();
    });

    $('#sideBarNavigation li > a .fa-chevron-down, #sideBarNavigation li > a .fa-chevron-up').on('click', function (e) {
        if ($(this).hasClass('fa-chevron-down')) {
            $(this).removeClass('fa-chevron-down').addClass('fa-chevron-up');
            $(this).closest('li').find('ul').slideDown(300);
        } else {
            $(this).removeClass('fa-chevron-up').addClass('fa-chevron-down');
            $(this).closest('li').find('ul').slideUp(300);
        }

        e.preventDefault();
    });


});