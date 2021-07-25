using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCoreIdentity.CustomDescriber
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code = "Password too short",
                Description = $"Password must be min {length}"
            };
        }
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new()
            {
                Code = "Password Requires Non Alphanumeric",
                Description = "Password must has min one alphanumeric chracter"
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new()
            {
                Code = "Duplicate Username",
                Description = $"{userName} already in database "
            };
        }
    }
}
