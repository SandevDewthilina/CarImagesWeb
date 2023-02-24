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
    }

    public class CountryHandler : ICountryHandler
    {
        private readonly ICountryRepository _repository;

        public CountryHandler(ICountryRepository repository)
        {
            _repository = repository;
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
    }
}