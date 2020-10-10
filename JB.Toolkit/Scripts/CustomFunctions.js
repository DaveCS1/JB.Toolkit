/*
    ************************************************************************
    REUSABLES JAVASCRIPT CUSTOM FUNCTIONS - CAN BE COPIED TO ANOTHER PROJECT
    ************************************************************************
*/

/** @description I.e. http://domina.com/
 */
function BaseUrl() {
    return window.location.protocol + "//" + window.location.host + "/"
}

/** @description Create Url affix: UrlAction("<Action>","<Controler>","<data>");
 */
function UrlAction() {
    var base = window.location.protocol + "//" + window.location.host + "/" + arguments[1] + "/" + arguments[0];
    for (var i = 2; i < arguments.length; ++i)
        base = base + "/" + arguments[i];

    return base;
}

/** @description Create Url affix: UrlApiAction("<end-point>", "<action / data>");
 */
function UrlApiAction() {
    var base = window.location.protocol + "//" + window.location.host + "/" + arguments[0] + "/" + arguments[1];
    for (var i = 2; i < arguments.length; ++i)
        base = base + "/" + arguments[i];

    return base;
}

/** @description Returns browser, i.e. Chrome, Opera, IE
 */
function GetBrowser() {
    if (navigator.userAgent.indexOf("Chrome") != -1) {
        return "Chrome";
    } else if (navigator.userAgent.indexOf("Opera") != -1) {
        return "Opera";
    } else if (navigator.userAgent.indexOf("MSIE") != -1) {
        return "IE";
    } else if (navigator.userAgent.indexOf("NET CLR") != -1) {
        return "IE";
    } else if (navigator.userAgent.indexOf("Windows NT") != -1) {
        return "IE";
    } else if (navigator.userAgent.indexOf("Firefox") != -1) {
        return "Firefox";
    } else {
        return "unknown";
    }
}

/** @description Disabled mouse right click on page
 */
function DisableRightClick() {
    $(function () {
        $(this).bind("contextmenu", function (e) {
            e.preventDefault();
        });
    });
}

/** @description Enabled mouse right click on page
 */
function EnableRightClick() {
    $(document).unbind("contextmenu");
    $(document).bind("contextmenu", function (e) {
        return true;
    });
}

/** @description Shows print with preview window (if available)
 */
function PrintWindow() {
    if (GetBrowser() == "IE") {
        try {
            // IE's 'Print preview' :

            var OLECMDID = 7;
            /* OLECMDID values:
            * 6 - print
            * 7 - print preview
            * 1 - open window
            * 4 - Save As
            */
            var PROMPT = 1; // 2 DONTPROMPTUSER
            var WebBrowser = '<OBJECT ID="WebBrowser1" WIDTH=0 HEIGHT=0 CLASSID="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2"></OBJECT>';
            document.body.insertAdjacentHTML('beforeEnd', WebBrowser);
            WebBrowser1.ExecWB(OLECMDID, PROMPT);
            WebBrowser1.outerHTML = "";
        }
        catch (err) {
            window.print();
        }
    }
    else {
        window.print();
    }
}

/** @description Set the selected dropdown entry by text rather than key value
 */
function SetDropDownByText(dropDownId, textToFind) {
    var dd = document.getElementById(dropDownId);
    for (var i = 0; i < dd.options.length; i++) {
        if (dd.options[i].text === textToFind) {
            dd.selectedIndex = i;
            break;
        }
    }
}

/** @description Disables a HTML element, sets readonly and adds a 'disabled' class name
 *  @param {string} id ID of element to disable
 *  @param {boolean} includeParent In bootstrap, often elements are nested inside another, so include parent
 */
function DisableElement(id, includeParent) {
    if (includeParent) {
        $("#" + id).parent().addClass('disabled');
        $("#" + id).parent().attr('disabled', 'disabled');
        $("#" + id).parent().attr('readonly', 'readonly');
    }

    $("#" + id).addClass('disabled');
    $("#" + id).attr('disabled', 'disabled');
    $("#" + id).attr('readonly', 'readonly');
}

/** @description Enable a HTML element, sets readonly and adds a 'disabled' class name
 *  @param {string} id ID of element to enable
 *  @param {boolean} includeParent In bootstrap, often elements are nested inside another, so include parent
 */
function EnableElement(id, includeParent) {
    if (includeParent) {
        $("#" + id).parent().removeClass('disabled');
        $("#" + id).parent().removeAttr('disabled');
        $("#" + id).parent().removeAttr('readonly');
    }

    $("#" + id).removeClass('disabled');
    $("#" + id).removeAttr('disabled');
    $("#" + id).removeAttr('readonly');
}

