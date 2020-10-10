var JqueryHelpers = function () { }
JqueryHelpers.prototype = function () {
    function setCheckButtonCheckedState(checkboxId, isChecked) {
        $(checkboxId).prop('checked', isChecked).change();
        if (isChecked) {
            $(checkboxId).parent().addClass('active')
        }
        else {
            $(checkboxId).parent().removeClass('active')
        }
    }
    function setBootstrapToggleCheckedState(toggleId, isChecked) {
        $(toggleId).bootstrapToggle((isChecked ? 'on' : 'off'))
    }
    function handleCheckboxGroupActingAsRadioGroup(changedCheckboxId, checkboxGroupIds) {
        var item = $(changedCheckboxId)
        var isChecked = (item.is(':checked'))
        var currentId = item.attr('id')
        if (isChecked) {
            var otherToggle = checkboxGroupIds.filter(function (x) {
                return x != '#' + currentId
            })
            otherToggle.forEach(function (checkboxId) {
                setCheckButtonCheckedState(checkboxId, false)
            })
        }
    }
    return {
        setBootstrapToggleCheckedState: setBootstrapToggleCheckedState,
        setCheckButtonCheckedState: setCheckButtonCheckedState,
        handleCheckboxGroupActingAsRadioGroup: handleCheckboxGroupActingAsRadioGroup
    }
}()
