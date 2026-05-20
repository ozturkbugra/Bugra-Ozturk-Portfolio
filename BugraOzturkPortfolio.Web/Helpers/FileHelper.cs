namespace BugraOzturkPortfolio.Web.Helpers
{
    public static class FileHelper
    {
        public static async Task<string?> UploadImageAsync(IFormFile? file, string webRootPath, string folderName = "uploads")
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                var targetFolder = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(targetFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"/{folderName}/{uniqueFileName}";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}