/** @description Writes text to a file location
 *  @param {string} content Text to write
 *  @param {string} filePath File path where to save the file
 */
function WriteToFile(content, filePath) {
    var textToWrite = content;
    var textFileAsBlob = new Blob([textToWrite], { type: 'text/plain' });

    if (!filePath)
        filePath = "log-" + GetTimestamp() + ".txt";

    if ('msSaveOrOpenBlob' in navigator) {
        navigator.msSaveOrOpenBlob(textFileAsBlob, filePath);
    } else {
        var downloadLink = document.createElement('a');
        downloadLink.download = filePath;
        downloadLink.innerHTML = 'Download File';

        if ('webkitURL' in window) {
            // Chrome allows the link to be clicked without actually adding it to the DOM.
            downloadLink.href = window.webkitURL.createObjectURL(textFileAsBlob);
        } else {
            // Firefox requires the link to be added to the DOM before it can be clicked.
            downloadLink.href = window.URL.createObjectURL(textFileAsBlob);
            downloadLink.click(function () {
                document.body.removeChild(event.target);
            });

            downloadLink.style.display = 'none';
            document.body.appendChild(downloadLink);
        }
        downloadLink.click();
    }
}

/** @description Starts a file download session
 *  @param {string} targetLinkId Some a href link to use as template (can be a hidden link)
 *  @param {string} downloadUrl Source URl
 */
function DownloadFile(targetLinkId, downloadUrl) {
    var req = new XMLHttpRequest();
    req.open("GET", downloadUrl, true);
    req.responseType = "blob";
    req.setRequestHeader('my-custom-header', 'custom-value'); // adding some headers (if needed)

    req.onload = function (event) {
        var blob = req.response;
        var fileName = null;
        var contentType = req.getResponseHeader("content-type");

        // IE/EDGE seems not returning some response header
        if (req.getResponseHeader("content-disposition")) {
            var contentDisposition = req.getResponseHeader("content-disposition");
            fileName = contentDisposition.substring(contentDisposition.indexOf("=") + 1);
            fileName = fileName.substring(1, fileName.length - 1);
        } else {
            fileName = "unnamed." + contentType.substring(contentType.indexOf("/") + 1);
        }

        if (window.navigator.msSaveOrOpenBlob) {
            // Internet Explorer
            window.navigator.msSaveOrOpenBlob(new Blob([blob], { type: contentType }), fileName);
        } else {
            var el = document.getElementById(targetLinkId);
            el.href = window.URL.createObjectURL(blob);
            el.download = fileName;
            el.click();
        }
    };
    req.send();
}

/** @description Shows a JQuery UI Dialogue, header classes can be: success, dangeer, warning, info, secondary, light, dark, white or default
 */
function JqueryUIDialog(containerId, title, message, headerClassName, timeoutInMs) {
    var tmpID = GenerateGUID();
    $("#" + containerId).append("<div id=" + tmpID + "></div>")
    $("#" + tmpID).html(message);
    $("#" + tmpID).dialog({
        title: title,
        modal: true,
        dialogClass:
            (headerClassName == "success" ||
                headerClassName == "danger" ||
                headerClassName == "warning" ||
                headerClassName == "info" ||
                headerClassName == "secondary" ||
                headerClassName == "light" ||
                headerClassName == "dark" ||
                headerClassName == "white" ||
                headerClassName == "default") ? "header-" + headerClassName : "header-default",
        buttons: [{
            text: "Ok",
            click: function () {
                $(this).dialog("close");
                $("#" + tmpID).remove();
            }
        }],
    });

    if (timeoutInMs) {
        setTimeout(function () {
            try {
                $("#" + tmpID).remove();
            }
            catch (e) { }
        }, timeoutInMs);
    }
}

/** @description Format time in AM / PM format
 */
function FormatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

/** @description Format number to 2 decimal places
 */
function FormatCurrency(amount) {
    if (!amount)
        return "";

    var neg = false;
    if (amount < 0) {
        neg = true;
        total = Math.abs(amount);
    }
    return (neg ? "-" : '') + parseFloat(amount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "1,").toString();
}

/** @description Simulate a mouse click on an element by ID
 */
function SimulateClickById(id) {
    var evt = document.createEvent("MouseEvents");
    evt.initMouseEvent("click", true, true, window,
        0, 0, 0, 0, 0, false, false, false, false, 0, null);
    var a = document.getElementById(id);
    a.dispatchEvent(evt);
}

