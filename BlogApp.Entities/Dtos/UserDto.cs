﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Entities.Dtos
{
    public class UserDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "ID negatif olamaz.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam, alt çizgi ve tire içerebilir.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Kullanıcının admin olup olmadığını belirtir.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// Kullanıcının profil fotoğrafı yolu
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Kullanıcının oluşturulma tarihi
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
    }
}
