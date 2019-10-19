using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using static Google.Apis.Auth.OAuth2.GoogleCredential;
using File = Application.Models.Entities.File;

namespace Application.Services
{
    public interface IFileService
    {
        Task<File> UploadFile(File file, IFormFile formFile);
    }

    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private GoogleCredential _credentials;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            string base64ServiceCredentials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            string jsonServiceCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64ServiceCredentials));
            _credentials = FromJson(jsonServiceCredentials);
        }

        public async Task<File> UploadFile(File file, IFormFile formFile)
        {
            using(var storageClient = await StorageClient.CreateAsync(_credentials))
            {
                var disposition = formFile.ContentDisposition;
                var imageObject = await storageClient.UploadObjectAsync(
                    "converge-bucket",
                    file.Id + Path.GetExtension(formFile.FileName),
                    formFile.ContentType,
                    formFile.OpenReadStream(),
                    new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
                );

                file.BucketLink = imageObject.MediaLink;

                return await _fileRepository.Create(file);;
            }
        }
    }
}