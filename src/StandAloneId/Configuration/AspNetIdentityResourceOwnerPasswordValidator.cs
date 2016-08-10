using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using StandAloneId.Models;

namespace StandAloneId.Configuration
{
    public class AspNetIdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetIdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return new CustomGrantValidationResult("Email is not confirmed");
                }
                return new CustomGrantValidationResult(user.Id, "password");
            }


            return new CustomGrantValidationResult("Invalid username or password");
        }
    }
}
