using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;

namespace ToDoApp.Models
{
    public class ToDo
    {
   
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public bool IsDone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Add this:
        [ValidateNever]
        public string? UserId { get; set; }

        // Navigation property
        [ValidateNever]
        public ApplicationUser? User { get; set; }
    }
}








