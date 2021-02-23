using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MSSA_Covid.Data.Interfaces
{
    interface IKioskBlobFile<T>
    {
        string BlobPath { get; set; }
        Task<bool> UploadFileAsync(IFormFile file, T entity);
        Task<List<string>> GetAllAsync();
        Task<bool> VerifyFileAsync(IFormFile file, T entity);
        Task<bool> DeleteBlobAsync(IFormFile file, T entity);
    }
}
