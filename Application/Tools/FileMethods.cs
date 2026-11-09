using Microsoft.AspNetCore.Http;

namespace Application.Tools
{
    public static class FileMethods
    {
        public static void UploadFileToServer(this IFormFile file, string name, string path)
        {
            string saveFoler = Directory.GetCurrentDirectory() + "/wwwroot" + path;
            string savePath = Path.Combine(saveFoler, name);

            if (!Directory.Exists(saveFoler))
            {
                Directory.CreateDirectory(saveFoler);
            }

            savePath.RemoveFileFromServer();

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        public static void UploadSensitiveFileToServer(this IFormFile file, string name, string path)
        {
            string saveFoler = Directory.GetCurrentDirectory() + "/SensitiveFiles" + path;
            string savePath = Path.Combine(saveFoler, name);

            if (!Directory.Exists(saveFoler))
            {
                Directory.CreateDirectory(saveFoler);
            }

            savePath.RemoveFileFromServer();

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        public static void RemoveSensitiveFileFromServer(this string name, string path)
        {
            string fileAddress = Directory.GetCurrentDirectory() + "/SensitiveFiles" + path + name;

            if (File.Exists(fileAddress))
            {
                File.Delete(fileAddress);
            }
        }


        public static string GetSensitiveFilePath(this string fileName, string subFolder)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;

            return Directory.GetCurrentDirectory() + "/SensitiveFiles" + subFolder + "/" + fileName;
        }

        public static void RemoveFileFromServer(this string name, string path)
        {
            string fileAddress = Directory.GetCurrentDirectory() + "/wwwroot" + path + name;

            if (File.Exists(fileAddress))
            {
                File.Delete(fileAddress);
            }
        }

        public static void RemoveFileFromServer(this string fileAddress)
        {
            if (File.Exists(fileAddress))
            {
                File.Delete(fileAddress);
            }
        }

        // ۱. تعریف پسوندهای مجاز
        private static readonly HashSet<string> AllowedExtensions = new() { ".jpg", ".jpeg", ".png", ".webp" };

        // ۲. تعریف امضاهای بایتی واقعی تصاویر (Magic Numbers)
        private static readonly Dictionary<string, byte[]> ImageSignatures = new() { { ".jpeg", new byte[] { 0xFF, 0xD8, 0xFF } }, { ".jpg", new byte[] { 0xFF, 0xD8, 0xFF } }, { ".png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } }, { ".webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } } };

        public static bool IsImage(this IFormFile postedFile)
        {
            if (postedFile == null || postedFile.Length == 0) return false;

            // -------------------------------------------
            // الف) بررسی ظاهری پسوند فایل
            // -------------------------------------------
            var extension = Path.GetExtension(postedFile.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension)) return false;

            // -------------------------------------------
            // ب) بررسی محتوایی امضای بایت‌های فایل (ضد هک)
            // -------------------------------------------
            try
            {
                // خواندن چند بایت اول فایل بدون لود کردن کل فایل در حافظه
                using var stream = postedFile.OpenReadStream();
                using var reader = new BinaryReader(stream);

                // بزرگترین امضای ما ۴ بایت است
                byte[] fileBytes = reader.ReadBytes(4);

                if (!ImageSignatures.TryGetValue(extension, out var signature))
                    return false;

                // مقایسه بایت‌های فایل با امضای استاندارد
                for (int i = 0; i < signature.Length; i++)
                {
                    if (fileBytes[i] != signature[i]) return false;
                }

                return true;
            }
            catch
            {
                return false; // اگر فایل خراب بود یا خوانده نشد
            }
        }

        public static bool IsCompressedFile(this IFormFile postedFile, bool checkFile = true)
        {
            if (checkFile)
            {
                //-------------------------------------------
                //  Check the compressed file mime types
                //-------------------------------------------
                var allowedMimeTypes = new[]
                {
            "application/x-rar-compressed",
            "application/x-rar",
            "application/zip",
            "application/x-zip-compressed",
            "multipart/x-zip"
        };

                if (!allowedMimeTypes.Contains(postedFile.ContentType.ToLower()))
                {
                    return false;
                }

                //-------------------------------------------
                //  Check the file extension
                //-------------------------------------------
                var allowedExtensions = new[] { ".zip", ".rar" };
                var fileExtension = Path.GetExtension(postedFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return false;
                }

                return true;
            }

            return true;
        }

        public static string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".pdf":
                    return "application/pdf";
                case ".mp3":
                    return "audio/mpeg";       // MP3
                case ".wav":
                    return "audio/wav";        // WAV
                case ".ogg":
                    return "audio/ogg";        // OGG
                case ".aac":
                    return "audio/aac";        // AAC
                case ".flac":
                    return "audio/flac";       // FLAC
                case ".aiff":
                    return "audio/x-aiff";     // AIFF
                case ".wma":
                    return "audio/x-ms-wma";   // WMA
                case ".webm":
                    return "audio/webm";       // WebM
                case ".3gp":
                    return "audio/3gpp";       // 3GPP
                case ".3g2":
                    return "audio/3gpp2";      // 3GPP2
                case ".mka":
                    return "audio/x-matroska"; // Matroska
                default:
                    return "unknown";
            }
        }

        public static bool IsAllowedMimeType(this string filePath)
        {
            var mimeType = GetMimeType(filePath);

            string[] allowedMimeTypes = {
                "image/jpeg",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/pdf",
                "audio/mpeg",
                "audio/wav",
                "audio/ogg",
                "audio/aac",
                "audio/flac",
                "audio/x-aiff",
                "audio/x-ms-wma",
                "audio/webm",
                "audio/3gpp",
                "audio/3gpp2",
                "audio/x-matroska"
            };

            return Array.Exists(allowedMimeTypes, type => type == mimeType);
        }
    }
}
