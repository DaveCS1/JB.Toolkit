var _dataTables = [];

function registerAutoFilter(tableID, columnID, columnIndex) {
    for (let i = 0; i < _dataTables.length; i++) {
        if (_dataTables[i].tableID == 'tbl-' + tableID) {
            _dataTables[i].filterCols.push({
                tableID: 'tbl-' + tableID,
                columnIndex: columnIndex,
                dropdownID: 'tbl-' + tableID + '-' + columnID,
                anchorClassName: 'tbl-' + tableID + '-' + columnID + '-Filter',
                selectedAnchorClassName: 'tbl-' + tableID + '-' + columnID + '-SelectedFilter',
                current: ''
            });
            break;
        }
    }
}

// == BASIC FEED ==================================================================================================================

function registerFeed(tableID) {
    _dataTables.push({
        tableID: 'tbl-' + tableID,
        dataTable: null,
        filterCols: [],
        initialised: false,
        feed: true,
        isUsingJsonData: false,
        isUsingAjaxUrl: true,
        isEditor: false
    });
}

// == STANDARD DATATABLE WITH HTML ELEMENTS AS DATA SOURCE ========================================================================

function registerDataTable(
    tableID,
    defaultSortCol,
    defaultSortDirection,
    responsive) {

    var sortDir;
    if (defaultSortDirection == 0) {
        sortDir = 'asc';
    } else {
        sortDir = 'desc';
    }

    _dataTables.push({
        tableID: 'tbl-' + tableID,
        defaultSortCol: defaultSortCol,
        defaultSortDirection: sortDir,
        dataTable: null,
        filterCols: [],
        initialised: false,
        feed: false,
        isUsingJsonData: false,
        isUsingAjaxUrl: false,
        isEditor: false,
        responsive: responsive == undefined ? false : responsive === "False" ? false : true
    });
}

// == JSON DATA SOURCE ============================================================================================================

function registerJsonDataTable(
    tableID,
    sorting,
    columns,
    data,
    columnDefs,
    postScriptFunctionName,
    responsive) {

    _dataTables.push({
        tableID: 'tbl-' + tableID,
        dataTable: null,
        filterCols: [],
        columns: columns,
        columnDefs: columnDefs,
        data: data,
        sorting: sorting,
        initialised: false,
        feed: false,
        isUsingJsonData: true,
        isUsingAjaxUrl: false,
        isEditor: false,
        postScriptFunctionName: postScriptFunctionName,
        responsive: responsive == undefined ? false : responsive === "False" ? false : true
    });
}

// == AJAX (Url) DATASOURCE =======================================================================================================

function registerAjaxDataTable(
    tableID,
    sorting,
    columns,
    columnDefs,
    url,
    postScriptFunctionName,
    serverSide,
    responsive,
    cachedPipelinePages) {

    _dataTables.push({
        tableID: 'tbl-' + tableID,
        dataTable: null,
        filterCols: [],
        columns: columns,
        columnDefs: columnDefs,
        url: url,
        sorting: sorting,
        initialised: false,
        feed: false,
        isUsingJsonData: true,
        isUsingAjaxUrl: true,
        isEditor: false,
        postScriptFunctionName: postScriptFunctionName,
        serverSide: serverSide == undefined ? false : serverSide === "False" ? false : true,
        responsive: responsive == undefined ? false : responsive === "False" ? false : true,
        cachedPipelinePages: cachedPipelinePages == undefined ? 1 : cachedPipelinePages,

    });
}

// == 'EDITOR' AJAX (Url) DATASOURCE ================================================================================================

function registerEditorDataTable(
    tableID,
    sorting,
    columns,
    fields,
    enableInlineEditing,
    enableBubbleEditing,
    disableActionColumn,
    columnDefs,
    url,
    postScriptFunctionName,
    responsive) {

    _dataTables.push({
        tableID: 'tbl-' + tableID,
        dataTable: null,
        filterCols: [],
        columns: columns,
        columnDefs: columnDefs,
        fields: fields,
        url: url,
        sorting: sorting,
        initialised: false,
        feed: false,
        isUsingJsonData: true,
        isUsingAjaxUrl: true,
        isEditor: true,
        editor: null,
        enableInlineEditing: enableInlineEditing,
        enableBubbleEditing: enableBubbleEditing,
        disableActionColumn: disableActionColumn,
        postScriptFunctionName: postScriptFunctionName,
        responsive: responsive == undefined ? false : responsive === "False" ? false : true
    });
}

