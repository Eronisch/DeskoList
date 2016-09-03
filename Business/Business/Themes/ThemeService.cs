using System.Collections.Generic;
using Database;
using Database.Entities;

namespace Core.Business.Themes
{
    public class ThemeService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public ThemeService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        public Database.Entities.Themes AddTheme(string themeName, string description, string authorName, string authorUrl, string folderName, string image,
            string version)
        {
            var theme = new Database.Entities.Themes
            {
                AuthorName = authorName,
                Description = description,
                AuthorUrl = authorUrl,
                FolderName = folderName,
                ThemeName = themeName,
                Image = image,
                Version = version
            };

            _unitOfWorkRepository.ThemesRepository.AddTheme(theme);

            _unitOfWorkRepository.SaveChanges();

            return theme;
        }

        public Database.Entities.Themes AddTheme(string themeName, string description, string authorName, string authorUrl,
         string folderName, string version, string image, string updateUrl)
        {
            var theme = _unitOfWorkRepository.ThemesRepository.AddTheme(new Database.Entities.Themes
            {
                AuthorName = authorName,
                AuthorUrl = authorUrl,
                Description = description,
                FolderName = folderName,
                Image = image,
                ThemeName = themeName,
                Version = version,
                UpdateUrl = updateUrl
            });

            _unitOfWorkRepository.SaveChanges();

            return theme;
        }

        public IEnumerable<WidgetsThemeSection> GetThemeSections(int themeId)
        {
            return _unitOfWorkRepository.ThemeSectionRepository.GetByThemeId(themeId);
        }

        public IEnumerable<Database.Entities.Themes> GetThemes()
        {
            return _unitOfWorkRepository.ThemesRepository.GetAll();
        }

        public void AddThemeSection(string friendlyName, string codeName, int themeId)
        {
            _unitOfWorkRepository.ThemeSectionRepository.AddThemeSection(new WidgetsThemeSection
            {
                CodeName = codeName,
                FriendlyName = friendlyName,
                ThemeId = themeId
            });

            _unitOfWorkRepository.SaveChanges();
        }

        public void RemoveThemeSection(int id)
        {
            var themeSection = _unitOfWorkRepository.ThemeSectionRepository.GetById(id);

            _unitOfWorkRepository.ThemeSectionRepository.RemoveThemeSection(themeSection);

            _unitOfWorkRepository.SaveChanges();
        }

        public void UpdateThemeSection(int id, string friendlyName)
        {
            var themeSection = _unitOfWorkRepository.ThemeSectionRepository.GetById(id);

            if (themeSection != null)
            {
                themeSection.FriendlyName = friendlyName;
            }
        }

        public Database.Entities.Themes GetThemeById(int themeId)
        {
            return _unitOfWorkRepository.ThemesRepository.GetById(themeId);
        }

        public Database.Entities.Themes GetThemeByNameAndAuthor(string themeName, string author)
        {
            return _unitOfWorkRepository.ThemesRepository.GetByNameAndAuthor(themeName, author);
        }

        public Database.Entities.Themes UpdateTheme(int themeId, string description, string authorUrl, string version, string image, string updateUrl)
        {
            var theme = _unitOfWorkRepository.ThemesRepository.GetById(themeId);

            theme.Description = description;
            theme.AuthorUrl = authorUrl;
            theme.Version = version;
            theme.Image = image;
            theme.UpdateUrl = updateUrl;

            _unitOfWorkRepository.SaveChanges();

            return theme;
        }

        public void UpdateTheme(Database.Entities.Themes theme)
        {
            _unitOfWorkRepository.UpdateEntity(theme);

            _unitOfWorkRepository.SaveChanges();
        }

        public void RemoveTheme(int themeId)
        {
            _unitOfWorkRepository.ThemesRepository.RemoveTheme(themeId);

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
