using System.IO;
using System.Threading.Tasks;
using Core.Business.File;
using Core.Business.Settings;
using Core.Business.Url;
using Core.Business.Websites;
using SoundInTheory.DynamicImage.Fluent;

namespace Core.Business.Thumbnail
{
    public class ThumbnailService
    {
        private const int Width = 800;
        private const int Height = 600;

        private const int MaxAmountParallel = 5;

        private readonly WebsiteService _websiteService;

        public ThumbnailService()
        {
            _websiteService = new WebsiteService();
        }

        public void Start()
        {
            Parallel.ForEach(_websiteService.GetAllWebsites(includeBanned: false),
                new ParallelOptions { MaxDegreeOfParallelism = MaxAmountParallel },
                CreateScreenShot);
        }

        public void CreateScreenShot(int websiteId)
        {
            var website = _websiteService.GetWebsite(websiteId);

            Task.Run(() => CreateScreenShot(website));
        }

        private void CreateScreenShot(Database.Entities.Websites website)
        {
            string directoryPath = string.Format("{0}Thumbnails\\{1}", FileService.GetBaseDirectory(),
                  website.Users.Username);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filepath = string.Format("{0}\\{1}.jpeg", directoryPath, UrlHelpers.GetHost(website.Url, false));

            new CompositionBuilder().WithLayer(new WebsiteScreenshotLayerBuilder().WebsiteUrl(website.Url).WithFilter(FilterBuilder.Crop.To(Width, Height))).SaveTo(filepath);
            
            _websiteService.UpdateThumbnail(website.Id, Path.GetFileName(filepath));
        }
    }
}
