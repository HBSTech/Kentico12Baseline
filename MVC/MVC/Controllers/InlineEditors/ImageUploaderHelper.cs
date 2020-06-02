using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;

using CMS.IO;

namespace Generic.Controllers.InlineEditors
{
    /// <summary>
    /// Helper methods for image uploader.
    /// </summary>
    public static class ImageUploaderHelper
    {
        private static readonly HashSet<string> allowedExtensions = new HashSet<string>(new[]
        {
            ".bmp",
            ".gif",
            ".ico",
            ".png",
            ".wmf",
            ".jpg",
            ".jpeg",
            ".tiff",
            ".tif"
        }, StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Uploads given file.
        /// </summary>
        /// <param name="file">Posted file.</param>
        /// <param name="storeFile">Action to store the file.</param>
        public static Guid Upload(HttpPostedFileWrapper file, Func<string, Guid> storeFile)
        {
            var imagePath = ImageUploaderHelper.GetTempFilePath(file);

            byte[] data = new byte[file.ContentLength];
            file.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
            file.InputStream.Read(data, 0, file.ContentLength);

            File.WriteAllBytes(imagePath, data);

            var fileGuid = storeFile(imagePath);

            File.Delete(imagePath);

            return fileGuid;
        }


        private static string GetTempFilePath(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("Cannot upload file without file name.");
            }

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName)))
            {
                throw new InvalidOperationException("Cannot upload file of this type.");
            }

            var directoryPath = EnsureUploadDirectory();

            return Path.Combine(directoryPath, fileName);
        }


        private static string EnsureUploadDirectory()
        {
            var directoryPath = $"{HostingEnvironment.MapPath(@"~\")}App_Data\\Temp\\ImageUploader";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }
}