
document.addEventListener("DOMContentLoaded", function (event) {

    GetMainContactEmailAddresses();

    // populate email dropdown

    setTimeout(function () {
        var viewerContainer = document.getElementById("viewerContainer")
        viewerContainer.scrollTop = 0;
    }, 2000);

    var modal = document.getElementById("emailModal");
    var btn1 = document.getElementById("secondaryEmail");
    var btn2 = document.getElementById("email");
    var close = document.getElementById("emailClose");
    var send = document.getElementById("emailSend");
    var inputEmail = document.getElementById("inputEmail");

    var span = document.getElementsByClassName("close")[0];

    btn1.onclick = function () {

        if (PDFViewerApplication.url)
            modal.style.display = "block";
    }

    btn2.onclick = function () {
        if (PDFViewerApplication.url)
            modal.style.display = "block";
    }

    var emailSpinner = document.getElementById("emailSpinner");
    var valMessage = document.getElementById("valMessage");
    var valText = document.getElementById("valText");

    span.onclick = function () {

        valMessage.style.display = "none";
        inputEmail.value = "";
        emailSpinner.style.visibility = "hidden";

        modal.style.display = "none";
    }

    close.onclick = function () {

        valMessage.style.display = "none";
        inputEmail.value = "";
        emailSpinner.style.visibility = "hidden";
        $("#emailSelected").text("- Select Recipient -");

        modal.style.display = "none";
    }

    send.onclick = function () {

        emailSpinner = document.getElementById("emailSpinner");
        valMessage = document.getElementById("valMessage");
        valText = document.getElementById("valText");

        var data = {};
        data.blobUrl = PDFViewerApplication.url;
        data.contentType = "application/pdf";
        data.noreply = true;

        if ($("#emailSelected").text() == "- Select Recipient -")
            data.emailTo = inputEmail.value;
        else if (inputEmail.value)
            data.emailTo = $("#emailSelected").text() + ";" + inputEmail.value;
        else
            data.emailTo = $("#emailSelected").text();

        data.fileName = decodeURIComponent(window.location.hash.split('&')[1].replace("filenameo=", "").replace("#filenameo=", ""));
        data.recordId = "";
        data.recordType = "";

        try {

            if (window.location.hash.split('&').length > 2) {
                data.recordId = decodeURIComponent(window.location.hash.split('&')[2].replace("recordid=", "").replace("#recordid=", ""));
                data.recordType = decodeURIComponent(window.location.hash.split('&')[3].replace("recordtype=", "").replace("#recordtype=", ""));
            }
        }
        catch (err) { }

        if (ValidateEmailAddress(data.emailTo)) {
            emailSpinner.style.visibility = "visible";
            valMessage.style.display = "none";

            var xhttp = new XMLHttpRequest();
            xhttp.onloadend = function (e) {

                if (this.status == 200) {
                    var blob = this.response;

                    var reader = new FileReader();
                    reader.readAsDataURL(blob);

                    reader.onloadend = function () {

                        data.base64data = reader.result.replace("data:application/pdf;base64,", "");

                        var json = JSON.stringify(data);
                        var xhttp2 = new XMLHttpRequest();

                        xhttp2.onreadystatechange = function () {

                            if (this.readyState == 4 && this.status == 200) {

                                emailSpinner.style.visibility = "hidden";
                                modal.style.display = "none";
                                valMessage.style.display = "none";
                                inputEmail.value = "";
                                $("#emailSelected").text("- Select Recipient -");

                                $("#pdfjsDialog").html("<br /><br /><center>Email sent successfully</center>");
                                $("#pdfjsDialog").dialog({
                                    title: "Email Sent",
                                    modal: true  
                                });

                            }
                        };

                        xhttp2.open("POST", "/api/Emailer", true);
                        xhttp2.send(json);
                    }
                }

                else {

                    emailSpinner.style.visibility = "hidden";
                    modal.style.display = "none";
                    valMessage.style.display = "none";
                    inputEmail.value = "";
                    $("#emailSelected").text("- Select Recipient -");

                    emailSpinner.style.style.visibility = "hidden";

                    $("#pdfjsDialog").html("<br />Failed to fetch blob data. Are you sure you've got a document open?");
                    $("#pdfjsDialog").dialog({
                        title: "Email Send Failed",
                        modal: true
                    });
                }
            };

            xhttp.open("GET", data.blobUrl, true);
            xhttp.responseType = 'blob';
            xhttp.send();

        }
        else {
            valText.innerText = "Invalid email address(s)";
            valMessage.style.display = "block";
        }
    }

    /*Dropdown Menu*/

    $('.dropdown').click(function () {
        $(this).attr('tabindex', 1).focus();
        $(this).toggleClass('active');
        $(this).find('.dropdown-menu').slideToggle(300);
    });
    $('.dropdown').focusout(function () {
        $(this).removeClass('active');
        $(this).find('.dropdown-menu').slideUp(300);
    });
    $('.dropdown .dropdown-menu li').click(function () {
        $(this).parents('.dropdown').find('span').text($(this).text());
        $(this).parents('.dropdown').find('input').attr('value', $(this).attr('id'));
    });

    /*End Dropdown Menu*/

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {

        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

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

    // Main Contact Email Address Link
    function GetMainContactEmailAddresses() {

        try {

            if (window.location.hash.split('&').length > 2) {
                var linkId = decodeURIComponent(window.location.hash.split('&')[2].replace("recordid=", "").replace("#recordid=", ""));
                var linkType = decodeURIComponent(window.location.hash.split('&')[3].replace("recordtype=", "").replace("#recordtype=", ""));

                var xhttp = new XMLHttpRequest();
                xhttp.onreadystatechange = function () {

                    if (this.readyState == 4 && this.status == 200) {

                        var value = JSON.parse(xhttp.responseText);

                        if (value.length > 0) {
                            for (var i = 0; i < value.length; i++) {
                                $("#emailSelectionDropdown").show();

                                var recipientEmail = value[i].Name + " &lt;" + value[i].Email + "&gt;";

                                $("#emailULList").append("<li onclick='SelectRecipient(\"" + recipientEmail + "\");'>" + recipientEmail + "</li>");
                            }
                        }
                        else {
                            $("#emailSelectionDropdown").hide();
                        }
                    }
                    else {
                        $("#emailSelectionDropdown").hide();
                    }
                };

                xhttp.open("GET", "/api/EmailerGet/MainContactEmails/" + linkId + "/" + linkType + "?v=" + new Date().getTime(), true);
                xhttp.send();
            }
            else {
                $("#emailSelectionDropdown").hide();
            }
        }
        catch (err) {
            $("#emailSelectionDropdown").hide();
        }
    }
});