/** @description Simulate a mouse click on an element by class name
 */
function SimulateClickByClass(className) {
    var evt = document.createEvent("MouseEvents");
    evt.initMouseEvent("click", true, true, window,
        0, 0, 0, 0, 0, false, false, false, false, 0, null);
    var a = document.getElementsByClassName(className)[0];
    a.dispatchEvent(evt);
}

/** @description Simulate a mouse click on an element by query selector
 */
function SimulateClickByQuerySelector(query) {
    var evt = document.createEvent("MouseEvents");
    evt.initMouseEvent("click", true, true, window,
        0, 0, 0, 0, 0, false, false, false, false, 0, null);
    var a = document.querySelector(query);
    a.dispatchEvent(evt);
}

/** @description Show / Select a Boostrap tab
 */
function ShowTab(id) {
    $("#" + id).trigger('click');
}

/** @description Checks whether a string is valid json
 */
function IsJsonString(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}

/** @description After copying text to clipboard, this function shows a tooltip indicating success
 */
function ShowCopiedTooltip(btn) {

    try {
        btn.classList.add("tooltipped");

        if (btn.id.indexOf("eBtn") > -1)
            btn.classList.add("tooltipped-n");
        else
            btn.classList.add("tooltipped-s");

        setTimeout(function () {
            btn.classList.remove("tooltipped");
            btn.classList.remove("tooltipped-s");
            btn.classList.remove("tooltipped-n");
        }, 3000);
    }
    catch (err) {

    }
}

/** @description Use $("#someElem").invisible(); instead of $("#someElem").hide() to keep element on page
 *               Use $("#someElem").visible(); instead of $("#someElem").show() to keep element on page
 */
(function ($) {
    $.fn.invisible = function () {
        return this.each(function () {
            $(this).css("visibility", "hidden");
        });
    };
    $.fn.visible = function () {
        return this.each(function () {
            $(this).css("visibility", "visible");
        });
    };
}(jQuery));

/** @description Use $("#someElem").fadeInVisibility(); instead of $("#someElem").fadeIn() to keep element on page
 *               Use $("#someElem").fadeOutVisibility(); instead of $("#someElem").fadeOut() to keep element on page
 */
(function ($) {
    $.fn.fadeInVisibility = function () {
        return this.each(function () {
            $(this).css('opacity', 0);
            $(this).css('visibility', 'visible');
            $(this).animate({ opacity: 1 }, 1000);
        });
    };
    $.fn.fadeOutVisibility = function () {
        return this.each(function () {
            $(this).animate({ opacity: 0 }, 1000);
            $(this).css('visibility', 'hidden');
        });
    };
}(jQuery));

function AddDays(theDate, days) {
    return new Date(theDate.getTime() + days * 24 * 60 * 60 * 1000);
}

/** @description Organises entries in a dropdown by text alphabetically
 */
function SortSelectPickerOptionsByText(elementId) {
    var my_options = $("#" + elementId + " option");
    my_options.sort(function (a, b) {
        if (a.text > b.text) return 1;
        else if (a.text < b.text) return -1;
        else return 0;
    });

    $("#" + elementId).empty().append(my_options).selectpicker("refresh");
}

/** @description Parses text using regular expressions, and any links (i.e. https://something.com) found are converted
 * to actual <a href= links
 */
function ConvertUrlInTextToLinks(text) {
    return (text || "").replace(
        /([^\S]|^)(((https?\:\/\/)|(www\.))(\S+))/gi,
        function (match, space, url) {
            var hyperlink = url;
            if (!hyperlink.match('^https?:\/\/')) {
                hyperlink = 'http://' + hyperlink;
            }
            return space + '<a href="' + hyperlink + '">' + url + '</a>';
        }
    );
};

/** @description Returns whether or not an email address is valid using regular expressions
 */
function ValidateEmailAddress(email) {
    var expression = /(?!.*\.{2})^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    var allOK;
    var parts = email.split(';')

    for (var i = 0; i < parts.length; i++) {

        var theEmail = parts[i];
        if (theEmail.indexOf('<') !== -1 || theEmail.indexOf('>') !== -1) {
            var parts2 = theEmail.split("<");
            var parts3 = parts2[1].split(">");

            theEmail = parts3[0];
        }

        if (i > 0 && theEmail != "")
            allOK = expression.test(String(theEmail).toLowerCase());

        if (allOK == false)
            return false;
    }

    return true;
}

