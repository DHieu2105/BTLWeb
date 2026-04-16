using System;
using System.Collections.Generic;

namespace BTL_Web.Models
{
    public class LopHocPage
    {
        public List<LopHoc> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public int StartItem => TotalItems == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int EndItem => Math.Min(Page * PageSize, TotalItems);
    }
}