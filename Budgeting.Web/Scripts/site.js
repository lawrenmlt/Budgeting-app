function ShowAddUpdateCategoryDiv() {
    $('#AddUpdateCategoryDiv').removeClass('hidden');
}

function GenericFailure(jqXHR, textStatus, error) {
    alert(error);
}

function ReplaceBudgetPlanCategoryListing(url) {
    $.ajax(url, {
        success: function (data) {
            $('#BudgetPlanCategoryListingDiv').html(data);
            //ShowAddUpdateCategoryDiv();
        },
        error: function (jqXHR, status, error) {
            alert(error);
        }
    })
}
function ReplaceCategoryList(url)
{
    $.ajax(url, {
        success: function (data) {
            $('#CategoryListDiv').html(data);
        },
        error: GenericFailure
    })
}
function ReplaceSummaryList(url, replaceDivId)
{
    $.ajax(url, {
        success: function (data) {
            $('#BudgetPlanCategoryDiv_' + replaceDivId).html(data);
        },
        error: GenericFailure
    })
}
function ReplaceDiv(url, replaceDivId) {
    $.ajax(url, {
        success: function (data) {
            $('#' + replaceDivId).html(data);
        },
        error: GenericFailure
    })
}
function DisablePercentageTextBox() {
    $('#PercentageTextBox').val('');
    $('#PercentageTextBox').prop("disabled", true);
    $('#DollarAmountTextBox').prop("disabled", false);
}
function DisableDollarAmountTextBox() {
    $('#DollarAmountTextBox').val('');
    $('#DollarAmountTextBox').prop("disabled", true);
    $('#PercentageTextBox').prop("disabled", false);
}
function EditBudgetPlanCategory(budgetPlanCategoryId, UsePercent) {
    if (UsePercent == 'True') {
        $('#BPCListingSavePercentageBtn_' + budgetPlanCategoryId).removeClass('hidden');
    }
    else {
        $('#BPCListingSaveAmountBtn_' + budgetPlanCategoryId).removeClass('hidden');
    }
    $('#BudgetPlanCategoryListingEditBtn_' + budgetPlanCategoryId).addClass('hidden');
    $('.BPC-disabled-' + budgetPlanCategoryId).prop("disabled", false);
}
function EditCategory(categoryId)
{
    $('#CategoryListEditBtn_' + categoryId).addClass('hidden');
    $('#CategoryListSaveBtn_' + categoryId).removeClass('hidden');
    $('.cat-disabled-' + categoryId).prop("disabled", false);
}