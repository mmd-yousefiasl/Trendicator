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

        public static bool IsImage(this IFormFile postedFile, bool checkImage = true)
        {
            if (checkImage)
            {
                //-------------------------------------------
                //  Check the image mime types
                //-------------------------------------------
                if (postedFile.ContentType.ToLower() != "image/jpg" &&
                    postedFile.ContentType.ToLower() != "image/jpeg" &&
                    postedFile.ContentType.ToLower() != "image/x-png" &&
                    postedFile.ContentType.ToLower() != "image/png" &&
                    postedFile.ContentType.ToLower() != "image/svg+xml" &&
                    postedFile.ContentType.ToLower() != "image/webp")
                {
                    return false;
                }

                //-------------------------------------------
                //  Check the image extension
                //-------------------------------------------
                string extension = Path.GetExtension(postedFile.FileName).ToLower();
                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png" &&
                    extension != ".svg" &&
                    extension != ".webp")
                {
                    return false;
                }

                return true;
            }

            return true;
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
