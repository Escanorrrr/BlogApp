using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Entities;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;

public interface IUserService
{
    Task<Result<UserDto>> GetByEmailAsync(string email);
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<List<UserDto>>> GetAllAsync();
    Task<Result<UserDto>> AddAsync(UserAddDto userAddDto);
    Task<Result<UserDto>> UpdateAsync(UserUpdateDto userUpdateDto, bool isUpdaterAdmin = false);
    Task<Result<object>> DeleteAsync(int id);
    Task<Result<User>> AuthenticateAsync(LoginDto loginDto);
    Task<Result<User>> AuthenticateAsync(string email, string password);
    Task<Result<User>> RegisterAsync(RegisterDto dto);
    Task<Result<User>> BanUserAsync(int userId);
}




