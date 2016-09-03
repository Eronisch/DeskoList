$(document).ready(function () {

    var timeOutDatabase;
    var timeOutEmail;

    // Navigation
    $('#steps a').on('click', function (e) {

        $('#steps a').removeClass('active');

        $('form > div').hide();

        $(this).addClass('active');

        var step = $(this).attr('id');

        $('form > div').each(function () {
            if ($(this).data('step') === step) {
                $(this).show();
                return false;
            }
        });

        e.preventDefault();
    });

    $('#btnValidateEmailConnection').on('click', function () {

        $('#successNoReply').hide();
        $('#successReply').hide();
        $('#errorNoReply').hide();
        $('#errorReply').hide();

        if (timeOutEmail != null) {
            clearTimeout(timeOutEmail);
        }

        $(this).prop('disabled', true);

        $.ajax({
            url: '/AutoInstaller/ValidateEmailConnection',
            data: {
                noReplyHost: $('#NoReplyHost').val(),
                noReplyPort: $('#NoReplyPort').val(),
                replyHost: $('#ReplyHost').val(),
                replyPort: $('#ReplyPort').val()
            }
        }).done(function (data) {

            if (data.isNoReplyValid === true) {
                $('#successNoReply').fadeIn();
            } else {
                $('#errorNoReply').fadeIn();
            }

            if (data.isReplyValid === true) {
                $('#successReply').fadeIn();
            } else {
                $('#errorReply').fadeIn();
            }

            timeOutEmail = setTimeout(function () {
                if (data.isNoReplyValid === true) {
                    $('#successNoReply').fadeOut();
                } else {
                    $('#errorNoReply').fadeOut();
                }

                if (data.isReplyValid === true) {
                    $('#successReply').fadeOut();
                } else {
                    $('#errorReply').fadeOut();
                }
            }, 10000);

        }).always(function () {
            $('#btnValidateEmailConnection').prop('disabled', false);
        });
    });

    $('#btnValidateConnection').on('click', function () {

        $('#successDatabase').hide();
        $('#errorDatabase').hide();

        if (timeOutDatabase != null) {
            clearTimeout(timeOutDatabase);
        }

        $(this).prop('disabled', true);

        $.ajax({
            url: '/AutoInstaller/ValidateDatabaseConnection',
            data: {
                host: $('#DatabaseHost').val(),
                database: $('#DatabaseName').val(),
                username: $('#DatabaseUsername').val(),
                password: $('#DatabasePassword').val()
            }
        }).done(function (data) {

            if (data === "True") {
                $('#successDatabase').fadeIn();
            } else {
                $('#errorDatabase').fadeIn();
            }

            timeOutDatabase = setTimeout(function () {
                $('#successDatabase').fadeOut();
                $('#errorDatabase').fadeOut();
            }, 10000);

        }).always(function () {
            $('#btnValidateConnection').prop('disabled', false);
        });
    });

    $('.next').on('click', function () {

        var parent = $(this).parent();

        var step = parent.next().data('step');

        parent.hide();
        parent.next().show();

        $('#steps a').removeClass('active');

        $('#steps a').each(function () {
            if ($(this).attr('id') === step) {
                $(this).addClass('active');
                return false;
            }
        });
    });
});