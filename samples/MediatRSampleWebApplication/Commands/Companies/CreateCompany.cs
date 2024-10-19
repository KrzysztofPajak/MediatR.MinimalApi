using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Companies
{
    public static class CreateCompany
    {        
        public record CreateCompanyRequest(string Name);

        [Authorize]
        public record CreateCompanyCommand([FromBody] CreateCompanyRequest Company) : IRequest<Company>;

        public class Handler : IRequestHandler<CreateCompanyCommand, Company>
        {
            public Task<Company> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
            {
                var company = new Company { Name = request.Company.Name, Balance = 0 };
                return Task.FromResult(company);
            }
        }
    }
}
