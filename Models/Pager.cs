namespace TinTucGameAPI.Models
{
    public class Pager
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int TotalPages { get; set; }
        public Pager(int totalItems, int currentPage, int pageSize = 3)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if(currentPage > totalPages)
            {
                currentPage = totalPages;
            }
            int startPage = currentPage - 4;
            int endPage = currentPage + 5;
            if(startPage <= 0)
            {
                //endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if(endPage > totalPages)
            {
                endPage = totalPages;
                if(endPage > 6 && currentPage > 3)
                {
                    startPage = endPage - 5;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            StartPage = startPage;
            EndPage = endPage;
            TotalPages = totalPages;
        }
    }
}
