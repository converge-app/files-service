using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IFileRepository
    {
        Task<List<File>> Get();
        Task<File> GetById(string id);
        Task<List<File>> GetByProject(string projectId);
        Task<List<File>> GetByFreelancerId(string freelancerId);
        Task<List<File>> GetByProjectAndFreelancer(string projectId, string freelancerId);
        Task<File> Create(File file);
        Task Update(string id, File fileIn);
        Task Remove(File fileIn);
        Task Remove(string id);
        Task<List<File>> GetByProjectId(string projectId);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IMongoCollection<File> _files;

        public FileRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _files = dbContext.Files;
        }

        public async Task<List<File>> Get() => await (await _files.FindAsync(file => true)).ToListAsync();
        public async Task<File> GetById(string id) => await (await _files.FindAsync(file => file.Id == id)).FirstOrDefaultAsync();
        public async Task<List<File>> GetByProject(string projectId) => await (await _files.FindAsync(file => file.ProjectId == projectId)).ToListAsync();
        public async Task<List<File>> GetByFreelancerId(string freelancerId) => await (await _files.FindAsync(file => file.FreelancerId == freelancerId)).ToListAsync();

        public async Task<List<File>> GetByProjectAndFreelancer(string projectId, string freelancerId) => await (
            await _files.FindAsync(
                file => file.ProjectId == projectId && file.FreelancerId == freelancerId)
        ).ToListAsync();

        public async Task<File> Create(File file)
        {
            await _files.InsertOneAsync(file);
            return file;
        }

        public async Task Update(string id, File fileIn) => await _files.ReplaceOneAsync(file => file.Id == id, fileIn);
        public async Task Remove(File fileIn) => await _files.DeleteOneAsync(file => file.Id == fileIn.Id);
        public async Task Remove(string id) => await _files.DeleteOneAsync(file => file.Id == id);
        public async Task<List<File>> GetByProjectId(string projectId) => await (await _files.FindAsync(file => file.ProjectId == projectId)).ToListAsync();
    }
}