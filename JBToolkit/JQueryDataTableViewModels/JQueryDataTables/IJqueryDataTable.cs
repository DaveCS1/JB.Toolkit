namespace JBToolkit.Views
{
    public interface IJqueryDataTable
    {
        string GenerateFilters();
        string GenerateRegisterDataTableScript();
        string GenerateRegisterFiltersScript();
        string GenerateAdditionalJavaScript();
        string GenerateHtmlTable();
    }
}