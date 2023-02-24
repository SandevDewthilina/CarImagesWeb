using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.ViewModels
{
    public class UpdateAssetsViewModel
    {
        [SheetFile]
        public IFormFile File { get; set; }

        public bool IsReset { get; set; }
        
        [SheetFile]
        public IFormFile DeleteFile { get; set; }
    }

    public class SheetFile : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file)) 
                //If the file is null continue the validation to be successful
                return ValidationResult.Success;
            // Check if the file extension is .xlsx or .csv
            var fileExtension = Path.GetExtension(file.FileName);
            return fileExtension == ".csv"
                ? ValidationResult.Success
                : new ValidationResult("Only csv files are supported.");
        }
    }
}