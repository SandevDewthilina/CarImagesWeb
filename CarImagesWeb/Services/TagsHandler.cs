using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;

namespace CarImagesWeb.Services
{
    public interface ITagsHandler
    {
        Task<IEnumerable<Tag>> GetTagsAsync();
    }
    
    public class TagsHandler : ITagsHandler
    {
        private readonly ITagRepository _repository;

        public TagsHandler(ITagRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}