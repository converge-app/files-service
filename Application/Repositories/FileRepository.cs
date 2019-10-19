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
        Task<IList<File>> GetByUserId(string userId);
        Task<File> Create(File file);
        Task Update(string id, File fileIn);
        Task Remove(File fileIn);
        Task Remove(string id);
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

        public async Task<IList<File>> GetByUserId(string userId) =>
            await (await _files.FindAsync(file => file.UserId == userId)).ToListAsync();

        public async Task<File> Create(File file)
        {
            await _files.InsertOneAsync(file);
            return file;
        }

        public async Task Update(string id, File fileIn) => await _files.ReplaceOneAsync(file => file.Id == id, fileIn);
        public async Task Remove(File fileIn) => await _files.DeleteOneAsync(file => file.Id == fileIn.Id);
        public async Task Remove(string id) => await _files.DeleteOneAsync(file => file.Id == id);
    }
}