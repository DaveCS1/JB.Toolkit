/** @description Using jquery.filer it initialises a file upload section with a drag and drop interface
 *  @param {string} elementId ID of div to convert to a file uploader
 *  @param {string} uploadFileUrl Upload file API action location URL
 *  @param {string} removeFileUrl Remove file API action location URL
 *  @param {string} sessionId Unique upload session GUID (used to identify when submitting the uploaded files - They're stored temporarily in the database)
 *  @param {string} scrollToBottomDivId Optional DIV of where to scroll the page to when adding files
 */
function InitialiseFileUploader(elementId, uploadFileUrl, removeFileUrl, sessionId, scrollToBottomDivId) {
    $("#" + elementId).filer({
        limit: null,
        maxSize: null,
        fileMaxSize: 50, // 50MB
        extensions: null,
        changeInput: '<div class="jFiler-input-dragDrop"><div class="jFiler-input-inner"><div class="jFiler-input-icon"><i class="icon-jfi-cloud-up-o"></i></div><div class="jFiler-input-text"><h3>Drag & drop files here</h3> <span style="display:inline-block; margin: 15px 0">or</span></div><a class="jFiler-input-choose-btn blue">Browse Files</a></div></div>',
        showThumbs: true,
        theme: "dragdropbox",
        templates: {
            box: '<ul class="jFiler-items-list jFiler-items-grid"></ul>',
            item: '<li class="jFiler-item">\
						<div class="jFiler-item-container">\
							<div class="jFiler-item-inner">\
								<div class="jFiler-item-thumb">\
									<div class="jFiler-item-status"></div>\
									<div class="jFiler-item-thumb-overlay">\
										<div class="jFiler-item-info">\
											<div style="display:table-cell;vertical-align: middle;">\
												<span class="jFiler-item-title"><b title="{{fi-name}}">{{fi-name}}</b></span>\
												<span class="jFiler-item-others">{{fi-size2}}</span>\
											</div>\
										</div>\
									</div>\
									{{fi-image}}\
                                </div>\
                                <span class="jFilter-Title-Ellipse">{{fi-name}}</span>\
								<div class="jFiler-item-assets jFiler-row">\
                                    <ul class="list-inline pull-left">\
										<li>{{fi-progressBar}}</li>\
									</ul>\
									<ul class="list-inline pull-right">\
										<li><a class="icon-jfi-trash jFiler-item-trash-action" style="text-decoration: none;"></a></li>\
									</ul>\
								</div>\
							</div>\
						</div>\
					</li>',
            itemAppend: '<li class="jFiler-item">\
							<div class="jFiler-item-container">\
								<div class="jFiler-item-inner">\
									<div class="jFiler-item-thumb">\
										<div class="jFiler-item-status"></div>\
										<div class="jFiler-item-thumb-overlay">\
											<div class="jFiler-item-info">\
												<div style="display:table-cell;vertical-align: middle;">\
													<span class="jFiler-item-title"><b title="{{fi-name}}">{{fi-name}}</b></span>\
													<span class="jFiler-item-others">{{fi-size2}}</span>\
												</div>\
											</div>\
										</div>\
										{{fi-image}}\
									</div>\
									<div class="jFiler-item-assets jFiler-row">\
										<ul class="list-inline pull-left">\
											<li><span class="jFiler-item-others">{{fi-icon}}</span></li>\
										</ul>\
										<ul class="list-inline pull-right">\
											<li><a class="icon-jfi-trash jFiler-item-trash-action" style="text-decoration: none;"></a></li>\
										</ul>\
									</div>\
								</div>\
							</div>\
						</li>',
            progressBar: '<div class="bar"></div>',
            itemAppendToEnd: false,
            canvasImage: true,
            removeConfirmation: false,
            _selectors: {
                list: '.jFiler-items-list',
                item: '.jFiler-item',
                progressBar: '.bar',
                remove: '.jFiler-item-trash-action'
            }
        },
        dragDrop: {
            dragEnter: null,
            dragLeave: null,
            drop: null,
            dragContainer: null,
        },
        uploadFile: {
            url: uploadFileUrl + "/" + sessionId,
            data: null,
            type: 'POST',
            enctype: 'multipart/form-data',
            synchron: true,
            beforeSend: function (data, itemEl) {
                // Any divs you want scroller to bottom
                try {
                    setTimeout(function () {
                        if (scrollToBottomDivId)
                            $("#" + scrollToBottomDivId).scrollTop($("#" + scrollToBottomDivId)[0].scrollHeight);
                    }, 1);
                }
                catch (err) { }
            },
            success: function (data, itemEl, listEl, boxEl, newInputEl, inputEl, id) {
                if (data.result == "success") {
                    var parent = itemEl.find(".jFiler-jProgressBar").parent();
                    var new_file_name = data.filename;
                    var filerKit = inputEl.prop("jFiler");
                    filerKit.files_list[id].name = new_file_name;
                    itemEl.find(".jFiler-jProgressBar").fadeOut("slow", function () {
                        $("<div class=\"jFiler-item-others text-success\"><i class=\"icon-jfi-check-circle\"></i> Success</div>").hide().appendTo(parent).fadeIn("slow");
                    });
                }
                else {
                    var parent = itemEl.find(".jFiler-jProgressBar").parent();
                    itemEl.find(".jFiler-jProgressBar").fadeOut("slow", function () {
                        $("<div class=\"jFiler-item-others text-error\"><i class=\"icon-jfi-minus-circle\"></i> Error</div>").hide().appendTo(parent).fadeIn("slow");
                    });
                }
            },
            error: function (el) {
                var parent = el.find(".jFiler-jProgressBar").parent();
                el.find(".jFiler-jProgressBar").fadeOut("slow", function () {
                    $("<div class=\"jFiler-item-others text-error\"><i class=\"icon-jfi-minus-circle\"></i> Error</div>").hide().appendTo(parent).fadeIn("slow");
                });
            },
            statusCode: null,
            onProgress: null,
            onComplete: null
        },
        files: null,
        addMore: false,
        allowDuplicates: true,
        clipBoardPaste: true,
        excludeName: null,
        beforeRender: null,
        afterRender: null,
        beforeShow: null,
        beforeSelect: null,
        onSelect: null,
        afterShow: null,
        onRemove: function (itemEl, file, id, listEl, boxEl, newInputEl, inputEl) {
            var filerKit = inputEl.prop("jFiler");
            var file_name = filerKit.files_list[id].name;
            $.post(removeFileUrl + "/",
                JSON.stringify({
                    Filename: file_name,
                    SessionId: sessionId,
                    FileIndex: id,
                    CaseId: caseId != undefined ? caseId : null
                }));

            setTimeout(function () {
                $('#' + scrollToBottomDivId).hide().show(0);
            }, 500);
        },
        onEmpty: null,
        options: null,
        dialogs: {
            alert: function (text) {
                return alert(text);
            },
            confirm: function (text, callback) {
                confirm(text) ? callback() : null;
            }
        },
        captions: {
            button: "Choose Files",
            feedback: "Choose files To Upload",
            feedback2: "files were chosen",
            drop: "Drop file here to Upload",
            removeConfirmation: "Are you sure you want to remove this file?",
            errors: {
                filesLimit: "Only {{fi-limit}} files are allowed to be uploaded.",
                filesType: "Only Images are allowed to be uploaded.",
                filesSize: "{{fi-name}} is too large! Please upload file up to {{fi-maxSize}} MB.",
                filesSizeAll: "Files you've choosed are too large! Please upload files up to {{fi-maxSize}} MB."
            }
        }
    });
}
