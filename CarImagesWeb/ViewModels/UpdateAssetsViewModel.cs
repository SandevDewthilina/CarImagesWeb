using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.ViewModels
{
    public class UpdateAssetsViewModel
    {
        [Required]
        [Display(Name = "Sheet")]
        [SheetFile]
        public IFormFile File { get; set; }
    }

    public class SheetFile : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if the file is null
            if (!(value is IFormFile file))
            {
                return new ValidationResult("A file is required.");  
            }
            // Check if the file extension is .xlsx or .csv
            var fileExtension = Path.GetExtension(file.FileName);
            return fileExtension == ".csv" ? ValidationResult.Success  
                : new ValidationResult("Only csv files are supported.");
        }
    }
}