using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;

namespace CarImagesWeb.Services
{
    public interface ITagsHandler
    {
        /// <summary>
        ///     Get all tags from the database
        /// </summary>
        /// <returns></returns>
        Task<List<Tag>> GetTagsAsync();

        /// <summary>
        ///     Search the database for the tag from the image upload dto.
        ///     Throws an exception if the tag type is not implemented.
        /// </summary>
        /// <param name="imageUploadDto">
        ///     The image upload dto contains the asset type and tag id.
        /// </param>
        /// <returns></returns>
        Task<Tag> GetTagToUpload(ImageUploadDto imageUploadDto);
    }

    public class TagsHandler : ITagsHandler
    {
        private readonly ITagRepository _repository;

        public TagsHandler(ITagRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        public async Task<Tag> GetTagToUpload(ImageUploadDto imageUploadDto)
        {
            //get the tag id based on the asset type
            var tagId = Enum.Parse<AssetType>(imageUploadDto.ImageCategory) switch
            {
                AssetType.Vehicle => int.Parse(imageUploadDto.VehicleTagId),
                AssetType.Container => int.Parse(imageUploadDto.ContainerTagId),
                _ => throw new NotImplementedException()
            };

            return await _repository.GetAsync(t => t.Id == tagId);
        }
    }
}