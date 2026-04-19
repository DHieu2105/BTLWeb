using BTL_Web.Models;
using System.Collections.Generic;

namespace BTL_Web.ViewModels
{
    public class TeacherAssignmentViewModel
    {
        public List<GiaoVien> AssignedTeachers { get; set; } = new List<GiaoVien>();
        public List<LopHoc> AvailableClasses { get; set; } = new List<LopHoc>();
    }
}
