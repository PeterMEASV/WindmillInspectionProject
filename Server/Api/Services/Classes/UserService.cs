﻿
using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class UserService(MyDbContext context, ILogger<UserService> logger, KonciousArgon2idPasswordHasher passwordHasher) : IUserService
{
    public async Task<User> CreateUserAsync(CreateUserDTO userDto)
    {
        logger.LogInformation("Creating user {Email}", userDto.email);
        

        //Validator
        if (string.IsNullOrWhiteSpace(userDto.email))
        {
            throw new ArgumentException("Fill out all fields");
        }

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == userDto.email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = userDto.email,
            Password = passwordHasher.HashPassword(null, userDto.password),

        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Created user {UserId} - {Email}", user.Id, user.Email);

        return user;
    }
}