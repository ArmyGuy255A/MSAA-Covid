using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MSSA_Covid.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MSSA_Covid.Services
{
    public class BlobStorageService
    {
        string connectionString = string.Empty;
        public IConfiguration _configuration { get; }

        public BlobContainerClient container;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            this.container = new BlobContainerClient(_configuration.GetConnectionString("BlobStorage"), _configuration.GetValue<string>("AzureStorage:UploadBlobName"));
            container.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }

        public string UploadFileToBlob(IFormFile fileData)
        {
            try
            {
                var _task = Task.Run(() => this.UploadFileToBlobAsync(fileData, null));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }

        //    public async void DeleteBlobData(string fileUrl)
        //    {
        //        Uri uriObj = new Uri(fileUrl);
        //        string BlobName = Path.GetFileName(uriObj.LocalPath);

        //        BlobClient blobClient - 

        //        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
        //        CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //        string strContainerName = "uploads";
        //        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

        //        string pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";
        //        CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);
        //        // get block blob refarence    
        //        CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

        //        // delete blob from container        
        //        await blockBlob.DeleteAsync();
        //    }


        //private string GenerateFileName(string fileName)
        //{
        //    string strFileName = string.Empty;
        //    string[] strName = fileName.Split('.');
        //    strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
        //    return strFileName;
        //}

        public async Task<string> UploadFileToBlobAsync(IFormFile fileData, string path)
        {
            try
            {
                string fileName = String.Format("{0}/{1}",path, fileData.FileName);
                if (fileName != null && fileData != null)
                {
                    BlobClient blob = container.GetBlobClient(fileName);
                    using (Stream stream = fileData.OpenReadStream())
                    {
                        await blob.UploadAsync(stream);
                    }
                    BlobProperties properties = await blob.GetPropertiesAsync();
                    if (properties.ContentLength == fileData.Length)
                    {
                        return blob.Uri.AbsoluteUri;
                    } else
                    {
                        return null;
                    }
                    //Assert.AreEqual(fileData.Length, properties.ContentLength);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }

        public async Task<List<string>> GetAllBlobsAsync(string path)
        {
            try
            {
                List<string> uris = new List<string>();
                await foreach (BlobItem blob in container.GetBlobsAsync(prefix: path))
                {
                    uris.Add(String.Format("{0}/{1}", container.Uri.AbsoluteUri, blob.Name));
                }
                return uris;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }

        public async Task<BlobItem> GetBlobAsync(string path)
        {
            try
            {
                List<BlobItem> blobItems = new List<BlobItem>();
                await foreach (BlobItem blob in container.GetBlobsAsync(prefix: path))
                {
                    blobItems.Add(blob);
                }

                if (blobItems.Count > 0)
                {
                    return blobItems.FirstOrDefault();
                } else {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }

        public async Task<bool> VerifyBlobAsync(string path)
        {
            var blobItem = await GetBlobAsync(path);
            if (null == blobItem)
            {
                return false;
            } else
            {
                return true;
            }
        }

        public async Task<bool> VerifyBlobAsync(string path, IFormFile file)
        {
            var blobItem = await GetBlobAsync(path);
            if (null == blobItem)
            {
                return false;
            }
            if (file.Length == blobItem.Properties.ContentLength)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlobAsync(string path)
        {
            BlobItem item = await GetBlobAsync(path);
            if (null == item)
            {
                return true;
            }
            return await container.DeleteBlobIfExistsAsync(item.Name);
        }

        public async Task<bool> DeleteBlobAsync(KioskFile file)
        {
            if (null == file || file.BlobUri.Length == 0)
            {
                return true;
            }

            Uri url = new Uri(file.BlobUri);
            string path = url.AbsolutePath.Replace(container.Name, "").TrimStart('/');
            string decodedPath = HttpUtility.UrlDecode(path);

            return await DeleteBlobAsync(decodedPath);

        }
    }
}
