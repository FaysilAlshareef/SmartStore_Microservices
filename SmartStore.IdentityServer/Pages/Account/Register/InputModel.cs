﻿using System.ComponentModel.DataAnnotations;

namespace Matgr.IdentityServer.Pages.Account.Register
{
    public class InputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
