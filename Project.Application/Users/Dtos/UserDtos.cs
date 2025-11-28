using System;

namespace Project.Application.Users.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string EmailAddress { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginTime { get; set; }
}

public class CreateUserDto
{
    public string UserName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string? Password { get; set; } // Made optional for admin creating users without password
    public string? PhoneNumber { get; set; }
    public bool ShouldChangePasswordOnNextLogin { get; set; } = false;
    public bool SendWelcomeEmail { get; set; } = true;
}

public class UpdateUserDto
{
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}
