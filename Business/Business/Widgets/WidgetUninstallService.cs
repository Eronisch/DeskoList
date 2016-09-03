using System;
using System.Collections.Generic;
using System.IO;
using Core.Business.Dll;
using Core.Business.File;
using Core.Models;
using Core.Models.ThirdParty;
using Database;
using Elmah;
using Localization.Languages.Business.Widgets;

namespace Core.Business.Widgets
{
    public class WidgetUninstallService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private readonly DatabaseService _databaseService;
        private readonly WidgetConfigService _widgetConfigService;

        public WidgetUninstallService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _databaseService = new DatabaseService();
            _widgetConfigService = new WidgetConfigService();
        }

        /// <summary>
        /// Uninstall the widget
        /// </summary>
        /// <param name="widgetId"></param>
        public ResultModel Uninstall(int widgetId)
        {
            try
            {
                var widget = GetWidget(widgetId);

                if (widget == null)
                {
                    return new ResultModel(WidgetUninstallerService.NoWidgetFound);
                }

                string pathWidget = _widgetConfigService.GetAreaPath(widget.AreaName);

                WidgetSettingsService widgetSettingsService;

                var settingsResult = TryLoadSettings(widget.AreaName, out widgetSettingsService);

                if (!settingsResult.IsSuccess) { return new ResultModel(settingsResult.ErrorMessage); }

                var scriptResult = UnInstallSqlScripts(widgetSettingsService.GetSqlUninstallScripts(), pathWidget);

                if (!scriptResult.IsSuccess) { return new ResultModel(scriptResult.ErrorMessage); }

                RemoveUnusedDllsFromDatabase(widget.Id, widget.AreaName);

                RemoveNotUsedWidgetDlls(widget.Id, widget.AreaName);

                RemoveDbRecords(widget.Id);

                RemoveWidgetFolder(pathWidget);

                return new ResultModel();
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
                return new ResultModel(WidgetUninstallerService.UnknownError);
            }
        }

        private ResultModel TryLoadSettings(string tempAreaName, out WidgetSettingsService widgetSettingsResult)
        {
            widgetSettingsResult = null;

            try
            {
                widgetSettingsResult = new WidgetSettingsService(_widgetConfigService.GetSettingsPath(tempAreaName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void RemoveUnusedDllsFromDatabase(int widgetId, string widgetAreaName)
        {
            var dllService = new DllService();

            var widgetDllFilePaths = _widgetConfigService.GetDlls(widgetAreaName);

            foreach (var widgetDllFilePath in widgetDllFilePaths)
            {
                string dllName = Path.GetFileName(widgetDllFilePath);

                if (!dllService.IsDllBeingUsedByWidgetId(widgetId, dllName))
                {
                   dllService.RemoveDll(dllName);
                }
            }
        }

        private Database.Entities.Widgets GetWidget(int widgetId)
        {
            var widgetService = new WidgetService();

            return widgetService.GetWidget(widgetId);
        }

        private void RemoveDbRecords(int widgetId)
        {
            _unitOfWorkRepository.WidgetsRepository.Remove(widgetId);

            _unitOfWorkRepository.SaveChanges();
        }

        private void RemoveWidgetFolder(string pathWidget)
        {
            FileService.RemoveDirectory(pathWidget, true);
        }

        private IEnumerable<string> GetFilesInBin(string widgetAreaName)
        {
            return _widgetConfigService.GetDlls(widgetAreaName);
        }

        private void RemoveNotUsedWidgetDlls(int widgetId, string widgetAreaName)
        {
            var dllService = new DllService();

            foreach (var file in GetFilesInBin(widgetAreaName))
            {
                if (!dllService.IsDllBeingUsedByWidgetId(widgetId, Path.GetFileName(file)))
                {
                    FileService.RemoveFileInBin(Path.GetFileName(file));
                }
            }
        }

        private ResultModel UnInstallSqlScripts(IEnumerable<SqlScript> sqlScripts, string pathWidget)
        {
            bool isSuccess = true;

            foreach (var script in sqlScripts)
            {
                try
                {
                    _databaseService.ExecuteQuery(FileService.GetFileContent(Path.Combine(pathWidget, script.Location)));
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return new ResultModel
            {
                ErrorMessage = !isSuccess ? WidgetUninstallerService.ErrorInSqlScript : string.Empty
            };
        }
    }
}
