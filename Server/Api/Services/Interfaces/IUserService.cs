namespace Api.Services.Interfaces;
using Api.Models;
using DataAccess;
public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDTO userDto);
}