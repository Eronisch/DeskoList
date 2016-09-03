var setupFooTable = {
    idElement: null,

    init: function (id, url, searchText, amountColumns) {
        setupFooTable.idElement = id;
        setupFooTable.loadData(url);
        setupFooTable.addSearch(searchText);
        setupFooTable.addDataAttributes();
        setupFooTable.addTableContent(amountColumns);
    },
    loadData: function (url) {
        $.ajax({
            url: url,
            success: function (jsonData) {
                setupFooTable.setData(jsonData);
            }
        });
    },
    setData: function (jsonData) {
        setupFooTable.insertResults(jsonData);
        setupFooTable.triggerInit();
    },
    addDataAttributes: function () {
        $(setupFooTable.idElement).attr({
            'data-filter': '#datatableText',
            'data-filter-minimum': '1',
            'data-limit-navigation': '10'
        });
    },
    triggerInit: function () {
        $(setupFooTable.idElement).footable();
        $(setupFooTable.idElement).trigger('footable_initialize');
    },
    addTableContent: function (amountColumns) {
        $('<tbody></tbody><tfoot><tr><td colspan=' + amountColumns + '><ul class="pagination pagination-centered"></ul></td></tr></tfoot>').insertAfter(setupFooTable.idElement + ' > thead');
    },
    addSearch: function (searchText) {
        $('<input class="form-control" id="datatableText" name="datatableText" placeholder="' +  searchText + '..." type="text" value="">').insertBefore(setupFooTable.idElement);
    },
    insertResults: function (jsonArray) {
        var htmlRows = null;

        for (var i = 0; i < jsonArray.length; i++) {
            var obj = jsonArray[i];
            htmlRows += '<tr>';
            for (var key in obj) {
                htmlRows += '<td>' + obj[key] + '</td>';
            }
            htmlRows += '</tr>';
        }
      
        $(setupFooTable.idElement + ' > tbody').append(htmlRows);
    }
};