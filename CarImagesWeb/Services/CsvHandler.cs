using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace CarImagesWeb.Services
{
    public interface ICsvHandler
    {
        /// Generic method to read a csv file and return a list of objects.
        Task<TOut> ReadCsvAsync<TIn, TOut>();
        
        /// Generic method to write a list of objects to a csv file.
        Task<byte[]> WriteCsvAsync<T>(List<T> records);

    }

    public class CsvHandler : ICsvHandler
    {
        public Task<TOut> ReadCsvAsync<TIn, TOut>()
        {
            throw new System.NotImplementedException();
        }

        public async Task<byte[]> WriteCsvAsync<T>(List<T> records)
        {
            // create new file stream
            var fileOutput = new MemoryStream();
            await using var writer = new StreamWriter(fileOutput);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write the header row by looping through the properties of the object
            foreach (var property in typeof(T).GetProperties())
            {
                csv.WriteField(property.Name);
            }
            await csv.NextRecordAsync();

            // Write the data rows
            foreach (var record in records)
            {
                // Loop through the properties of the object
                foreach (var property in typeof(T).GetProperties())
                {
                    // Get the value of the property
                    var value = property.GetValue(record);
                    csv.WriteField(value as string ?? string.Empty);
                }
                await csv.NextRecordAsync();
            }
            //return memory stream from csv writer
            return fileOutput.ToArray();
        }
    }
}