// used for table state saving
var CURRENT_USER_ID = 0;

// To differentiate between different projects - As states are stored in the database
var DATATABLE_STATE_PREFIX = "CP_";

function initDataTable(item) {
    if (!item.initialised) {

        // == FEED =================================================================================================================

        if (item.feed) {
            item.dataTable = $('#' + item.tableID).DataTable({
                customInitObj: item,
                searching: true,
                destroy: true,
                dom: 'Brtip',
                ordering: false,
                paging: false,
                bInfo: false,
                initComplete: function (settings, json) {
                    $('#' + item.tableID + '_wrapper .dt-buttons').css("left", -10000);
                    $('#' + item.tableID + '_wrapper .dt-buttons').css("position", "absolute");
                }
            });
        } else if (!item.isUsingJsonData) {

            // allow html tags           
            try {
                $('#' + item.tableID).html($('#' + item.tableID).html().replace(/&lt;/g, '<').replace(/&gt;/g, '>'));
            } catch (err) { }

            // == HTML DATATABLE =========================================================================================================

            item.dataTable = $('#' + item.tableID).DataTable({
                customInitObj: item,
                searching: true,
                destroy: true,
                colReorder: true,
                stateSave: true,
                responsive: item.responsive,
                stateSaveCallback: function (settings, s_data) {
                    try {

                        s_data.search.search = ""; // remove search filter, we don't want to use that!
                        s_data.start = 0; // Don't save pagination position

                        var jsonData = {};

                        jsonData.tableID = DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID;
                        jsonData.tableSettings = JSON.stringify(s_data);

                        var json = JSON.stringify(jsonData);
                        var xhttp = new XMLHttpRequest();

                        xhttp.open("POST", "/api/DataTableStates", true);
                        xhttp.send(json);
                    } catch (err) { }
                },
                stateLoadCallback: function (settings) {
                    var s_data = null;

                    try {
                        var xhttp = new XMLHttpRequest();
                        xhttp.onreadystatechange = function () {
                            if (this.readyState == 4) {
                                if (this.status == 200) {

                                    try {
                                        s_data = JSON.parse(xhttp.responseText);
                                    } catch (err2) { }
                                }
                            }
                        };

                        xhttp.open("GET", "/api/DataTableStates/" + DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID + "/" + "?v=" + new Date().getTime(), false);
                        xhttp.send();
                    } catch (err) { }

                    return s_data;
                },
                dom: 'Brtip',
                deferRender: true,
                async: true,
                buttons: [
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        orientation: 'landscape'

                    },
                    {
                        extend: 'csvHtml5',
                        orientation: 'landscape'

                    },
                    {
                        extend: 'pdfHtml5',
                        orientation: 'landscape'

                    },
                    {
                        extend: 'print',
                        orientation: 'landscape',
                        exportOptions: {
                            columns: ':visible'
                        },
                        customize: function (win) {

                            var last = null;
                            var current = null;
                            var bod = [];

                            var css = '@page { size: landscape; }',
                                head = win.document.head || win.document.getElementsByTagName('head')[0],
                                style = win.document.createElement('style');

                            style.type = 'text/css';
                            style.media = 'print';

                            if (style.styleSheet) {
                                style.styleSheet.cssText = css;
                            } else {
                                style.appendChild(win.document.createTextNode(css));
                            }

                            head.appendChild(style);
                        }
                    },
                    {
                        extend: 'colvis',
                        background: false
                    },
                    {
                        extend: 'colvisGroup',
                        text: 'Show All',
                        show: ['*']
                    }
                ],
                order: [
                    [item.defaultSortCol, item.defaultSortDirection]
                ],
                initComplete: function (settings, json) {
                    let filterCols = settings.oInit.customInitObj.filterCols;

                    if (filterCols != null && filterCols != undefined) {
                        for (let i = 0; i < filterCols.length; i++) {
                            let filterCol = filterCols[i];

                            let column = this.api().column(filterCol.columnIndex);
                            let dropdown = $('#' + filterCol.dropdownID);

                            column.data().unique().sort().each(function (d, j) {
                                if (d != null && d != "") {
                                    dropdown.append('<li><a class="' + filterCol.anchorClassName + '" href="#" data="' + d + '"> ' + d + '</a></li>');
                                    $('.' + filterCol.anchorClassName).click(function () {
                                        $('.' + filterCol.selectedAnchorClassName).remove();
                                        $(this).prepend('<i class="fa fa-check ' + filterCol.selectedAnchorClassName + '" aria-hidden="true"></i>');

                                        var filter = $(this).attr('data');
                                        filterCol.current = filter;
                                        applyFilters();
                                    })
                                }
                            });
                        }
                    }
                }
            });

            $(window).resize(function () {
                item.dataTable.columns.adjust();
            });

            $(window).blur(function () {
                item.dataTable.columns.adjust();
            });

            $("input").blur(function () {
                item.dataTable.columns.adjust();
            });

        } else if (item.isUsingJsonData) {
            if (!item.isUsingAjaxUrl) {

                // == JSON DATATABLE =========================================================================================================

                item.dataTable = $('#' + item.tableID).DataTable({
                    customInitObj: item,
                    searching: true,
                    destroy: true,
                    colReorder: true,
                    stateSave: true,
                    responsive: item.responsive,
                    columns: JSON.parse(item.columns),
                    columnDefs: item.columnDefs === "" ? null : JSON.parse(item.columnDefs),
                    data: JSON.parse(item.data),
                    drawCallback: function (settings) {
                        try {
                            window[item.postScriptFunctionName](item.tableID);
                        } catch (err) { }

                    },
                    stateSaveCallback: function (settings, s_data) {
                        try {

                            s_data.search.search = ""; // remove search filter, we don't want to use that!
                            s_data.start = 0; // Don't save pagination position

                            var jsonData = {};

                            jsonData.tableID = DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID;
                            jsonData.tableSettings = JSON.stringify(s_data);

                            var json = JSON.stringify(jsonData);
                            var xhttp = new XMLHttpRequest();

                            xhttp.open("POST", "/api/DataTableStates", true);
                            xhttp.send(json);
                        } catch (err) { }
                    },
                    stateLoadCallback: function (settings) {
                        var s_data = null;

                        try {
                            var xhttp = new XMLHttpRequest();
                            xhttp.onreadystatechange = function () {
                                if (this.readyState == 4) {
                                    if (this.status == 200) {

                                        try {
                                            s_data = JSON.parse(xhttp.responseText);
                                        } catch (err2) { }
                                    }
                                }
                            };

                            xhttp.open("GET", "/api/DataTableStates/" + DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID + "/" + "?v=" + new Date().getTime(), false);
                            xhttp.send();
                        } catch (err) { }

                        return s_data;
                    },
                    dom: 'Brtip',
                    deferRender: true,
                    async: true,
                    buttons: [
                        'copyHtml5',
                        {
                            extend: 'excelHtml5',
                            orientation: 'landscape'

                        },
                        {
                            extend: 'csvHtml5',
                            orientation: 'landscape'

                        },
                        {
                            extend: 'pdfHtml5',
                            orientation: 'landscape'

                        },
                        {
                            extend: 'print',
                            orientation: 'landscape',
                            exportOptions: {
                                columns: ':visible'
                            },
                            customize: function (win) {

                                var last = null;
                                var current = null;
                                var bod = [];

                                var css = '@page { size: landscape; }',
                                    head = win.document.head || win.document.getElementsByTagName('head')[0],
                                    style = win.document.createElement('style');

                                style.type = 'text/css';
                                style.media = 'print';

                                if (style.styleSheet) {
                                    style.styleSheet.cssText = css;
                                } else {
                                    style.appendChild(win.document.createTextNode(css));
                                }

                                head.appendChild(style);
                            }
                        },
                        {
                            extend: 'colvis',
                            background: false
                        },
                        {
                            extend: 'colvisGroup',
                            text: 'Show All',
                            show: ['*']
                        }
                    ],
                    order: JSON.parse(item.sorting),
                    initComplete: function (settings, json) {
                        let filterCols = settings.oInit.customInitObj.filterCols;

                        if (filterCols != null && filterCols != undefined) {
                            for (let i = 0; i < filterCols.length; i++) {
                                let filterCol = filterCols[i];

                                let column = this.api().column(filterCol.columnIndex);
                                let dropdown = $('#' + filterCol.dropdownID);

                                column.data().unique().sort().each(function (d, j) {
                                    if (d != null && d != "") {
                                        dropdown.append('<li><a class="' + filterCol.anchorClassName + '" href="#" data="' + d + '"> ' + d + '</a></li>');
                                        $('.' + filterCol.anchorClassName).click(function () {
                                            $('.' + filterCol.selectedAnchorClassName).remove();
                                            $(this).prepend('<i class="fa fa-check ' + filterCol.selectedAnchorClassName + '" aria-hidden="true"></i>');

                                            var filter = $(this).attr('data');
                                            filterCol.current = filter;
                                            applyFilters();
                                        })
                                    }
                                });
                            }
                        }
                    }
                });

                $(window).resize(function () {
                    item.dataTable.columns.adjust();
                });

                $(window).blur(function () {
                    item.dataTable.columns.adjust();
                });

                $("input").blur(function () {
                    item.dataTable.columns.adjust();
                });

            } else if (item.isUsingAjaxUrl) {
                if (!item.isEditor) {

                    // == AJAX DATATABLE =========================================================================================================

                    var ajaxInitialised = false;

                    $.fn.dataTable.pipeline = function (opts) {

                        // Pages caching pipeline

                        var conf = $.extend({
                            pages: opts.pages,
                            type: opts.type,
                            url: opts.url,
                            dataType: opts.dataType,
                            contentType: opts.contentType,
                            data: opts.data
                        }, opts);

                        var cacheLower = -1;
                        var cacheUpper = null;
                        var cacheLastRequest = null;
                        var cacheLastJson = null;

                        return function (request, drawCallback, settings) {
                            var ajax = false;
                            var requestStart = request.start;
                            var drawStart = request.start;
                            var requestLength = request.length;
                            var requestEnd = requestStart + requestLength;

                            if (settings.clearCache) {
                                ajax = true;
                                settings.clearCache = false;
                            }
                            else if (cacheLower < 0 || requestStart < cacheLower || requestEnd > cacheUpper) {
                                // outside cached data - need to make a request
                                ajax = true;
                            }
                            else if (JSON.stringify(request.order) !== JSON.stringify(cacheLastRequest.order) ||
                                JSON.stringify(request.columns) !== JSON.stringify(cacheLastRequest.columns) ||
                                JSON.stringify(request.search) !== JSON.stringify(cacheLastRequest.search)
                            ) {
                                // properties changed (ordering, columns, searching)
                                ajax = true;
                            }

                            // Store the request for checking next time around
                            cacheLastRequest = $.extend(true, {}, request);

                            if (ajax) {
                                // Need data from the server
                                if (requestStart < cacheLower) {
                                    requestStart = requestStart - (requestLength * (conf.pages - 1));

                                    if (requestStart < 0) {
                                        requestStart = 0;
                                    }
                                }

                                cacheLower = requestStart;
                                cacheUpper = requestStart + (requestLength * conf.pages);

                                request.start = requestStart;
                                request.length = requestLength * conf.pages;

                                if (typeof conf.data === 'function') {
                                    var d = conf.data(request);
                                    if (d) {
                                        $.extend(request, d);
                                    }
                                }
                                else if ($.isPlainObject(conf.data)) {
                                    $.extend(request, conf.data);
                                }

                                return $.ajax({
                                    type: conf.type,
                                    url: conf.url,
                                    data: JSON.stringify({ parameters: request }),
                                    dataType: conf.dataType,
                                    contentType: conf.contentType,
                                    cache: false,
                                    success: function (json) {
                                        cacheLastJson = $.extend(true, {}, json);
                                        if (cacheLower != drawStart) {
                                            json.data.splice(0, drawStart - cacheLower);
                                        }
                                        if (requestLength >= -1) {
                                            json.data.splice(requestLength, json.data.length);
                                        }

                                        drawCallback(json);
                                    },
                                    beforeSend: function () {
                                        $("#dtOverlay").show();
                                        if (!ajaxInitialised) {
                                            $('#' + item.tableID + ' > tbody').html(
                                                '<tr class="odd">' +
                                                '<td valign="top" colspan="' + $('#' + item.tableID).find("tr:first th").length +
                                                '" class="dataTables_empty">Loading&hellip;</td>' +
                                                '</tr>'
                                            );

                                            ajaxInitialised = true;
                                        }
                                    }
                                });
                            }
                            else {
                                json = $.extend(true, {}, cacheLastJson);
                                json.draw = request.draw; // Update the echo for each response
                                json.data.splice(0, requestStart - cacheLower);
                                json.data.splice(requestLength, json.data.length);

                                drawCallback(json);
                            }
                        }
                    };

                    $.fn.dataTable.Api.register('clearPipeline()', function () {
                        return this.iterator('table', function (settings) {
                            settings.clearCache = true;
                        });
                    });

                    item.dataTable = $('#' + item.tableID).DataTable({
                        customInitObj: item,
                        ajax: $.fn.dataTable.pipeline({
                            pages: item.cachedPipelinePages,
                            url: item.url,
                            type: 'POST',
                            dataType: "json",
                            contentType: 'application/json',
                            data: function (d) {
                                return d;
                            }
                        }),
                        drawCallback: function (settings) {
                            $("#dtOverlay").hide();

                            setTimeout(function () {
                                try {
                                    item.dataTable.columns.adjust();
                                }
                                catch (e) { }
                            }, 50);

                            try {
                                window[item.postScriptFunctionName](item.tableID);
                            } catch (err) { }
                        },
                        processing: true,
                        searching: true,
                        serverSide: item.serverSide,
                        responsive: item.responsive,
                        searchDelay: 1000, // May want to play around with this one
                        destroy: true,
                        colReorder: true,
                        stateSave: true,
                        columns: JSON.parse(item.columns),
                        columnDefs: item.columnDefs === "" ? null : JSON.parse(item.columnDefs),
                        stateSaveCallback: function (settings, s_data) {
                            try {

                                s_data.search.search = ""; // remove search filter, we don't want to use that!
                                s_data.start = 0; // Don't save pagination position

                                var jsonData = {};

                                jsonData.tableID = DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID;
                                jsonData.tableSettings = JSON.stringify(s_data);

                                var json = JSON.stringify(jsonData);
                                var xhttp = new XMLHttpRequest();

                                xhttp.open("POST", "/api/DataTableStates", true);
                                xhttp.send(json);
                            } catch (err) { }
                        },
                        stateLoadCallback: function (settings) {
                            var s_data = null;

                            try {
                                var xhttp = new XMLHttpRequest();
                                xhttp.onreadystatechange = function () {
                                    if (this.readyState == 4) {
                                        if (this.status == 200) {

                                            try {
                                                s_data = JSON.parse(xhttp.responseText);
                                            } catch (err2) { }
                                        }
                                    }
                                };

                                xhttp.open("GET", "/api/DataTableStates/" + DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID + "/" + "?v=" + new Date().getTime(), false);
                                xhttp.send();
                            } catch (err) { }

                            return s_data;
                        },
                        dom: 'Brtip',
                        deferRender: true,
                        async: true,
                        buttons: [
                            'copyHtml5',
                            {
                                extend: 'excelHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'csvHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'pdfHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'print',
                                orientation: 'landscape',
                                exportOptions: {
                                    columns: ':visible'
                                },
                                customize: function (win) {

                                    var last = null;
                                    var current = null;
                                    var bod = [];

                                    var css = '@page { size: landscape; }',
                                        head = win.document.head || win.document.getElementsByTagName('head')[0],
                                        style = win.document.createElement('style');

                                    style.type = 'text/css';
                                    style.media = 'print';

                                    if (style.styleSheet) {
                                        style.styleSheet.cssText = css;
                                    } else {
                                        style.appendChild(win.document.createTextNode(css));
                                    }

                                    head.appendChild(style);
                                }
                            },
                            {
                                extend: 'colvis',
                                background: false
                            },
                            {
                                extend: 'colvisGroup',
                                text: 'Show All',
                                show: ['*']
                            }
                        ],
                        order: JSON.parse(item.sorting),
                        initComplete: function (settings, json) {
                            $("#dtOverlay").hide();

                            let filterCols = settings.oInit.customInitObj.filterCols;
                            if (filterCols != null && filterCols != undefined) {
                                for (let i = 0; i < filterCols.length; i++) {
                                    let filterCol = filterCols[i];

                                    let column = this.api().column(filterCol.columnIndex);
                                    let dropdown = $('#' + filterCol.dropdownID);

                                    column.data().unique().sort().each(function (d, j) {
                                        if (d != null && d != "") {
                                            dropdown.append('<li><a class="' + filterCol.anchorClassName + '" href="#" data="' + d + '"> ' + d + '</a></li>');
                                            $('.' + filterCol.anchorClassName).click(function () {
                                                $('.' + filterCol.selectedAnchorClassName).remove();
                                                $(this).prepend('<i class="fa fa-check ' + filterCol.selectedAnchorClassName + '" aria-hidden="true"></i>');

                                                var filter = $(this).attr('data');
                                                filterCol.current = filter;
                                                applyFilters();
                                            })
                                        }
                                    });
                                }
                            }

                            item.dataTable.columns.adjust();
                        }
                    });

                    $(window).resize(function () {
                        item.dataTable.columns.adjust();
                    });

                    $(window).blur(function () {
                        item.dataTable.columns.adjust();
                    });

                    $("input").blur(function () {
                        item.dataTable.columns.adjust();
                    });

                } else if (item.isEditor) {

                    // == EDITOR DATATABLE =========================================================================================================

                    // DATATABLE COLUMNS

                    var jsColumns = JSON.parse(item.columns);

                    for (var i = 0; i < jsColumns.length; i++) {

                        if (jsColumns[i].display) {

                            var parts = jsColumns[i].display.split(".");
                            jsColumns[i].render = function (data, type, row) {

                                if (parts.length > 1) {
                                    var obj = row[parts[0]];
                                    return obj[parts[1]];
                                } else {
                                    var obj = row[parts[0]];
                                    return obj;
                                }
                            }
                        }

                        if (jsColumns[i].type) {

                            var datatype = jsColumns[i].type;

                            if (datatype == "boolean" || datatype == "checkbox") {
                                jsColumns[i].render = function (data, type, row) {

                                    var guid = GenerateGUID();

                                    if (data == "True")
                                        return "<center><div class='checkbox checkbox-primary'><input id='" + guid + "' class='styled' type='checkbox' checked style='margin-left: -5px;' onclick='return false;'><label for='" + guid + "'></label></div></center>";
                                    else
                                        return "<center><div class='checkbox checkbox-primary'><input id='" + guid + "' class='styled' type='checkbox' style='margin-left: -5px;' onclick='return false;'><label for='" + guid + "'></label></div></center>";
                                }
                            } else if (datatype == "money") {
                                jsColumns[i].render = $.fn.dataTable.render.number(',', '.', 0, '£');
                            }
                        }
                    }

                    // EDITOR FIELDS

                    var jsFields = JSON.parse(item.fields);

                    for (var i = 0; i < jsFields.length; i++) {

                        if (jsFields[i].type) {
                            if (jsFields[i].type == "datetime" || jsFields[i].type == "date") {
                                jsFields[i].type = 'date';
                                jsFields[i].def = function () {
                                    return new Date();
                                };
                                jsFields[i].dateFormat = 'm/d/yy';
                            }
                        }
                    }

                    if (item.disableActionColumn) {
                        if (item.disableActionColumn == "False") {
                            var editColumn = {};
                            onclick = "return false;"
                            editColumn.data = null;
                            editColumn.defaultContent = '<a href="#" class="edit"><span class="glyphicon glyphicon-edit"></span></a>&nbsp;&nbsp;<a href="#" class="remove"><span class="glyphicon glyphicon-remove" style="color: red;"></span></a>';
                            editColumn.orderable = false;
                            editColumn.class = "dt-action-cell";

                            jsColumns.push(editColumn);
                        }
                    }

                    item.editor = new $.fn.dataTable.Editor({
                        ajax: item.url,
                        table: "#" + item.tableID,
                        fields: jsFields,
                        display: "bootstrap",
                        formOptions: {
                            bubble: {
                                title: 'Edit',
                                buttons: false
                            }
                        },
                    });

                    item.editor
                        .on('onSubmitSuccess', function (e, json, data) {

                            // placeholder

                        });

                    if (item.enableBubbleEditing == "True") {

                        $("#" + item.tableID).on('click', 'tbody td', function (e) {
                            if ($(this).index() < 6) {
                                item.editor.bubble(this);
                            }
                        });
                    }

                    $("#" + item.tableID).on('click', 'a.remove', function (e) {
                        $("#" + item.tableID + "_wrapper .buttons-remove").click();
                    });

                    $("#" + item.tableID).on('click', 'a.edit', function (e) {
                        $("#" + item.tableID + "_wrapper .buttons-edit").click();
                    });

                    if (item.enableInlineEditing == "True") {

                        // Only enable double click in-lin editing

                        var DELAY = 500,
                            clicks = 0,
                            timer = null;

                        // Activate an inline edit on click of a table cell
                        $("#" + item.tableID).on('click', 'tbody td', function (e) {
                            clicks++;

                            if (clicks === 1) {
                                timer = setTimeout(function () {
                                    clicks = 0;
                                }, DELAY);

                            } else {
                                clicks = 0;
                                item.editor.inline(this);

                                item.editor.inline(this, {
                                    onBlur: 'submit'
                                });
                            }
                        });

                        $("#" + item.tableID).on("dblclick", function (e) {
                            e.preventDefault(); //cancel system double-click event
                        });
                    }

                    item.dataTable = $('#' + item.tableID).DataTable({
                        customInitObj: item,
                        processing: true,
                        ajax: item.url,
                        searching: true,
                        responsive: item.responsive,
                        destroy: true,
                        colReorder: true,
                        keys: {
                            editor: item.editor
                        },
                        autoFill: {
                            editor: item.editor
                        },
                        select: {
                            style: 'os',
                            blurable: true,
                            style: 'single'
                        },
                        stateSave: true,
                        columns: jsColumns,
                        columnDefs: item.columnDefs === "" ? null : JSON.parse(item.columnDefs),
                        drawCallback: function (settings) {
                            setTimeout(function () {
                                try {
                                    item.dataTable.columns.adjust();
                                }
                                catch (e) { }
                            }, 50);

                            try {
                                window[item.postScriptFunctionName](item.tableID);
                            } catch (err) { }
                        },
                        stateSaveCallback: function (settings, s_data) {
                            try {

                                s_data.search.search = ""; // remove search filter, we don't want to use that!
                                s_data.start = 0; // Don't save pagination position

                                var jsonData = {};

                                jsonData.tableID = DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID;
                                jsonData.tableSettings = JSON.stringify(s_data);

                                var json = JSON.stringify(jsonData);
                                var xhttp = new XMLHttpRequest();

                                xhttp.open("POST", "/api/DataTableStates", true);
                                xhttp.send(json);
                            } catch (err) { }
                        },
                        stateLoadCallback: function (settings) {
                            var s_data = null;

                            try {
                                var xhttp = new XMLHttpRequest();
                                xhttp.onreadystatechange = function () {
                                    if (this.readyState == 4) {
                                        if (this.status == 200) {

                                            try {
                                                s_data = JSON.parse(xhttp.responseText);
                                            } catch (err2) { }
                                        }
                                    }
                                };

                                xhttp.open("GET", "/api/DataTableStates/" + DATATABLE_STATE_PREFIX + settings.sInstance.substring(41) + "_" + CURRENT_USER_ID + "/" + "?v=" + new Date().getTime(), false);
                                xhttp.send();
                            } catch (err) { }

                            return s_data;
                        },
                        dom: 'Brtip',
                        deferRender: true,
                        async: true,
                        buttons: [
                            'copyHtml5',
                            {
                                extend: 'excelHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'csvHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'pdfHtml5',
                                orientation: 'landscape'

                            },
                            {
                                extend: 'print',
                                orientation: 'landscape',
                                exportOptions: {
                                    columns: ':visible'
                                },
                                customize: function (win) {

                                    var last = null;
                                    var current = null;
                                    var bod = [];

                                    var css = '@page { size: landscape; }',
                                        head = win.document.head || win.document.getElementsByTagName('head')[0],
                                        style = win.document.createElement('style');

                                    style.type = 'text/css';
                                    style.media = 'print';

                                    if (style.styleSheet) {
                                        style.styleSheet.cssText = css;
                                    } else {
                                        style.appendChild(win.document.createTextNode(css));
                                    }

                                    head.appendChild(style);
                                }
                            },
                            {
                                extend: 'colvis',
                                background: false
                            },
                            {
                                extend: 'colvisGroup',
                                text: 'Show All',
                                show: ['*']
                            },
                            { extend: "create", editor: item.editor },
                            { extend: "edit", editor: item.editor },
                            { extend: "remove", editor: item.editor }
                        ],
                        order: JSON.parse(item.sorting),
                        initComplete: function (settings, json) {

                            let filterCols = settings.oInit.customInitObj.filterCols;

                            if (filterCols != null && filterCols != undefined) {
                                for (let i = 0; i < filterCols.length; i++) {
                                    let filterCol = filterCols[i];

                                    let column = this.api().column(filterCol.columnIndex);
                                    let dropdown = $('#' + filterCol.dropdownID);

                                    column.data().unique().sort().each(function (d, j) {
                                        if (d != null && d != "") {
                                            dropdown.append('<li><a class="' + filterCol.anchorClassName + '" href="#" data="' + d + '"> ' + d + '</a></li>');
                                            $('.' + filterCol.anchorClassName).click(function () {
                                                $('.' + filterCol.selectedAnchorClassName).remove();
                                                $(this).prepend('<i class="fa fa-check ' + filterCol.selectedAnchorClassName + '" aria-hidden="true"></i>');

                                                var filter = $(this).attr('data');
                                                filterCol.current = filter;
                                                applyFilters();
                                            })
                                        }
                                    });
                                }
                            }

                            item.dataTable.columns.adjust();
                        }
                    });

                    $(window).resize(function () {
                        item.dataTable.columns.adjust();
                    });

                    $(window).blur(function () {
                        item.dataTable.columns.adjust();
                    });

                    $("input").blur(function () {
                        item.dataTable.columns.adjust();
                    });
                }
            }
        }

        item.initialised = true;
    }
}

function applyFilters() {
    // Clear filters
    while ($.fn.dataTable.ext.search.length > 0) {
        $.fn.dataTable.ext.search.pop();
    }

    for (let i = 0; i < _dataTables.length; i++) {
        for (let j = 0; j < _dataTables[i].filterCols.length; j++) {
            if (_dataTables[i].filterCols[j].current.length > 0) {
                processFilter(_dataTables[i].filterCols[j]);
            }
        }

        _dataTables[i].dataTable.draw();
    }
}

function processFilter(filterCol) {
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            if (settings.nTable.getAttribute('id') != filterCol.tableID) {
                return true; // does not apply to this tableID
            }

            return (data[filterCol.columnIndex] == filterCol.current) ? true : false;
        }
    );
}

function getDataTable(id) {
    for (let i = 0; i < _dataTables.length; i++) {
        if (_dataTables[i].tableID == id) {
            return _dataTables[i].dataTable;
            break;
        }
    }
}

function initNewDataTables(callback) {
    _dataTables.forEach(initDataTable);

    if (callback) {
        callback();
    }
}

$(document).ready(function () {
    initNewDataTables();
});