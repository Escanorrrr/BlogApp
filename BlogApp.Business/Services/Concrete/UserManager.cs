using BlogApp.Business.Services.Abstract;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;
using BlogApp.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using BlogApp.Business.Helpers;

namespace BlogApp.Business.Services.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFotoService _fotoService;

        public UserManager(IUserRepository userRepository, IFotoService fotoService)
        {
            _userRepository = userRepository;
            _fotoService = fotoService;
        }

        public async Task<Result<UserDto>> AddAsync(UserAddDto userAddDto)
        {
            string? imagePath = null;

            // Eğer base64 formatında fotoğraf gönderildiyse işle
            if (!string.IsNullOrEmpty(userAddDto.ImageBase64))
            {
                if (!_fotoService.IsBase64String(userAddDto.ImageBase64))
                {
                    return Result<UserDto>.FailureResult("Geçersiz fotoğraf formatı. Lütfen geçerli bir base64 formatında fotoğraf gönderin.");
                }

                try
                {
                    imagePath = _fotoService.Upload(userAddDto.ImageBase64, PhotoType.USER_PHOTO);
                }
                catch (Exception ex)
                {
                    return Result<UserDto>.FailureResult($"Fotoğraf yükleme hatası: {ex.Message}");
                }
            }

            var user = new User
            {
                Username = userAddDto.Username,
                Email = userAddDto.Email,
                Password = PasswordHasher.Hash("Default123!"),
                CreatedAt = DateTime.Now,
                IsAdmin = userAddDto.IsAdmin,
                ImagePath = imagePath
            };

            var addedUser = await _userRepository.AddAsync(user);

            // Kullanıcı başarıyla eklendiğinde, geri UserDto dönüyoruz
            var userDto = new UserDto
            {
                Id = addedUser.Id,
                Username = addedUser.Username,
                Email = addedUser.Email,
                IsAdmin = addedUser.IsAdmin,
                ImagePath = addedUser.ImagePath,
                CreatedAt = addedUser.CreatedAt
            };

            return Result<UserDto>.SuccessResult(userDto, "Kullanıcı eklendi.");
        }

        public async Task<Result<object>> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result<object>.FailureResult("Kullanıcı bulunamadı.");

            await _userRepository.DeleteAsync(user);
            return Result<object>.SuccessResult(null, "Kullanıcı silindi.");
        }

        public async Task<Result<List<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                ImagePath = u.ImagePath,
                CreatedAt = u.CreatedAt
            }).ToList();

            return Result<List<UserDto>>.SuccessResult(userDtos);
        }

        public async Task<Result<UserDto>> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return Result<UserDto>.FailureResult("Kullanıcı bulunamadı.");

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                ImagePath = user.ImagePath,
                CreatedAt = user.CreatedAt
            };

            return Result<UserDto>.SuccessResult(dto);
        }

        public async Task<Result<UserDto>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result<UserDto>.FailureResult("Kullanıcı bulunamadı.");

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                ImagePath = "user_photos/" + user.ImagePath,
                CreatedAt = user.CreatedAt
            };

            return Result<UserDto>.SuccessResult(dto);
        }

        public async Task<Result<UserDto>> UpdateAsync(UserUpdateDto userUpdateDto, bool isUpdaterAdmin = false)
        {
            var user = await _userRepository.GetByIdAsync(userUpdateDto.Id);
            if (user == null)
                return Result<UserDto>.FailureResult("Kullanıcı bulunamadı.");

            // Temel kullanıcı bilgilerini her zaman güncelle
            user.Username = userUpdateDto.Username;
            user.Email = userUpdateDto.Email;

            // Admin yetkisi kontrolü
            if (isUpdaterAdmin)
            {
                // JWT token ile gelen kullanıcı admin ise, admin durumunu DTO'dan alarak güncelleyebilir
                user.IsAdmin = userUpdateDto.IsAdmin;
            }
            else
            {
                // Normal kullanıcı kendini admin yapmaya çalışıyorsa hata ver
                if (userUpdateDto.IsAdmin && !user.IsAdmin)
                {
                    return Result<UserDto>.FailureResult("Admin yetkisi verme izniniz yok.");
                }
                // Normal kullanıcılar admin yetkisini değiştiremez, mevcut durum korunur
            }

            // Fotoğraf işleme
            if (!string.IsNullOrEmpty(userUpdateDto.ImageBase64))
            {
                if (!_fotoService.IsBase64String(userUpdateDto.ImageBase64))
                {
                    return Result<UserDto>.FailureResult("Geçersiz fotoğraf formatı. Lütfen geçerli bir base64 formatında fotoğraf gönderin.");
                }

                try
                {
                    string imagePath = _fotoService.Upload(userUpdateDto.ImageBase64, PhotoType.USER_PHOTO);
                    user.ImagePath = imagePath;
                }
                catch (Exception ex)
                {
                    return Result<UserDto>.FailureResult($"Fotoğraf yükleme hatası: {ex.Message}");
                }
            }

            var updatedUser = await _userRepository.UpdateAsync(user);

            // Güncellenmiş kullanıcı verilerini DTO'ya aktar
            var userDto = new UserDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                IsAdmin = updatedUser.IsAdmin,
                ImagePath = updatedUser.ImagePath,
                CreatedAt = updatedUser.CreatedAt
            };

            return Result<UserDto>.SuccessResult(userDto, "Kullanıcı güncellendi.");
        }

        public async Task<Result<User>> AuthenticateAsync(LoginDto dto)
        {
            return await AuthenticateAsync(dto.Email, dto.Password);
        }

        public async Task<Result<User>> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !PasswordHasher.Verify(password, user.Password))
            {
                return Result<User>.FailureResult("Geçersiz email ya da şifre.");
            }

            return Result<User>.SuccessResult(user, "Giriş başarılı.");
        }

        public async Task<Result<User>> RegisterAsync(RegisterDto dto)
        {
            var emailExists = !(await _userRepository.IsEmailUniqueAsync(dto.Email));
            if (emailExists)
                return Result<User>.FailureResult("Bu email ile kayıtlı bir kullanıcı zaten var.");

            string? imagePath = null;

            // Eğer base64 formatında fotoğraf gönderildiyse işle
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                if (!_fotoService.IsBase64String(dto.ImageBase64))
                {
                    return Result<User>.FailureResult("Geçersiz fotoğraf formatı. Lütfen geçerli bir base64 formatında fotoğraf gönderin.");
                }

                try
                {
                    imagePath = _fotoService.Upload(dto.ImageBase64, PhotoType.USER_PHOTO);
                }
                catch (Exception ex)
                {
                    return Result<User>.FailureResult($"Fotoğraf yükleme hatası: {ex.Message}");
                }
            }

            var user = new User
            {
                Email = dto.Email,
                Password = PasswordHasher.Hash(dto.Password),
                Username = dto.Username,
                CreatedAt = DateTime.Now,
                IsAdmin = false,
                ImagePath = imagePath
            };

            var addedUser = await _userRepository.AddAsync(user);
            return Result<User>.SuccessResult(addedUser, "Kayıt başarılı.");
        }
        public async Task<Result<User>> BanUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<User>.FailureResult("Kullanıcı bulunamadı.");

            if (user.IsBlocked)
                return Result<User>.FailureResult("Kullanıcı zaten banlanmış.");

            user.IsBlocked = true;
            
            var updatedUser = await _userRepository.UpdateAsync(user);
            return Result<User>.SuccessResult(updatedUser, "Kullanıcı başarıyla banlandı.");
        }
    }
}
