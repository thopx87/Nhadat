using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Application2016.Helpers
{
    public static class FileHelper
    {        
        public static string UploadFile(HttpPostedFileBase file, string username, Enums.ImageType type)
        {
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            string fileName = DateTime.Now.Millisecond + file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return "";

            string path = string.Empty;
            switch (type)
            {
                case Enums.ImageType.PRODUCT:
                    path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS + AdminConfigs.DIRSEPARATOR;
                    break;
                case Enums.ImageType.PRODUCT_TEMP:
                    path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS + AdminConfigs.DIRSEPARATOR;
                    break;
                default:
                    break;
            }
            // Check if the directory we are saving to exists
            if (path != string.Empty && !Directory.Exists(path))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(path);
            }

            // Set our full path for saving
            path = path + fileName;

            // Save our file
            file.SaveAs(Path.GetFullPath(path));

            // Save our thumbnail as well
            // ResizeImage(file, 70, 70);

            // Return the filename
            return path;
        }

        public static string UploadFile(HttpPostedFileBase file, string username)
        {
            string imgPath = AdminConfigs.PHYSICAL_PATH + username;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            string fileName = DateTime.Now.Millisecond + file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return "";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(imgPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(imgPath);
            }

            // Set our full path for saving
            string path = imgPath + AdminConfigs.DIRSEPARATOR + fileName;

            // Save our file
            file.SaveAs(Path.GetFullPath(path));

            // Save our thumbnail as well
            ResizeImage(file, 70, 70, username);

            // Return the filename
            return path;
        }

        public static void DeleteFile(string fileName, int productId = 0, string username = "")
        {
            // Don't do anything if there is no name
            if (fileName.Length == 0) return;

            // Set our full path for deleting
            string path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS;

            if (productId > 0)
            {
                path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS;
            }

            if(Directory.Exists(path))
            {
                path += AdminConfigs.DIRSEPARATOR + fileName;
                RemoveFile(path);
            }
            
            //Xóa ảnh resize.
            int width = AdminConfigs.AVATAR_WIDTH;
            int height = AdminConfigs.AVATAR_HEIGHT;
            string pathResize = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + width.ToString() + "_" + height.ToString() + AdminConfigs.DIRSEPARATOR;
            if (Directory.Exists(pathResize))
            {
                pathResize += AdminConfigs.DIRSEPARATOR + fileName;
                RemoveFile(pathResize);
            }
        }

        private static void RemoveFile(string path)
        {
            // Check if our file exists
            if (File.Exists(Path.GetFullPath(path)))
            {
                // Delete our file
                File.Delete(Path.GetFullPath(path));
            }
        }

        public static void ResizeImage(HttpPostedFileBase file, int width, int height)
        {
            string thumbnailDirectory = String.Format(@"{0}{1}{2}", AdminConfigs.PHYSICAL_PATH, AdminConfigs.DIRSEPARATOR, "Thumbnails");

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            // Final path we will save our thumbnail
            string imagePath = String.Format(@"{0}{1}{2}", thumbnailDirectory, AdminConfigs.DIRSEPARATOR, file.FileName);
            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            Image OrigImage = Image.FromStream(file.InputStream);
            // Create a new bitmap with the size of our thumbnail
            Bitmap TempBitmap = new Bitmap(width, height);

            // Create a new image that contains are quality information
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a rectangle and draw the image
            Rectangle imageRectangle = new Rectangle(0, 0, width, height);
            NewImage.DrawImage(OrigImage, imageRectangle);

            // Save the final file
            TempBitmap.Save(stream, OrigImage.RawFormat);

            // Clean up the resources
            NewImage.Dispose();
            TempBitmap.Dispose();
            OrigImage.Dispose();
            stream.Close();
            stream.Dispose();
        }

        public static void ResizeImage(HttpPostedFileBase file, int width, int height, string username)
        {
            string thumbnailDirectory = String.Format(@"{0}{1}{2}", AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR, AdminConfigs.DIRSEPARATOR, "Thumbnails");

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            // Final path we will save our thumbnail
            string imagePath = String.Format(@"{0}{1}{2}", thumbnailDirectory, AdminConfigs.DIRSEPARATOR, file.FileName);
            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            Image OrigImage = Image.FromStream(file.InputStream);
            // Create a new bitmap with the size of our thumbnail
            Bitmap TempBitmap = new Bitmap(width, height);

            // Create a new image that contains are quality information
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a rectangle and draw the image
            Rectangle imageRectangle = new Rectangle(0, 0, width, height);
            NewImage.DrawImage(OrigImage, imageRectangle);

            // Save the final file
            TempBitmap.Save(stream, OrigImage.RawFormat);

            // Clean up the resources
            NewImage.Dispose();
            TempBitmap.Dispose();
            OrigImage.Dispose();
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Lấy danh sách file trong folder temp bao gồm cả đường dẫn
        /// Chú ý: Muốn dùng được hàm này thì phải đăng nhập trước.
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllFileTemp(string username)
        {
            string path = AdminConfigs.PHYSICAL_PATH + username.ToLower() + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS;
            string urlImg = AdminConfigs.IMAGE_PATH + username.ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCT_TEMPS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var arrImg = Directory.GetFiles(path);
            if (arrImg != null)
            {
                string[] result = new string[arrImg.Count()];
                for(int i=0; i<arrImg.Count(); i++){
                    string fileName = new FileInfo(arrImg[i]).Name;
                    result[i] = urlImg + fileName;
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Lấy thông tin file ảnh.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Entities.ProductImage[] GetAllFileInfo(Enums.ImageType type, string username)
        {
            string path = "";
            string imgUrl = "";
            switch (type)
            {
                case Enums.ImageType.PRODUCT:
                    path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS;
                    imgUrl = AdminConfigs.IMAGE_PATH + username.ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCTS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;
                    break;
                case Enums.ImageType.PRODUCT_TEMP:
                    path = AdminConfigs.PHYSICAL_PATH + username + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS;
                    break;
            }
            var arrImg = Directory.GetFiles(path);
            if (arrImg != null)
            {
                Entities.ProductImage[] result = new Entities.ProductImage[arrImg.Count()];
                Entities.ProductImage img;
                for (int i = 0; i < arrImg.Count(); i++)
                {
                    img = new Entities.ProductImage();
                    var fileInfo = new FileInfo(arrImg[i]);
                    img.Text = fileInfo.Name;
                    img.ImageUrl = imgUrl;
                    img.Size = (int)fileInfo.Length;
                    result[i] = img;
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Chuyển file sang folder khác
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="targetFolder"></param>
        /// <returns></returns>
        public static bool MoveFile(string sourceFolder, string targetFolder)
        {
            try
            {
                var arrImg = Directory.GetFiles(sourceFolder, "*.*");
                if (arrImg != null || arrImg.Count() == 0)
                {
                    FileInfo fileInfo;
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    foreach (string file in arrImg)
                    {
                        fileInfo = new FileInfo(file);
                        if (new FileInfo(targetFolder + AdminConfigs.DIRSEPARATOR + fileInfo.Name).Exists == false)
                        {
                            fileInfo.MoveTo(targetFolder + AdminConfigs.DIRSEPARATOR + fileInfo.Name);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// crop image and save to taget folder.
        /// Taget folder: source folder + size folder (width _ height)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="taget"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public static void CropImage(string source)
        {
            int x1 = 0;
            int y1 = 0;
            int width = AdminConfigs.AVATAR_WIDTH;
            int height = AdminConfigs.AVATAR_HEIGHT;
            //Create a rectanagle to represent the cropping area
            Rectangle rect = new Rectangle(x1, y1, width, height);
            
            // Set target folder.
            // Change Folder

            DirectoryInfo d = new DirectoryInfo(source);
            string target_temp = d.Parent.Parent.FullName;
            target_temp += AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS + AdminConfigs.DIRSEPARATOR + width.ToString() + "_" + height.ToString() + AdminConfigs.DIRSEPARATOR;
            // Check folder exists
            if (!Directory.Exists(target_temp))
            {
                Directory.CreateDirectory(target_temp);
            }

            string target = target_temp;

            // Set target file.
            target_temp +="temp_"+ Path.GetFileName(source);
            target += Path.GetFileName(source);
            // Kiểm tra sự tồn tại của file.
            // Nếu đã tồn tại thì thoát luôn.
            if (File.Exists(target_temp))
            {
                return;
            }

            // Resize image before crop.
            ResizeImage(source, target_temp, height, width);

            using (Image img = Image.FromFile(target_temp))
            {
                //now do the crop
                SaveCroppedImage(img, width, height, target);
            }

            // Xóa ảnh temp
            if(File.Exists(target_temp))
            {
                File.Delete(target_temp);
            }
        }
        
        public static void ResizeImage(string OriginalFile, string NewFile, int NewHeight, int NewWidth)
        {
            Image FullsizeImage = Image.FromFile(OriginalFile);
            
            // Trường hợp ảnh không đúng với dự kiến (chiều cao lớn hơn chiều rộng)
            if (NewWidth * FullsizeImage.Height > NewHeight * FullsizeImage.Width)
            {
                NewHeight = NewWidth * FullsizeImage.Height / FullsizeImage.Width;
            }
            else
            {
                NewWidth = NewHeight * FullsizeImage.Width / FullsizeImage.Height;
            }


            //int NewWidth = FullsizeImage.Width * NewHeight / FullsizeImage.Height;
            Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
            FullsizeImage.Dispose();

            Bitmap newBitmap = new Bitmap(NewImage);

            NewImage.Save(NewFile);
        }

        private static bool SaveCroppedImage(Image image, int targetWidth, int targetHeight, string filePath)
        {
            ImageCodecInfo jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/jpeg").First();
            Image finalImage = image;
            System.Drawing.Bitmap bitmap = null;
            try
            {
                int left = 0;
                int top = 0;
                int srcWidth = targetWidth;
                int srcHeight = targetHeight;
                bitmap = new System.Drawing.Bitmap(targetWidth, targetHeight);
                double croppedHeightToWidth = (double)targetHeight / targetWidth;
                double croppedWidthToHeight = (double)targetWidth / targetHeight;

                if (image.Width > image.Height)
                {
                    srcWidth = (int)(Math.Round(image.Height * croppedWidthToHeight));
                    if (srcWidth < image.Width)
                    {
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                    else
                    {
                        srcHeight = (int)Math.Round(image.Height * ((double)image.Width / srcWidth));
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                }
                else
                {
                    srcHeight = (int)(Math.Round(image.Width * croppedHeightToWidth));
                    if (srcHeight < image.Height)
                    {
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                    else
                    {
                        srcWidth = (int)Math.Round(image.Width * ((double)image.Height / srcHeight));
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                }
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
                }
                finalImage = bitmap;
            }
            catch { }
            try
            {
                using (EncoderParameters encParams = new EncoderParameters(1))
                {
                    encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                    //quality should be in the range [0..100] .. 100 for max, 0 for min (0 best compression)
                    finalImage.Save(filePath, jpgInfo, encParams);
                    return true;
                }
            }
            catch { }
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
            return false;
        }

        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

    }
}