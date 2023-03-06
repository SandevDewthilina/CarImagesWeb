using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<bool> IsTagInRole(int tag, UserRole role);
        Task<bool> IsTagInRole(Tag tag, UserRole role);
        Task<UserRoleTagMapping> AddTagToRoleAsync(Tag tag, UserRole role,bool allowUpload, bool allowDownload);
        Task RemoveTagFromRoleAsync(Tag tag, UserRole role);
        Task<UserRoleTagMapping> UpdateTagToRole(Tag tag, UserRole role, bool allowUpload, bool allowDownload);
        Task<List<Tag>> TagsInRole(UserRole role);
        Task<Tag> GetTagAsync(int tagId);
        Task<IEnumerable<Tag>> GetTagsForRole(string userRole);
        Task<IEnumerable<Tag>> GetTagsForRoles(List<string> userRoles);
    }

    public class TagsHandler : ITagsHandler
    {
        private readonly ITagRepository _tagRepository;
        private readonly IRoleTagRepository _roleTagRepository;
        private readonly ICountryRepository _countryRepository;

        public TagsHandler(ITagRepository tagRepository, IRoleTagRepository roleTagRepository, ICountryRepository countryRepository)
        {
            _tagRepository = tagRepository;
            _roleTagRepository = roleTagRepository;
            _countryRepository = countryRepository;
        }
        
        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }
        
        public async Task<Tag> GetTagToUpload(ImageUploadDto imageUploadDto)
        {
            //get the tag id based on the asset type
            var tagId = Enum.Parse<AssetType>(imageUploadDto.ImageCategory) switch
            {
                AssetType.Vehicle => int.Parse(imageUploadDto.VehicleTagId),
                AssetType.Container => int.Parse(imageUploadDto.ContainerTagId),
                _ => throw new NotImplementedException()
            };

            return await _tagRepository.GetAsync(t => t.Id == tagId);
        }

        public async Task<bool> IsTagInRole(int tagId, UserRole role)
        {
            return await _roleTagRepository.GetAsync(
                rt => rt.UserRole.Id == role.Id && rt.TagId == tagId) != null;
        }

        public async Task<bool> IsTagInRole(Tag tag, UserRole role)
        {
            return await _roleTagRepository.GetAsync(
                rt => rt.UserRole.Id == role.Id && rt.TagId == tag.Id) != null;
        }

        public async Task<UserRoleTagMapping> AddTagToRoleAsync(Tag tag, UserRole role, bool allowUpload, bool allowDownload)
        {
            var roleTag = new UserRoleTagMapping(tag, role, allowUpload, allowDownload);
            return await _roleTagRepository.AddAsync(roleTag);
        }

        public async Task RemoveTagFromRoleAsync(Tag tag, UserRole role)
        {
            var roleTag = await _roleTagRepository.GetAsync(
                rt => rt.UserRoleId == role.Id && rt.TagId == tag.Id);
            await _roleTagRepository.DeleteAsync(roleTag);
        }

        public async Task<UserRoleTagMapping> UpdateTagToRole(Tag tag, UserRole role, bool allowUpload, bool allowDownload)
        {
            var roleTag = await _roleTagRepository.GetAsync(m => m.UserRoleId == role.Id && m.TagId == tag.Id);
            roleTag.AllowDownload = allowDownload;
            roleTag.AllowUpload = allowUpload;
            return await _roleTagRepository.UpdateAsync(roleTag);
        }

        public async Task<List<Tag>> TagsInRole(UserRole role)
        {
            var roleTags = await _roleTagRepository.FindAsync(
                rt => rt.UserRoleId == role.Id);
            return roleTags.Select(r => r.Tag).ToList();
        }

        public async Task<Tag> GetTagAsync(int tagId)
        {
            return await _tagRepository.GetAsync(t => t.Id == tagId);
        }

        public async Task<IEnumerable<Tag>> GetTagsForRole(string userRole)
        {
            var roleTags = await _roleTagRepository
                .FindAsync(rt => rt.UserRole.Name == userRole);
            return roleTags.Select(rt => rt.Tag);
        }

        public async Task<IEnumerable<Tag>> GetTagsForRoles(List<string> userRoles)
        {
            // get all tags for each role
            // add to the list of tags if it is not already in the list
            var tags = new List<Tag>();
            foreach (var userRole in userRoles)
            {
                var roleTags = await GetTagsForRole(userRole);
                foreach (var roleTag in roleTags)
                {
                    if (!tags.Contains(roleTag))
                    {
                        var country = await _countryRepository.GetAsync(c => c.Id == roleTag.CountryId);
                        roleTag.Country = new Country() {Code = country.Code, Id = country.Id, Name = country.Name};
                        tags.Add(roleTag);
                    }
                }
            }

            return tags;
        }
    }
}