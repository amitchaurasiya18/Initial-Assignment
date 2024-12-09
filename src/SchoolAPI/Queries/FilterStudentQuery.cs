using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.DTO;

namespace SchoolAPI.Queries
{
    public class FilterStudentQuery: IRequest<(IEnumerable<Student>, int)>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }

        public FilterStudentQuery(int page, int pageSize, string searchTerm)
        {
            Page = page;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }
}