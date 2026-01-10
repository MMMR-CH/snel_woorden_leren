using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace UnicoStudio.UnicoLibs.Utilities
{
    public static class GZip
    {
        public static string CompressData(string dataToCompress)
        {
            if (string.IsNullOrEmpty(dataToCompress))
                return null;

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToCompress);

            using (MemoryStream targetStream = new MemoryStream())
            {
                using (MemoryStream sourceStream = new MemoryStream(dataBytes))
                {
                    using (GZipStream compressor = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        compressor.Write(sourceStream.ToArray(), 0, (int)sourceStream.Length);
                    }

                    return Convert.ToBase64String(targetStream.ToArray());
                }
            }
        }

        public static string DecompressData(string compressedData)
        {
            if (string.IsNullOrEmpty(compressedData))
                return null;

            byte[] dataBytes = Convert.FromBase64String(compressedData);

            using (MemoryStream targetStream = new MemoryStream())
            {
                using (MemoryStream sourceStream = new MemoryStream(dataBytes))
                {
                    using (GZipStream decompressor = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(targetStream);
                    }

                    return Encoding.UTF8.GetString(targetStream.ToArray());
                }
            }
        }

        public static Exception SaveCompressedObject(byte[] data, string destinationPath)
        {
            try
            {
                using (MemoryStream sourceStream = new MemoryStream(data))
                {
                    using (FileStream targetStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        sourceStream.CopyTo(targetStream);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static Exception LoadCompressedObject(string sourcePath, out byte[] data)
        {
            try
            {
                using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
                {
                    using (MemoryStream targetStream = new MemoryStream())
                    {
                        sourceStream.CopyTo(targetStream);
                        data = targetStream.ToArray();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                data = null;
                return ex;
            }
        }

        public static void CompressFile(string sourcePath, string destinationPath)
        {
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(destinationPath, FileMode.Create))
                {
                    using (GZipStream compressor = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressor);
                    }
                }
            }
        }

        public static void DecompressFile(string sourcePath, string destinationPath)
        {
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(destinationPath, FileMode.Create))
                {
                    using (GZipStream decompressor = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(targetStream);
                    }
                }
            }
        }
    }
}