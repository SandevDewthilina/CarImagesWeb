using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;

namespace CarImagesWeb.Services
{
    public interface ICountryHandler
    {
        Task<IEnumerable<string>> GetCountryCodesAsync();
        Task<Country> GetCountryFromCode(string countryCode);
        Task<IEnumerable<Country>> GetCountryForRoles(List<string> userRoles);
    }

    public class CountryHandler : ICountryHandler
    {
        private readonly ICountryRepository _repository;
        private readonly ITagsHandler _tagsHandler;

        public CountryHandler(ICountryRepository repository, ITagsHandler tagsHandler)
        {
            _repository = repository;
            _tagsHandler = tagsHandler;
        }

        public async Task<IEnumerable<string>> GetCountryCodesAsync()
        {
            var countries = await _repository.GetAllAsync();
            return countries.Select(country => country.Code).ToList();
        }

        public Task<Country> GetCountryFromCode(string countryCode)
        {
            return _repository.GetAsync(c => c.Code == countryCode);
        }

        public async Task<IEnumerable<Country>> GetCountryForRoles(List<string> userRoles)
        {
            var tagForRoles = await _tagsHandler.GetTagsForRoles(userRoles);
            var countryList = new List<Country>();
            foreach (Tag tag in tagForRoles)
            {
                if (!countryList.Any(c => c.Id == tag.CountryId))
                {
                    countryList.Add(tag.Country);
                }
            }
            return countryList;
        }
    }
}