/** @description Input mask of different types (i.e. if set to MONEY or NUMBER you can't enter text into the input filed).
 * Checks types include: NUMBER, MONEY, NUMBER-SPACE-COMMA and DATE
 */
function ValidateDataType(obj, type) {
    // delete, backspace, shift, tab
    if (!(event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16)) {
        if (type == "NUMBER") {
            // arrows, dash
            if (event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 109 || event.keyCode == 189) {

                // invert negative positive if '-' pressed
                if (event.keyCode == 109 || event.keyCode == 189) {
                    if (obj.value.includes("-")) {
                        obj.value = obj.value.replace("-", "");
                        event.preventDefault();
                    }
                    else {
                        obj.value = "-" + obj.value;
                        event.preventDefault();
                    }
                }
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (!((event.keyCode > 47 && event.keyCode < 58) || (event.keyCode > 94 && event.keyCode < 106))) {
                    event.preventDefault();
                }
            }
        }
        if (type == "NUMBER-SPACE-COMMA") {
            // arrows, dash, comma, space
            if (event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 109 || event.keyCode == 189 || event.keyCode == 188 || event.keyCode == 32) {

                // invert negative positive if '-' pressed
                if (event.keyCode == 109 || event.keyCode == 189) {
                    if (obj.value.includes("-")) {
                        obj.value = obj.value.replace("-", "");
                        event.preventDefault();
                    }
                    else {
                        obj.value = "-" + obj.value;
                        event.preventDefault();
                    }
                }
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (!((event.keyCode > 47 && event.keyCode < 58) || (event.keyCode > 94 && event.keyCode < 106))) {
                    event.preventDefault();
                }
            }
        }
        else if (type == "MONEY") {
            // arrows, dash, full stop
            if (event.keyCode == 190 || event.keyCode == 110 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 109 || event.keyCode == 189) {
                // only allow 1 full stop
                if (event.keyCode == 190 || event.keyCode == 110) {
                    if (obj.value.includes(".")) {
                        event.preventDefault();
                    }
                }
                // invert negative positive if '-' pressed
                if (event.keyCode == 109 || event.keyCode == 189) {
                    if (obj.value.includes("-")) {
                        obj.value = obj.value.replace("-", "");
                        event.preventDefault();
                    }
                    else {
                        obj.value = "-" + obj.value;
                        event.preventDefault();
                    }
                }
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (!((event.keyCode > 47 && event.keyCode < 58) || (event.keyCode > 94 && event.keyCode < 106))) {
                    event.preventDefault();
                }
                else {
                    var n = obj.value.indexOf(".");
                    if (n != -1) {
                        if (obj.value.length - n > 2)
                            event.preventDefault();
                    }
                }
            }
        }
        else if (type == "DATE") {
            // arrows, slash
            if (event.keyCode == 191 || event.keyCode == 111 || event.keyCode == 37 || event.keyCode == 39) {

                // only allow 1 full stop
                if (event.keyCode == 191 || event.keyCode == 111) {
                    if (obj.value.length == 2 || obj.value.length == 5) {
                        if (obj.value.length > 9) {
                            event.preventDefault();
                        }
                    }
                    else {
                        event.preventDefault();
                    }
                }
            }
            else {

                // Ensure that it is a number and stop the keypress
                if (!((event.keyCode > 47 && event.keyCode < 58) || (event.keyCode > 94 && event.keyCode < 106))) {
                    event.preventDefault();
                }

                // only allow certain quanity of number unless slash is entered
                else {
                    if (obj.value.length > 9) {
                        event.preventDefault();
                    }
                    else if (obj.value.length == 2 || obj.value.length == 5) {
                        event.preventDefault();
                    }
                }
            }
        }
    }
}

/** @description Input make helper. I.e. if set to DATE, the after you enter say '22' a forward slash will
 * then be automatically appended to the input field
 * Checks types include: DATE
 */
function DataTypeHelper(obj, type) {
    // allows user not to have to enter slash '/'
    if (type == "DATE") {
        if (obj.value.length == 2 || obj.value.length == 5) {
            if (event.keyCode != 8)
                obj.value = obj.value + "/";
        }
    }
}

/** @description Simply returns a random integer
 */
function GetRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

/** @description Get number sequence timestamp based on date and time (long)
 */
