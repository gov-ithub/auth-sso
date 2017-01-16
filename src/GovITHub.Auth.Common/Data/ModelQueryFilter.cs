namespace GovITHub.Auth.Common.Data
{
    public class ModelQueryFilter
    {
        public int CurrentPage { get; set; } = 0;

        public int ItemsPerPage { get; set; } = 10;

        public string SortBy { get; set; }

        public bool SortAscending { get; set; } = true;

        public ModelQueryFilter()
        {
        }

        public ModelQueryFilter(int currentPage, int itemsPerPage, bool sortAscending, string sortBy)
        {
            if (currentPage > 0)
            {
                CurrentPage = currentPage - 1;
            }

            if (itemsPerPage > 0)
            {
                ItemsPerPage = itemsPerPage;
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                SortBy = sortBy.ToTitleCase();
                SortAscending = sortAscending;
            }
        }
    }
}
