﻿using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.MongoDB.Share;

namespace Template.Infrastructure.MongoDB.Models
{
    [Collection("Users")]
    public class Users : MainFields
    {
        [Required]
        [Description("User id.")]
        public ObjectId? ID { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("User firstname.")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("User lastname.")]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(20)]
        [Description("User mobile.")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]
        [Description("User email.")]
        public string? Email { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("Password encrypt.")]
        public string? Password { get; set; }
    }
}