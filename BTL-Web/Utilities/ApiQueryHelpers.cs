namespace BTL_Web.Utilities;

public static class ApiQueryHelpers
{
    public static (int Page, int PageSize) NormalizePaging(int page, int pageSize, int defaultPageSize = 10, int maxPageSize = 100)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = defaultPageSize;
        }

        if (pageSize > maxPageSize)
        {
            pageSize = maxPageSize;
        }

        return (page, pageSize);
    }
}
