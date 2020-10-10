/** @description Loads PDF into iframe (embeds) with a given PDF URL (file or internal URL)
 */
function LoadPdFromUrl(iFrameContainer, fileUrl) {
    var pdfjsUrl = 'Scripts/packages/pdfjs/web/viewer.html?file=';
    iFrameContainer.attr('src', pdfjsUrl + fileUrl);
}

/** @description Loads PDF into iframe (embeds) with a give 'raw' base64 (without data type identifier) string
 */
function LoadPdFromBase64String(iFrameContainer, base64) {
    var pdfAsDataUri = base64
    var pdfAsArray = ConvertDataURIToBinary(pdfAsDataUri);
    var url = 'Scripts/packages/pdfjs/web/viewer.html?file=';

    var binaryData = [];
    binaryData.push(pdfAsArray);
    var dataPdf = window.URL.createObjectURL(new Blob(binaryData, { type: "application/pdf" }));

    iFrameContainer.attr('src', url + encodeURIComponent(dataPdf));
}

/** @description Loads PDF into iframe (embeds) with a give byte array
 */
function LoadPdFromUint8Array(iFrameContainer, Uint8Array) {
    var url = '/Scripts/packages/pdfjs/web/viewer.html?file=';

    var binaryData = [];
    binaryData.push(Uint8Array);
    var dataPdf = window.URL.createObjectURL(new Blob(binaryData, { type: "application/pdf" }));

    iFrameContainer.attr('src', url + encodeURIComponent(dataPdf));
}

/** @description Loads PDF into iframe (embeds) with a give byte array
 * Grabs a pdf as base64 string from m-files (hense file id and version required)
 * and embeds it in the given iframe container sourcing a blob as a byte array
 * 
 * iFrame shoud look like this:
 * <div id="mfilesDocument" role="tabpanel" class="tab-pane active pdfobject-container" style="overflow: auto;">
 *    <div style="overflow: hidden; position: absolute; top: 0; right: 0; bottom: 0; left: 0;">
 *      <iframe id="mfileDocumentIFrame" style="border: none; width: 100%; height: 100%;" frameborder="0"></iframe>
 *    </div>
 * </div>
 */
function LoadPdfFromMFiles(iFrameContainer, fileId, fileVersion, fileName, parentContainerOptional,
    preloaderContainerOptional, errorContainerOptional, docThumbnailOptional, recordId, recordType) {

    if (preloaderContainerOptional)
        preloaderContainerOptional.show();

    var data = {};

    data.type = "PDFPreview_m2";
    data.fileId = fileId;
    data.fileVersion = fileVersion;
    data.fileName = fileName;

    var json = JSON.stringify(data);
    var xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {

        if (this.readyState == 4 && this.status == 200) {

            var pdfAsDataUri = xhttp.responseText;
            var pdfAsArray = ConvertDataURIToBinary(pdfAsDataUri);
            var url = '/Scripts/packages/pdfjs/web/viewerEx.html?file=';

            var binaryData = [];
            binaryData.push(pdfAsArray);
            var dataPdf = window.URL.createObjectURL(new Blob(binaryData, { type: "application/pdf" }));

            iFrameContainer.attr('src', url + encodeURIComponent(dataPdf) + "#zoom=FitH&filenameo=" + fileName + "&recordid=" + recordId + "&recordtype=" + recordType);

            $(iFrameContainer).ready(function (event) {

                setTimeout(function () {
                    if (parentContainerOptional) {
                        parentContainerOptional.show();

                        if (preloaderContainerOptional)
                            preloaderContainerOptional.hide();
                    }
                }, 500);

            });
        }
        else if (this.status == 400) {

            if (parentContainerOptional) {
                parentContainerOptional.hide();

                if (preloaderContainerOptional)
                    preloaderContainerOptional.hide();
            }

            if (docThumbnailOptional)
                docThumbnailOptional.hide();

            if (errorContainerOptional)
                errorContainerOptional.show();
        }
    };

    xhttp.open("POST", "/api/Attachments/MFilesPdf", true);
    xhttp.send(json);
}

// Uses pdf.js's viewer to embed into its own page (redirect with post data)
// Allows viewing pdfs on all modern browser with optional input pdf as URL, file path
// or base64 string.
//
// 'inputType' options include:
//
//     url,
//     path,
//     base64
//

/** @description Uses pdf.js's viewer to embed into its own page (redirect with post data)
 * Allows viewing pdfs on all modern browser with optional input pdf as URL, file path
 * or base64 string.
 *
 * inputType' options include: * 
 *      url,
 *      path,
 *      base64 
 */
function LoadFullPageEmbeddedPDF(urlOrPathOrBase64String, inputType, viewOnly, fileName, linkId, linkType) {

    var form = document.createElement('form');
    form.style.visibility = 'hidden'; // no user interaction is necessary
    form.method = 'POST'; // forms by default use GET query strings
    form.action = '/Home/PDFViewer';

    var input1 = document.createElement('input');
    input1.name = "urlOrPathOrBase64String";
    input1.value = urlOrPathOrBase64String;

    form.appendChild(input1);

    if (inputType) {
        var input2 = document.createElement('input');
        input2.name = "inputType";
        input2.value = inputType;

        form.appendChild(input2);

        if (viewOnly != null) {
            var input4 = document.createElement('input');
            input4.name = "viewOnly";
            input4.value = viewOnly;

            form.appendChild(input4);
        }
        else {
            var input4 = document.createElement('input');
            input4.name = "viewOnly";
            input4.value = true;

            form.appendChild(input4);
        }
    }

    var input3 = document.createElement('input');
    input3.name = "returnURL";
    input3.value = window.location.href;

    form.appendChild(input3);

    if (fileName) {
        var input5 = document.createElement('input');
        input5.name = "fileName";
        input5.value = fileName;

        form.appendChild(input5);
    }

    if (linkId) {

        var input6 = document.createElement('input');
        input6.name = "linkId";
        input6.value = linkId;

        form.appendChild(input6);
    }

    if (linkType) {
        var input7 = document.createElement('input');
        input7.name = "linkType";
        input7.value = linkType;

        form.appendChild(input7);
    }

    document.body.appendChild(form); // forms cannot be submitted outside of body
    form.submit(); // send the payload and navigate
}