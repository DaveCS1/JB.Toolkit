var DataTablesHelpers = function () { }
DataTablesHelpers.prototype = function () {
    function getColumnDefinitions(tableSelector) {
        var columnDefs = []
        var columns = []
        $(tableSelector + ' th').each(function (index, value) {
            var mapsTo = $(value).attr('data-maps-to')
            var tooltipOnOverflow = ($(value).attr('data-wrap-content') == 'false' || null)
            var dataClass = ($(value).attr('data-class') || '')
            var className = $(value).attr('class')

            var width = $(value).attr('data-column-width')

            if (!className) {
                if (mapsTo) {
                    className = mapsTo.toLowerCase() + '-col'
                } else {
                    className = 'unidentified-' + index + '-col'
                }
                $(value).addClass(className)
                var text = $(value).text()
                if (!text && mapsTo) {
                    var thName = mapsTo
                    thName = thName.replace(/_([iI][dD])$/, 'ID')
                    thName = thName.replace(/_/g, ' ')
                    $(value).text(thName)
                }
                var colFormat = $(value).attr('data-column-formatting')
                var formatting = null
                if (colFormat) {
                    switch (colFormat) {
                        case "currency":
                        case "currency-default":
                            formatting = columnFormatting.Currency(className, '', 0)
                            dataClass += ' text-right'
                            break;
                        case "short-date":
                            formatting = columnFormatting.ShortDate(className)
                            break;
                        case "number":
                            dataClass += ' text-right'
                            break;
                        case "bool-yesno":
                            dataClass += ' text-right'
                            formatting = columnFormatting.BoolYesNo(className, '')
                            break;
                        case "datepicker":
                            dataClass += ' text-right'
                            var isDisabled = (($(value).attr('data-is-disabled') || '').toLowerCase() == "true" ? true : false)
                            var inputIdMapsTo = ($(value).attr('data-column-editor-id-maps-to') || null)
                            formatting = columnFormatting.DatePicker(className, isDisabled, inputIdMapsTo)
                            break;
                    }
                }

                if (tooltipOnOverflow || (tooltipOnOverflow == null && width)) {
                    dataClass += ' tooltip-on-overflow'
                }

                var dataUrlTemplate = $(value).attr('data-url')
                if (dataUrlTemplate) {
                    formatting = columnFormatting.UrlTemplate(className, dataUrlTemplate)
                }

                if (!formatting) {
                    formatting = {}
                }
                columnDefs.push(formatting)
            }

            var column = { data: mapsTo }
            if (dataClass) {
                column.className = dataClass
            }
            if (width) {
                column.width = width
            }

            columns.push(column)
        })
        return {
            columnDefs: columnDefs,
            columns: columns
        }
    }
    function hasAutoInitialiseProperty(tableSelector) {
        return $(tableSelector).hasClass('auto-initialise')
    }
    function getSelectedRows(tableId) {
        return $(tableId).DataTable().rows({ selected: true })
    }

    function getSelectedRowsCount(tableId) {
        var selectedRow = getSelectedRows(tableId)
        if (!selectedRow) return 0;
        return selectedRow.length
    }
    function getSelectedRowData(tableId) {
        var rowCount = getSelectedRowsCount(tableId)
        if (rowCount == 1) {
            var rows = getSelectedRows(tableId)
            return rows.data()[0]
        }
        return null;
    }
    function applyThousandFormatting(n) {
        if (n) {
            var x = n.toString().split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }
    }
    function destroyDatatable(datatableId) {
        var table = $(datatableId)
        removeExistingDatatableButtons(datatableId)
        table.DataTable().buttons().destroy()
        table.DataTable().destroy()
    }
    function removeExistingDatatableButtons(datatableId) {
        var buttons = [];
        var table = $(datatableId)
        if (table.DataTable().buttons().length > 0) {
            $.each(table.DataTable().buttons()[0].inst.s.buttons,
                function () {
                    buttons.push(this);
                });
            $.each(buttons,
                function () {
                    table.DataTable().buttons()[0].inst.remove(this.node);
                });
        }
    }
    function getToShortTimeColumnFormatter(columnId) {
        return getMomentColumnFormatter(columnId, "HH:mm:ss", "")
    }
    function getToShortDateColumnFormatter(columnId) {
        return getMomentColumnFormatter(columnId, "DD/MM/YYYY", "")
    }
    function getToShortDateTimeColumnFormatter(columnId) {
        return getMomentColumnFormatter(columnId, "DD/MM/YYYY HH:mm:ss", "")
    }
    function getNullMaskColumnFormatter(columnId, nullMask) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                if (!data) return nullMask
                return data;
            }
        }
    }

    function getDatePickerColumnFormatter(columnId, isDisabled, inputIdMapsTo) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                var dateValue = (data ? moment(data).format("DD/MM/YYYY") : null)
                var orderValue = (data ? moment(data).format("YYYY-MM-DD") : null)
                var id = (inputIdMapsTo ? 'id="' + row[inputIdMapsTo] + '"' : '')
                var disabled = (isDisabled ? 'disabled' : '')
                var html = '<p class="column-sort" hidden>' + (orderValue || '') + '</p><input ' + id + ' ' + disabled + ' style="width:100%;" type="text" value="' + (dateValue ? dateValue : '') + '" />'
                return html;
            }
        }
    }
    function getBoolYesNoColumnFormatter(columnId, nullMask) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                if (data == undefined || data == null) return nullMask
                if (data == false) return 'No'
                return 'Yes';
            }
        }
    }
    function getCurrencyColumnFormatter(columnId, nullMask, numberOfDecimalPlacesToRoundTo, alwaysFormatToFixed) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                if (!data && nullMask == '') return nullMask
                var value = data
                if (!data) value = nullMask
                if (value == '') return nullMask

                if (numberOfDecimalPlacesToRoundTo == null || numberOfDecimalPlacesToRoundTo != 0)
                    return '£' + applyThousandFormatting(Number(value))

                var containsDecimal = value.toString().replace(/[.]/g) != value.toString()
                if (containsDecimal || alwaysFormatToFixed) {
                    return '£' + applyThousandFormatting(Number(value).toFixed(numberOfDecimalPlacesToRoundTo))
                }

                return '£' + applyThousandFormatting(Number(value).toLocaleString())
            }
        }
    }
    function getUrlColumnFormatter(columnId, url) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                var formatted = "<a href=\"" + url(data) + "\">" + data + "</a>"
                return formatted
            }
        }
    }

    function getUrlTemplateColumnFormatter(columnId, urlTemplate) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                var formatted = "<a href=\"" + urlTemplate.replace("{data}", data) + "\">" + data + "</a>"
                return formatted
            }
        }
    }
    function getMomentColumnFormatter(columnId, format, nullMask) {
        return {
            "targets": columnId,
            "render": function (data, type, row) {
                if (nullMask != null) {
                    if (!data) return nullMask
                }
                return moment(data).format(format)
            }
        }
    }
    function addWidthFormatter(widthInPixelOrPercent, formatter) {
        return Object.assign(formatter, { "width": widthInPixelOrPercent })
    }
    function getWidthFormatter(columnId, widthInPixelOrPercent) {
        return {
            "targets": columnId,
            "width": widthInPixelOrPercent
        }
    }
    var columnFormatting = {
        ShortDate: getToShortDateColumnFormatter,
        ShortDateTime: getToShortDateTimeColumnFormatter,
        ShortTime: getToShortTimeColumnFormatter,
        NullMask: getNullMaskColumnFormatter,
        Currency: getCurrencyColumnFormatter,
        BoolYesNo: getBoolYesNoColumnFormatter,
        DatePicker: getDatePickerColumnFormatter,
        Moment: getMomentColumnFormatter,
        Url: getUrlColumnFormatter,
        UrlTemplate: getUrlTemplateColumnFormatter,
        Width: getWidthFormatter,
        AddWidth: addWidthFormatter
    }

    return {
        LengthMenu: {
            AllOnly: [[-1], ["All"]],
            Default: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]]
        },
        ColumnFormatting: columnFormatting,
        destroyDatatable: destroyDatatable,
        getToShortDateColumnFormatter: getToShortDateColumnFormatter,
        getToShortDateTimeColumnFormatter: getToShortDateTimeColumnFormatter,
        getToShortTimeColumnFormatter: getToShortTimeColumnFormatter,
        getNullMaskColumnFormatter: getNullMaskColumnFormatter,
        getCurrencyColumnFormatter: getCurrencyColumnFormatter,
        getMomentColumnFormatter: getMomentColumnFormatter,
        getUrlColumnFormatter: getUrlColumnFormatter,
        getWidthFormatter: getWidthFormatter,
        getSelectedRowData: getSelectedRowData,
        addWidthFormatter: addWidthFormatter,
        hasAutoInitialiseProperty: hasAutoInitialiseProperty,
        getColumnDefinitions: getColumnDefinitions
    }
}()
