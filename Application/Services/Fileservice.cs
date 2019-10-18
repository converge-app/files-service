using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Utility.ClientLibrary;
using Application.Utility.ClientLibrary.Project;
using Application.Utility.Exception;
using Application.Utility.Models;
using Newtonsoft.Json;

namespace Application.Services
{
    public interface IFileservice
    {
        Task<File> Open(File file);
        Task<bool> Accept(File file, string authorizationToken);
    }

    public class Fileservice : IFileservice
    {
        private readonly IFileRepository _fileRepository;
        private readonly IClient _client;

        public Fileservice(IFileRepository fileRepository, IClient client)
        {
            _fileRepository = fileRepository;
            _client = client;
        }

        public async Task<File> Open(File file)
        {
            var project = await _client.GetProjectAsync(file.ProjectId);
            if (project == null) throw new InvalidFile();

            var createdFile = await _fileRepository.Create(file);

            return createdFile ??
                throw new InvalidFile();
        }

        public async Task<bool> Accept(File file, string authorizationToken)
        {
            var project = await _client.GetProjectAsync(file.ProjectId);
            if (project == null) throw new InvalidFile("projectId invalid");

            project.FreelancerId = file.FreelancerId;

            return await _client.UpdateProjectAsync(authorizationToken, project);
        }
    }
}