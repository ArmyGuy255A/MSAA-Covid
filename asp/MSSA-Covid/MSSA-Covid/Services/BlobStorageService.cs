using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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


        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }

        public async Task<string> UploadFileToBlobAsync(IFormFile fileData, string path)
        {
            try
            {
                string fileName = this.GenerateFileName(String.Format("{0}/{1}",path, fileData.FileName));
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
    }
}
