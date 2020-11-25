using FluentValidation;
using REST.API.DataContracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace REST.IoC.Configuration.RequestsValidation
{
    /// <summary>
    /// https://www.programmingwithwolfgang.com/programming-microservices-net-core-3-1/
    /// Let's check FluentValidation to add more advanced validation rules
    /// </summary>
    public class UserCreationRequestValidation : AbstractValidator<UserCreationRequest>
    {

        public UserCreationRequestValidation()
        {
            RuleFor(x => x.Date)
                .InclusiveBetween(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(12))
                .WithMessage("");

        }
    }
}