function GetTimestamp() {
    let current_datetime = new Date()
    let formatted_date = "" + current_datetime.getFullYear() + "" + (current_datetime.getMonth() + 1) + "" + current_datetime.getDate() + "-" + current_datetime.getHours() + "" + current_datetime.getMinutes() + "" + current_datetime.getSeconds();
    return formatted_date
}

/** @description Get number sequence timestamp based on date only (short)
 */
function GetDateOnlyTimestamp() {
    let current_datetime = new Date()
    let formatted_date = "" + current_datetime.getFullYear() + "" + (current_datetime.getMonth() + 1) + "" + current_datetime.getDate();
    return formatted_date
}

/** @description custom copy to clipboard (in case native doesn't work)
 * Basically creates a hidden text area on focus pane, copies of a control to it, and does
 * a browser method for highlighting and copying the text
 *  @param {any} elementObject The element id if a text area or input to copy to the clipboard
 *  @param {string} placeholderClass Class name of any element on the page to use for the DOM
 */
function CustomCopyToClipboard(elementObject, placeholderClass) {
    try {
        var randomNumber = GetRandomInt(1, 999);
        var targetId = "_hiddenCopyText_" + randomNumber;
        var target = document.getElementById(targetId);

        target = document.createElement("textarea");
        target.style.position = "absolute";
        target.style.left = "-9999px";
        target.style.top = "0";
        target.id = targetId;

        if (placeholderClass)
            $("." + placeholderClass).append(target);
        else
            document.body.appendChild(target);

        // trim whitespace
        var brRegex = /<br\s*[\/]?>/gi;

        try {
            target.textContent = elementObject.textContent.replace(brRegex, "\r\n").replace(/<\/?[a-zA-Z]+\/?>/g, '').trim();
        }
        catch (err1) {
            try {
                target.textContent = elementObject.val().replace(brRegex, "\r\n").replace(/<\/?[a-zA-Z]+\/?>/g, '').trim();

                if (target.textContent == "")
                    target.textContent = elementObject.text().replace(brRegex, "\r\n").replace(/<\/?[a-zA-Z]+\/?>/g, '').trim();
            }
            catch (err2) {
                try {
                    target.textContent = elementObject.text().replace(brRegex, "\r\n").replace(/<\/?[a-zA-Z]+\/?>/g, '').trim();
                }
                catch (err2) {
                    try {
                        target.textContent = elementObject.html().replace(brRegex, "\r\n").replace(/<\/?[a-zA-Z]+\/?>/g, '').trim();
                    }
                    catch (err3) {

                    }
                }
            }
        }

        target.focus();
        target.setSelectionRange(0, target.value.length);
        document.execCommand("copy", false, target.textContent);

        $('#' + targetId).remove();
    }
    catch (err) { }
}

/** @description Convert base64 string to byte array
 *  @param {string} base64 Base64 string
 */
function ConvertDataURIToBinary(base64) {
    var raw = window.atob(base64);
    var rawLength = raw.length;
    var array = new Uint8Array(new ArrayBuffer(rawLength));

    for (var i = 0; i < rawLength; i++) {
        array[i] = raw.charCodeAt(i);
    }
    return array;
}

/** @description Simply positions 1 element next to another element
 */
function PositionElementRelativeToAnother(source, target) {
    source.position({
        my: "left top",
        at: "left bottom",
        of: target,
        collision: "fit"
    });
}

/** @description Generate a unique GUID
 */
function GenerateGUID() { // Public Domain/MIT
    var d = new Date().getTime(); //Timestamp
    var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) { //Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else { //Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

/** @description Dropdown onChange will always fire even if selecting the same item
 */
$.fn.alwaysChange = function (callback) {
    return this.filter('select').each(function () {
        var elem = this;
        var $this = $(this);

        $this.change(function () {
            callback($this.val());
        }).focus(function () {
            elem.selectedIndex = -1;
            elem.blur();
        });
    });
}

/** @description Force redraw of an element
 */
function RedrawElement(id) {
    $("#" + id).hide().show(0);
}

/** @description JQuery DataTables, particular server side, responsive database sometimes have trouble rendering
 * (particularly, aligning the headers with the body in the table), this function seems to resolve it. Call after creation
 */
function RefreshDataTableColumnsWidths(tableId) {
    let table = $('#' + tableId).DataTable();

    table.columns.adjust();
    table.responsive.recalc();
    table.columns.adjust();

    setTimeout(function () {
        table.columns.adjust();
        table.responsive.recalc();
        table.columns.adjust();
    }, 250);

    $(window).resize();
    $(window).trigger('resize');
    window.dispatchEvent(new Event('resize'));
}