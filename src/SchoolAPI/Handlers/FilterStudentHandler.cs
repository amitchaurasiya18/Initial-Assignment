using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.DTO;
using SchoolAPI.Queries;

namespace SchoolAPI.Handlers
{
    public class FilterStudentHandler: IRequestHandler<FilterStudentQuery, (IEnumerable<Student>, int)>
    {
        private readonly IReadRepository<Student> _readRepository;

        public FilterStudentHandler(IReadRepository<Student> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<(IEnumerable<Student>, int)> Handle(FilterStudentQuery request, CancellationToken cancellationToken)
        {
            var (students, totalCount) = await _readRepository.FilterAsync(
                s => s.IsActive && 
                     (string.IsNullOrEmpty(request.SearchTerm) ||
                      s.FirstName.Contains(request.SearchTerm) ||
                      s.LastName.Contains(request.SearchTerm) ||
                      s.Email.Contains(request.SearchTerm) ||
                      s.Phone.Contains(request.SearchTerm)),
                request.Page,
                request.PageSize
            );

            return (students, totalCount);
        }
    }
}