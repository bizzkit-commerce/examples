using Bizzkit.Examples.Common;
using Bizzkit.Sdk.Dam;

// WARNING: This is DEMO code and is not intended for production use. 
// Please review and modify before using in any production environment.
namespace BizzkitTraining
{
    public static class DamHelper
    {
        public static async Task<Guid> CreateFolder(Guid root, string name, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var folder = await client.Folders_CreateAsync(culture, new CreateFolderModel { Name = name, ParentId = root });
            return folder;
        }

        public static async Task<CreatedSubscription> CreateWebHook(EventType eventType, string target, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            return await client.WebHooks_SubscribeAsync(eventType, new WebHookSubscriptionArguments
            {
                Target = target
            });
        }

        public static async Task DeleteFolder(string name, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var response = await FolderExistsGetId(name, culture, config);
            if (!response.exists)
                throw new Exception(name + " does not exitst");
            await client.Folders_DeleteAsync(response.guid!.Value, culture);
        }

        public static async Task<PagedResultOfFiles> FilesInFolder(Guid folder, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var files = await client.Folders_FilesAsync(folder, 1, int.MaxValue, SortOptions.AlphabeticalAscending, culture);
            return files;
        }

        public static async Task<bool> FolderExists(string name, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var folder = await client.Folders_ListByNameAsync(name, null, culture);
            return folder.Count() > 0;
        }

        public static async Task<(bool exists, Guid? guid)> FolderExistsGetId(string name, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var folder = await client.Folders_ListByNameAsync(name, null, culture);
            if (folder.Count() > 0)
                return (true, folder.First().Id);
            else
                return (false, null);
        }

        public static async Task<PagedResultOfFolders> FoldersUnderRoot(Guid root, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var folders = await client.Folders_ChildrenAsync(root, 1, int.MaxValue, culture);
            return folders;

        }

        public static async Task<IEnumerable<CachedFileDetailsResult>> GetFile100(Guid file, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var request = new List<CachedFileInfoParameter>(){
                new CachedFileInfoParameter (){
                    FileId = file,
                    Height = 100,
                    ImageType = CachedFileInfoParameterImageType.Png,
                    Transformation = CachedFileInfoParameterTransformation.Resize
                }
            };
            var response = await client.Files_CachedDetailsAsync(culture, request);
            return response;
        }

        public static async Task<IEnumerable<CachedFileDetailsResult>> GetFilePredfinedSetting(Guid file, string culture, string predefinedSettingName, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var preDefinedSetting = await client.PredefinedSettings_SingleByNameAsync(predefinedSettingName);
            var request = new List<CachedFileInfoParameter>(){
                new CachedFileInfoParameter (){
                    FileId = file,
                    PredefinedSettingId = preDefinedSetting.Id,
                    Transformation = CachedFileInfoParameterTransformation.PredefinedSetting
                }
            };
            var response = await client.Files_CachedDetailsAsync(culture, request);
            return response;
        }

        public static async Task<IEnumerable<CachedFileDetailsResult>> GetFirstFileWithCropping(string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var allImages = await client.Files_KnownOriginalAndPredefinedSettingCachedEntriesAsync(1, int.MaxValue, culture);
            var file = allImages.PageItems.ToList().FirstOrDefault(f => f.CroppingId != null);
            var request = new List<CachedFileInfoParameter>(){
                new CachedFileInfoParameter (){
                    FileId = file!.FileId,
                    CroppingId = file.CroppingId
                }
            };
            var response = await client.Files_CachedDetailsAsync(culture, request);
            return response;
        }

        public static async Task<Guid> GetRootFolder(string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            var root = await client.Folders_RootAsync(culture);
            return root;
        }
        public static async Task<ImportFileResult> UploadFile(Guid folder, string filePath, string culture, DomainConfig config)
        {
            var client = await BizzkitHelper.GetDamClientFactory(config);
            ImportFileFromByteDataParameters request = new ImportFileFromByteDataParameters();
            request.FileData = await System.IO.File.ReadAllBytesAsync(filePath);
            request.FileName = System.IO.Path.GetFileName(filePath);
            request.Description = System.IO.Path.GetFileName(filePath);
            request.ParentFolderId = folder;
            var newFile = await client.Files_ImportFromByteDataAsync(culture, request);
            return newFile;
        }
    }
}