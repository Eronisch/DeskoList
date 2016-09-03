var settingsUpdater = (function (settings) {

    startUp();

    //#region Public properties

    var CONST_ENUMUPDATEAVAILBLE = {
        NoUpdates: 0,
        SingleUpdate: 1,
        MultipleUpdates: 2
    }

    var CONST_UPDATESTATUS = {
        None: 0,
        Checking: 1,
        Downloading: 2,
        Installing: 3
    };

    //#endregion

    //#region User Interface Events

    $(settings.InstallButton).on('click', function () {
        startInstalling().done(updateChecking);
    });

    //#endregion

    //#region Methods

    function isUpdating() {
        return $(settings.InstallButton).prop('disabled');
    }

    function startUp() {
        if (settings.IsChecking === "True") {
            updateChecking().done(regularyCheckForUpdating());
        }
        else if (settings.IsDownloading === "True") {
            updateDownloading().done(regularyCheckForUpdating());
        }
        else if (settings.IsInstalling === "True") {
            updateInstalling().done(regularyCheckForUpdating());
        } else {
            regularyCheckForUpdating();
        }
    }

    function regularyCheckForUpdating() {
        if (!isUpdating()) {
            $.get(settings.UpdatingStatusLink).done(function (response) {
                switch (parseInt(response)) {
                    case CONST_UPDATESTATUS.Checking:
                        {
                            updateChecking();
                            break;
                        }
                    case CONST_UPDATESTATUS.Downloading:
                        {
                            updateDownloading();
                            break;
                        }
                    case CONST_UPDATESTATUS.Installing:
                        {
                            updateInstalling();
                            break;
                        }
                }
            });
        }
        setTimeout(regularyCheckForUpdating, 2500);
    }

    function animateUpdateProgressBars() {
        $(settings.AvailableUpdatesContainer).find('.progress-bar').addClass('active');
    }

    function updateChecking() {
        var dfd = $.Deferred();

        setTimeout(function () {

            setCheckingInterface();

            waitForChecking().done(function () {

                checkForSystemUpdates().done(function () {

                    setDownloadingInterface();

                    animateUpdateProgressBars();

                    var availableUpdateStatus = getUpdateAvailableStatus();

                    updateDownloadProgress().done(function () {

                        setInstallingInterface();

                        waitForInstalling().done(function () {

                            getUpdatingSuccess().done(function (isSuccess) {

                                viewUpdateInstalledAlert(availableUpdateStatus, isSuccess);

                                getAvailableUpdates();

                                unsetUpdatingInterface();

                                dfd.resolve();
                            });
                        });
                    });
                });
            });
        }, 0);

        return dfd.promise();
    }

    function updateDownloading() {

        var dfd = $.Deferred();

        setTimeout(function () {

            checkForSystemUpdates().done(function () {

                setDownloadingInterface();

                animateUpdateProgressBars();

                var availableUpdateStatus = getUpdateAvailableStatus();

                updateDownloadProgress().done(function () {

                    setInstallingInterface();

                    waitForInstalling().done(function () {

                        getUpdatingSuccess().done(function (isSuccess) {

                            viewUpdateInstalledAlert(availableUpdateStatus, isSuccess);

                            getAvailableUpdates();

                            unsetUpdatingInterface();

                            dfd.resolve();
                        });
                    });
                });
            });
        }, 0);

        return dfd.promise();
    }

    function updateInstalling() {

        var dfd = $.Deferred();

        setTimeout(function () {

            setInstallingInterface();

            var availableUpdateStatus = getUpdateAvailableStatus();

            waitForInstalling().done(function () {

                getUpdatingSuccess().done(function (isSuccess) {

                    viewUpdateInstalledAlert(availableUpdateStatus, isSuccess);

                    getAvailableUpdates();

                    unsetUpdatingInterface();

                    dfd.resolve();
                });
            });
        }, 0);

        return dfd.promise();
    }

    function waitForChecking(dfd) {

        if (dfd == null) {
            dfd = $.Deferred();
        }

        getIsCheckingUpdates().done(function (resultIsChecking) {
            if (resultIsChecking === "False") {
                return dfd.resolve();
            } else {
                setTimeout(function () {
                    waitForChecking(dfd);
                }, 250);
            }
        });

        return dfd.promise();
    }

    function waitForInstalling(dfd) {

        if (dfd == null) {
            dfd = $.Deferred();
        }

        getIsInstallingUpdates().done(function (resultIsUpdating) {
            if (resultIsUpdating === "False") {
                return dfd.resolve();
            } else {
                setTimeout(function () {
                    waitForInstalling(dfd);
                }, 500);
            }
        });

        return dfd.promise();
    }

    function getIsInstallingUpdates() {
        return $.get(settings.IsInstallingLink);
    }

    function getIsDownloadingUpdates() {
        return $.get(settings.IsDownloadingLink);
    }

    function getIsCheckingUpdates() {
        return $.get(settings.IsCheckingLink);
    }

    function displayAlert(htmlAlert) {
        var escapedAlertHolderHtml = $(settings.AlertContainer).html().replace(/"/g, "'").trim();
        if (escapedAlertHolderHtml !== htmlAlert) {
            $(settings.AlertContainer).fadeOut(function () {
                $(settings.AlertContainer).html(htmlAlert);
                $(settings.AlertContainer).fadeIn();
            });
        }
    }

    function checkForSystemUpdates() {

        var dfd = $.Deferred();

        $.when(getDateChecked(), getAvailableUpdates(), viewUpdatesAvailableAlert()).done(function () {
            dfd.resolve();
        });

        return dfd.promise();
    }

    function getDateChecked() {

        var dfd = $.Deferred();

        $.ajax(settings.GetDateCheckedLink).done(function (checkedDate) {
            updateDateLastChecked(checkedDate);
            return dfd.resolve();
        });

        return dfd.promise();
    }

    function getUpdateAvailableStatus() {

        var status;

        $.ajax({
            url: settings.GetAvailableUpdatesStatusLink,
            async: false
        }).done(function (reponseStatus) {
            status = reponseStatus;
        });

        return parseInt(status);
    }

    function setCheckingInterface() {
        $(settings.InstallButton).text(settings.CheckingForUpdatesText);
        $(settings.InstallButton).prop('disabled', true);
    }

    function setDownloadingInterface() {
        $(settings.InstallButton).text(settings.DownloadingUpdatesText);
        $(settings.InstallButton).prop('disabled', true);
    }

    function setInstallingInterface() {
        $(settings.InstallButton).text(settings.InstallingUpdatesText);
        $(settings.InstallButton).prop('disabled', true);
    }

    function unsetUpdatingInterface() {
        $(settings.InstallButton).text(settings.InstallUpdatesText);
        $(settings.InstallButton).prop('disabled', false);
    }

    function updateDateLastChecked(date) {
        $(settings.DateCheckedContainer).fadeOut(function () {
            $(settings.DateCheckedContainer).text(date);
            $(settings.DateCheckedContainer).fadeIn();
        });
    }

    function getUpdatingSuccess() {
        return $.ajax({
            url: settings.IsUpdatedSuccessfullyLink,
            dataType: "json"
        });
    }

    function viewUpdateInstalledAlert(updateAvailableStatus, isSuccess) {

        var htmlAlert;

        if (isSuccess) {
            switch (updateAvailableStatus) {
                case CONST_ENUMUPDATEAVAILBLE.SingleUpdate:
                    {
                        htmlAlert = settings.AlertSingleUpdateInstalledHtml;
                        break;
                    }
                default:
                    {
                        htmlAlert = settings.AlertMultipleUpdatesInstalledHtml;
                        break;
                    }
            }
        } else {
            htmlAlert = settings.AlertUpdatingFailedHtml;
        }

        displayAlert(htmlAlert);
    }

    function viewUpdatesAvailableAlert() {

        var dfd = $.Deferred();

        $.ajax(settings.GetAvailableUpdatesStatusLink).done(function (updateAvailableStatus) {

            var htmlAlert;

            switch (parseInt(updateAvailableStatus)) {
                case CONST_ENUMUPDATEAVAILBLE.NoUpdates:
                    {
                        htmlAlert = settings.AlertUpToDateHtml;
                        break;
                    }
                case CONST_ENUMUPDATEAVAILBLE.SingleUpdate:
                    {
                        htmlAlert = settings.AlertSingleUpdateAvailableHtml;
                        break;
                    }
                default:
                    {
                        htmlAlert = settings.AlertMultipleUpdatesAvailableHtml;
                        break;
                    }
            }

            displayAlert(htmlAlert);

            dfd.resolve();
        });

        return dfd.promise();
    }

    function getAvailableUpdates() {

        var dfd = $.Deferred();

        $.ajax(settings.GetAvailableUpdatesLink).done(function (html) {
            $(settings.AvailableUpdatesContainer).fadeOut('slow', function () {
                $(settings.AvailableUpdatesContainer).html(html);
                $(settings.AvailableUpdatesContainer).fadeIn('slow');
                dfd.resolve();
            });
        });

        return dfd.promise();
    }

    function startInstalling() {
        return $.ajax(settings.InstallUpdatesLink);
    }

    function getDownloadProgress() {

        var dfd = $.Deferred();

        $.getJSON(settings.GetDownloadProgressLink).done(function (jsonResult) {
            if (settings.IsThirdParty) {
                updateMultipleTablesDownloadProgress(jsonResult.downloads);
            } else {
                updateTableDownloadProgress($(settings.AvailableUpdatesContainer), jsonResult.downloads);
            }

            dfd.resolve(jsonResult["isDownloading"]);
        });

        return dfd.promise();
    }

    function updateDownloadProgress(dfd) {

        if (dfd == null) {
            dfd = $.Deferred();
        }

        getDownloadProgress().done(function (isDownloading) {
            if (isDownloading) {
                setTimeout(function () {
                    updateDownloadProgress(dfd);
                }, 250);
            } else {
                dfd.resolve();
            }
        });

        return dfd.promise();
    }

    function updateMultipleTablesDownloadProgress(jsonDownloads) {
        $(settings.AvailableUpdatesContainer).find('table').each(function () {
            var id = parseInt($(this).closest('.tab-pane').attr('id'));
            var table = $(this);

            if (jsonDownloads.length === 0) {
                updateTableDownloadProgress(table, jsonDownloads);
            } else {
                var downloadsTable = [];
                for (var counter = 0; jsonDownloads.length > counter; counter++) {
                    if (jsonDownloads[counter]['id'] === id) {
                        downloadsTable.push(jsonDownloads[counter]);
                    }
                };
                updateTableDownloadProgress(table, downloadsTable);
            }
        });
    }

    function updateTableDownloadProgress(table, jsonDownloads) {
        if (jsonDownloads.length === 0) {
            table.find('tbody > tr').each(function (index) {
                setProgressBarWidthAvailableUpdates(table, index, '100%');
                removeAnimationDownload(table, index);
            });
        } else {
            table.find('tbody > tr').each(function (index) {

                for (var counter = 0; jsonDownloads.length > counter; counter++) {

                    var updateVersion = $(this).find('#version').val();

                    // The update is still downloading, update the progress
                    if (jsonDownloads[counter]['version'] === updateVersion) {

                        setProgressBarWidthAvailableUpdates(table, index, jsonDownloads[counter]['progress'] + '%');

                        if (jsonDownloads[counter]['progress'] === 100) {
                            removeAnimationDownload(table, index);
                        }
                        // Check if the update still exists in the download list
                    } else
                        if (!isUpdateInDownloadList(jsonDownloads, updateVersion)) {
                            removeAnimationDownload(table, index);
                            setProgressBarWidthAvailableUpdates(table, index, '100%');
                        }
                }
            });
        }
    }

    function setProgressBarWidthAvailableUpdates(table, indexTr, width) {
        table.find('tbody > tr').eq(indexTr).find('.progress-bar').css('width', width);
    }

    function removeAnimationDownload(table, indexTr) {
        table.find('tbody > tr').eq(indexTr).find('.progress-bar').removeClass('active');
    }

    function isUpdateInDownloadList(jsonDownloads, version) {

        for (var counter = 0; jsonDownloads.length > counter; counter++) {
            if (jsonDownloads[counter]['version'] === version) {
                return true;
            }
        }
        return false;
    }

    function waitTillUpdatingIsComplete() {
        getIsInstallingUpdates().done(function (resultIsUpdating) {
            if (resultIsUpdating) {
                setTimeout(waitTillUpdatingIsComplete(), 1000);
            }
        });
    }

    function waitTillCheckingIsComplete() {
        getIsCheckingUpdates().done(function (resultIsChecking) {
            if (resultIsChecking) {
                setTimeout(waitTillCheckingIsComplete(), 1000);
            }
        });
    }

    //#endregion
});