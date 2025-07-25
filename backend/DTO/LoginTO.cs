﻿using System.ComponentModel.DataAnnotations;

namespace SmartContractVehicle.DTO
{
    public class LoginTO
    {

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }
    }
}
