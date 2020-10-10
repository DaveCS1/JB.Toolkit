var FormHelpers = function() { }
FormHelpers.prototype = function() {
    function getFormData(formId)
    {
        var formData = { }
        $.each($(formId).serializeArray(),
            function(i, v) {
            var element = $('[name="' + v.name + '"]')[0]
                var isDatePicker = $(element).hasClass('hasDatepicker')
                var value = v.value
                if (isDatePicker || element.type == 'datetime' || element.type == 'date')
            {
                if (v.value && v.value.toString().length == 10)
                {
                    value = moment(v.value, 'DD/MM/YYYY').format("YYYY-MM-DD HH:mm:ss")
                    }
                else
                {
                    value = null
                    }
            }
            formData[v.name] = value
            })
        return formData
    }
return {
    getFormData: getFormData
    }
}()

var PanelPreloaderHtml = '<div class="centerContainer"><div class="centerP"><img src="/Content/img/small_preloader.gif" /></div></div>';
