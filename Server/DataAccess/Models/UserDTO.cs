using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record CreateUserDTO([Required] [EmailAddress] string email,[Required] [MinLength(8)] string password);
