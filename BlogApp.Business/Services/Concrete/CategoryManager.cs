using BlogApp.Business.Services.Abstract;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using BlogApp.Entities.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Business.Services.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<List<Category>>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Result<List<Category>>.SuccessResult(categories.ToList());
        }

        public async Task<Result<Category>> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return Result<Category>.FailureResult("Kategori bulunamadı.");

            return Result<Category>.SuccessResult(category);
        }

        public async Task<Result<Category>> AddAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return Result<Category>.FailureResult("Kategori adı boş olamaz.");

            var addedCategory = await _categoryRepository.AddAsync(category);
            return Result<Category>.SuccessResult(addedCategory, "Kategori başarıyla eklendi.");
        }

        public async Task<Result<Category>> UpdateAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return Result<Category>.FailureResult("Kategori adı boş olamaz.");

            var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (existingCategory == null)
                return Result<Category>.FailureResult("Güncellenecek kategori bulunamadı.");

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return Result<Category>.SuccessResult(updatedCategory, "Kategori başarıyla güncellendi.");
        }

        public async Task<Result<object>> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return Result<object>.FailureResult("Silinecek kategori bulunamadı.");

            await _categoryRepository.DeleteAsync(category);
            return Result<object>.SuccessResult(null, "Kategori başarıyla silindi.");
        }
    }
} 