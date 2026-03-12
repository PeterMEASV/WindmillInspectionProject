﻿using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record LoginDTO(
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    string Password
);

public record LoginResponseDTO(
    string id,
    string email,
    string password,
    string token,
    string message
);