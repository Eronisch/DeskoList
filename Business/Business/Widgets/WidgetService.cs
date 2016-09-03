using System.Collections.Generic;
using System.Linq;
using Core.Business.Settings;
using Core.Models.Widgets;
using Database;
using Database.Entities;

namespace Core.Business.Widgets
{
    public class WidgetService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private readonly SettingsService _settingsService;

        public WidgetService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _settingsService = new SettingsService();
        }

        public bool IsWidgetByAreaAndNamespace(string area, string @namespace)
        {
            return _unitOfWorkRepository.WidgetsRepository.IsWidgetByAreaAndNamespace(area, @namespace);
        }

        public Database.Entities.Widgets GetByNameSpace(string @namespace)
        {
            return _unitOfWorkRepository.WidgetsRepository.GetByNameSpace(@namespace);
        }

        public IEnumerable<Database.Entities.Widgets> GetAllWidgets()
        {
            return _unitOfWorkRepository.WidgetsRepository.GetAll().OrderBy(x => x.Name);
        }

        /// <summary>
        /// Returns the widgets that are active in the current theme
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Database.Entities.Widgets> GetActiveWidgets()
        {
            int activeThemeId = _settingsService.GetActiveThemeId();

            return GetAllWidgets()
                .Where(
                    m =>
                        m.WidgetsTheme.Any(
                            x => x.IsEnabled && x.WidgetsThemeSection.ThemeId == activeThemeId));
        }

        /// <summary>
        /// Gets all the widgets from the active theme and section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public IEnumerable<Database.Entities.Widgets> GetActiveWidgetsBySection(string sectionName)
        {
            return (from section in _unitOfWorkRepository.ThemeSectionRepository.GetBySection(_settingsService.GetActiveThemeId(), sectionName)
                    from widgetsTheme in section.WidgetsTheme
                    where widgetsTheme.IsEnabled
                    orderby widgetsTheme.Order
                    select widgetsTheme.Widgets);
        }

        /// <summary>
        /// Returns the widget id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="areaName"></param>
        /// <param name="version"></param>
        /// <param name="author"></param>
        /// <param name="authorUrl"></param>
        /// <param name="image"></param>
        /// <param name="startController"></param>
        /// <param name="startIndex"></param>
        /// <param name="namespace"></param>
        /// <param name="updateUrl"></param>
        /// <returns></returns>
        public int AddToDatabase(string name, string description, string areaName, string version, string author,
            string authorUrl, string image, string startController, string startIndex, string @namespace, string updateUrl = null)
        {
            var widget = new Database.Entities.Widgets
            {
                Name = name,
                Description = description,
                AreaName = areaName,
                Author = author,
                AuthorUrl = authorUrl,
                Controller = startController,
                Version = version,
                Namespace = @namespace,
                StartIndex = startIndex,
                Image = image,
                UpdateUrl = updateUrl
            };

            _unitOfWorkRepository.WidgetsRepository.AddWidget(widget);

            _unitOfWorkRepository.SaveChanges();

            return widget.Id;
        }

        public Database.Entities.Widgets GetWidget(int widgetId)
        {
            return _unitOfWorkRepository.WidgetsRepository.GetById(widgetId);
        }

        public void SaveSettings(int id, int themeSectionId, int order)
        {
            var widgetTheme = _unitOfWorkRepository.WidgetsThemeRepository.GetById(id);

            if (widgetTheme == null) { return; }

            widgetTheme.ThemeSectionId = themeSectionId;
            widgetTheme.Order = order;

            _unitOfWorkRepository.SaveChanges();
        }

        public IEnumerable<Database.Entities.Widgets> GetByThemeId(int themeId)
        {
            return
                _unitOfWorkRepository.ThemeSectionRepository.GetByThemeId(themeId)
                    .SelectMany(tm => tm.WidgetsTheme.Select(x => x.Widgets));
        }

        public void ConfigureWidgetTheme(int widgetId, int sectionId, int order)
        {
            var themeSection = _unitOfWorkRepository.ThemeSectionRepository.GetById(sectionId);

            var widgetTheme = _unitOfWorkRepository.WidgetsThemeRepository.GetByWidgetAndSectionId(widgetId, sectionId);

            if (widgetTheme == null)
            {
                themeSection.WidgetsTheme.Add(new WidgetsTheme
                {
                    WidgetId = widgetId,
                    ThemeSectionId = sectionId,
                    Order = order,
                    IsEnabled = true
                });
            }
            else
            {
                widgetTheme.IsEnabled = true;
                widgetTheme.Order = order;
            }

            _unitOfWorkRepository.SaveChanges();
        }

        public void SaveWidget(Database.Entities.Widgets widget)
        {
            _unitOfWorkRepository.UpdateEntity(widget);
            _unitOfWorkRepository.SaveChanges();
        }

        public IEnumerable<WidgetNavigationModel> GetNavigation()
        {
            return (from widgetNavigation in _unitOfWorkRepository.AdminNavigationRepository.GetAll().ToList()
                    select new WidgetNavigationModel
                    {
                        Action = widgetNavigation.Action,
                        Controller = widgetNavigation.Controller,
                        Name =
                            Localization.Services.LocalizationService.GetValue(widgetNavigation.LocalizedBase,
                                widgetNavigation.LocalizedName),
                        AreaName = widgetNavigation.WidgetId != null ? widgetNavigation.Widgets.AreaName : widgetNavigation.Plugins.Area,
                        Icon = widgetNavigation.Icon
                    }).OrderBy(x => x.Name);
        }
    }
}
