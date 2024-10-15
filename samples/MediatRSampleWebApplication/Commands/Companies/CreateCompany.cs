﻿using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class CreateCompany
    {
        public record CreateCompanyRequest(string Name);

        [Authorize]
        public record CreateCompanyCommand([FromBody] CreateCompanyRequest Role) : IRequest<Company>;

        public class Handler : IRequestHandler<CreateCompanyCommand, Company>
        {
            public Task<Company> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
            {
                var company = new Company { Name = request.Role.Name, Balance = 0 };
                return Task.FromResult(company);
            }
        }
    }
}
