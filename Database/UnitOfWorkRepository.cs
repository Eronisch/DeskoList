using System;
using Database.Entities;
using Database.Repositories;

namespace Database
{
    public class UnitOfWorkRepository : DataSource
    {
        private readonly Lazy<SettingsRepository> _settingsRepository;
        private readonly Lazy<WebsiteRepository> _websitesRepository;
        private readonly Lazy<CategoryRepository> _categoryRepository;
        private readonly Lazy<AccountRepository> _accountRepository;
        private readonly Lazy<EmailTemplatesRepository> _emailTemplatesRepository;
        private readonly Lazy<EmailAccountRepository> _emailAccountRepository;
        private readonly Lazy<SeoPagesRepository> _seoPagesRepository;
        private readonly Lazy<PollRepository> _pollRepository;
        private readonly Lazy<NewsRepository> _newsRepository;
        private readonly Lazy<WebsiteBlackListRepository> _websiteBlackListRepository;
        private readonly Lazy<ReportRepository> _reportRepository;
        private readonly Lazy<WebsiteInRepository> _websiteInRepository;
        private readonly Lazy<RatingRepository> _ratingRepository;
        private readonly Lazy<WebsiteOutRepository> _websiteOutRepository;
        private readonly Lazy<WidgetsRepository> _widgetsRepository;
        private readonly Lazy<ThemesRepository> _themesRepository;
        private readonly Lazy<ThemeSectionRepository> _themeSectionRepository;
        private readonly Lazy<NavigationPagesRepository> _navigationPagesRepository;
        private readonly Lazy<DynamicPagesRepository> _dynamicPagesRepository;
        private readonly Lazy<WebsiteInDailyRepository> _websiteInDailyRepository;
        private readonly Lazy<WebsiteOutDailyRepository> _websiteOutDailyRepository;
        private readonly Lazy<AdminBreadCrumbsRepository> _adminBreadCrumbsRepository;
        private readonly Lazy<WidgetsThemeRepository> _widgetsThemeRepository;
        private readonly Lazy<OpenUpdatesRepository> _openUpdatesRepository;
        private readonly Lazy<AdminNavigationRepository> _adminNavigationRepository;
        private readonly Lazy<SoftwareUpdateSettingsRepository> _softwareUpdateSettingsRepository;
        private readonly Lazy<WidgetUpdateSettingsRepository> _widgetUpdateSettingsRepository;
        private readonly Lazy<WidgetOpenUpdatesRepository> _widgetOpenUpdatesRepository;
        private readonly Lazy<ThemeUpdateSettingsRepository> _themeUpdateSettingsRepository;
        private readonly Lazy<ThemeOpenUpdatesRepository> _themeOpenUpdatesRepository;
        private readonly Lazy<ActiveDllRepository> _activeDllRepository;
        private readonly Lazy<DllRepository> _dllRepository;
        private readonly Lazy<LanguageRepository> _languageRepository;
        private readonly Lazy<LoginTokensRepository> _loginTokensRepository;
        private readonly Lazy<LoginFailsRepository> _loginFailsRepository;
        private readonly Lazy<PluginRepository> _pluginRepository;
        private readonly Lazy<PluginOpenUpdatesRepository> _pluginOpenUpdatesRepository;
        private readonly Lazy<PluginUpdateSettingsRepository> _pluginUpdateSettingsRepository;
        private readonly Lazy<ElmahRepository> _elmahRepository;
        private readonly Lazy<LinksRepository> _linkRepository;

        public UnitOfWorkRepository()
        {
            _settingsRepository = new Lazy<SettingsRepository>(() => new SettingsRepository(Query.Set<Settings>()));
            _websitesRepository = new Lazy<WebsiteRepository>(() => new WebsiteRepository(Query.Set<Websites>()));
            _categoryRepository = new Lazy<CategoryRepository>(() => new CategoryRepository(Query.Set<Categories>()));
            _accountRepository = new Lazy<AccountRepository>(() => new AccountRepository(Query.Set<Users>()));
            _emailTemplatesRepository = new Lazy<EmailTemplatesRepository>(() => new EmailTemplatesRepository(Query.Set<EmailTemplates>()));
            _emailAccountRepository = new Lazy<EmailAccountRepository>(() => new EmailAccountRepository(Query.Set<EmailAccounts>()));
            _seoPagesRepository = new Lazy<SeoPagesRepository>(() => new SeoPagesRepository(Query.Set<SeoPages>()));
            _newsRepository = new Lazy<NewsRepository>(() => new NewsRepository(Query.Set<News>()));
            _pollRepository = new Lazy<PollRepository>(() => new PollRepository(Query.Set<Poll>()));
            _websiteBlackListRepository = new Lazy<WebsiteBlackListRepository>(() => new WebsiteBlackListRepository(Query.Set<WebsiteBlackList>()));
            _reportRepository = new Lazy<ReportRepository>(() => new ReportRepository(Query.Set<Reports>()));
            _ratingRepository = new Lazy<RatingRepository>(() => new RatingRepository(Query.Set<WebsiteRating>()));
            _websiteInRepository = new Lazy<WebsiteInRepository>(() => new WebsiteInRepository(Query.Set<WebsiteIn>()));
            _websiteOutRepository = new Lazy<WebsiteOutRepository>(() => new WebsiteOutRepository(Query.Set<WebsiteOut>()));
            _widgetsRepository = new Lazy<WidgetsRepository>(() => new WidgetsRepository(Query.Set<Widgets>()));
            _themesRepository = new Lazy<ThemesRepository>(() => new ThemesRepository(Query.Set<Themes>()));
            _themeSectionRepository = new Lazy<ThemeSectionRepository>(() => new ThemeSectionRepository(Query.Set<WidgetsThemeSection>()));
            _navigationPagesRepository = new Lazy<NavigationPagesRepository>(() => new NavigationPagesRepository(Query.Set<NavigationPages>()));
            _dynamicPagesRepository = new Lazy<DynamicPagesRepository>(() => new DynamicPagesRepository(Query.Set<DynamicPages>()));
            _websiteInDailyRepository = new Lazy<WebsiteInDailyRepository>(() => new WebsiteInDailyRepository(Query.Set<WebsiteInDaily>()));
            _websiteOutDailyRepository = new Lazy<WebsiteOutDailyRepository>(() => new WebsiteOutDailyRepository(Query.Set<WebsiteOutDaily>()));
            _adminBreadCrumbsRepository = new Lazy<AdminBreadCrumbsRepository>(() => new AdminBreadCrumbsRepository(Query.Set<AdminBreadcrumbs>()));
            _widgetsThemeRepository = new Lazy<WidgetsThemeRepository>(() => new WidgetsThemeRepository(Query.Set<WidgetsTheme>()));
            _openUpdatesRepository = new Lazy<OpenUpdatesRepository>(() => new OpenUpdatesRepository(Query.Set<OpenUpdates>()));
            _adminNavigationRepository = new Lazy<AdminNavigationRepository>(() => new AdminNavigationRepository(Query.Set<AdminNavigation>()));
            _softwareUpdateSettingsRepository = new Lazy<SoftwareUpdateSettingsRepository>(() => new SoftwareUpdateSettingsRepository(Query.Set<SoftwareUpdateSettings>()));
            _widgetUpdateSettingsRepository = new Lazy<WidgetUpdateSettingsRepository>(() => new WidgetUpdateSettingsRepository(Query.Set<WidgetUpdateSettings>()));
            _widgetOpenUpdatesRepository = new Lazy<WidgetOpenUpdatesRepository>(() => new WidgetOpenUpdatesRepository(Query.Set<WidgetOpenUpdates>()));
            _themeUpdateSettingsRepository = new Lazy<ThemeUpdateSettingsRepository>(() => new ThemeUpdateSettingsRepository(Query.Set<ThemeUpdateSettings>()));
            _themeOpenUpdatesRepository = new Lazy<ThemeOpenUpdatesRepository>(() => new ThemeOpenUpdatesRepository(Query.Set<ThemeOpenUpdates>()));
            _activeDllRepository = new Lazy<ActiveDllRepository>(() => new ActiveDllRepository(Query.Set<ActiveDlls>()));
            _dllRepository = new Lazy<DllRepository>(() => new DllRepository(Query.Set<Dlls>()));
            _languageRepository = new Lazy<LanguageRepository>(() => new LanguageRepository(Query.Set<Languages>()));
            _loginTokensRepository = new Lazy<LoginTokensRepository>(() => new LoginTokensRepository(Query.Set<LoginTokens>())); ;
            _loginFailsRepository = new Lazy<LoginFailsRepository>(() => new LoginFailsRepository(Query.Set<LoginFails>()));
            _pluginRepository = new Lazy<PluginRepository>(() => new PluginRepository(Query.Set<Plugins>()));
            _pluginOpenUpdatesRepository = new Lazy<PluginOpenUpdatesRepository>(() => new PluginOpenUpdatesRepository(Query.Set<PluginOpenUpdates>()));
            _pluginUpdateSettingsRepository = new Lazy<PluginUpdateSettingsRepository>(() => new PluginUpdateSettingsRepository(Query.Set<PluginUpdateSettings>()));
            _elmahRepository = new Lazy<ElmahRepository>(() => new ElmahRepository(Query.Set<ELMAH_Error>()));
            _linkRepository = new Lazy<LinksRepository>(() => new LinksRepository(Query.Set<Links>()));
        }

        public SettingsRepository SettingsRepository { get { return _settingsRepository.Value; } }
        public WebsiteRepository WebsitesRepository { get { return _websitesRepository.Value; } }
        public CategoryRepository CategoryRepository { get { return _categoryRepository.Value; } }
        public AccountRepository AccountRepository { get { return _accountRepository.Value; } }
        public EmailTemplatesRepository EmailTemplatesRepository { get { return _emailTemplatesRepository.Value; } }
        public EmailAccountRepository EmailAccountRepository { get { return _emailAccountRepository.Value; } }
        public SeoPagesRepository SeoPagesRepository { get { return _seoPagesRepository.Value; } }
        public PollRepository PollRepository { get { return _pollRepository.Value; } }
        public NewsRepository NewsRepository { get { return _newsRepository.Value; } }
        public WebsiteBlackListRepository WebsiteBlackListRepository { get { return _websiteBlackListRepository.Value; } }
        public ReportRepository ReportRepository { get { return _reportRepository.Value; } }
        public WebsiteOutRepository WebsiteOutRepository { get { return _websiteOutRepository.Value; } }
        public WidgetsRepository WidgetsRepository { get { return _widgetsRepository.Value; } }
        public ThemesRepository ThemesRepository { get { return _themesRepository.Value; } }
        public ThemeSectionRepository ThemeSectionRepository { get { return _themeSectionRepository.Value; } }
        public NavigationPagesRepository NavigationPagesRepository { get { return _navigationPagesRepository.Value; } }
        public DynamicPagesRepository DynamicPagesRepository { get { return _dynamicPagesRepository.Value; } }
        public WebsiteInDailyRepository WebsiteInDailyRepository { get { return _websiteInDailyRepository.Value; } }
        public WebsiteOutDailyRepository WebsiteOutDailyRepository { get { return _websiteOutDailyRepository.Value; } }
        public AdminBreadCrumbsRepository AdminBreadCrumbsRepository { get { return _adminBreadCrumbsRepository.Value; } }
        public WidgetsThemeRepository WidgetsThemeRepository { get { return _widgetsThemeRepository.Value; } }
        public OpenUpdatesRepository OpenUpdatesRepository { get { return _openUpdatesRepository.Value; } }
        public AdminNavigationRepository AdminNavigationRepository { get { return _adminNavigationRepository.Value; } }
        public SoftwareUpdateSettingsRepository SoftwareUpdateSettingsRepository { get { return _softwareUpdateSettingsRepository.Value; } }
        public WidgetUpdateSettingsRepository WidgetUpdateSettingsRepository { get { return _widgetUpdateSettingsRepository.Value; } }
        public WidgetOpenUpdatesRepository WidgetOpenUpdatesRepository { get { return _widgetOpenUpdatesRepository.Value; } }
        public ThemeUpdateSettingsRepository ThemeUpdateSettingsRepository { get { return _themeUpdateSettingsRepository.Value; } }
        public ThemeOpenUpdatesRepository ThemeOpenUpdatesRepository { get { return _themeOpenUpdatesRepository.Value; } }
        public ActiveDllRepository ActiveDllRepository { get { return _activeDllRepository.Value; } }
        public DllRepository DllRepository { get { return _dllRepository.Value; } }
        public LanguageRepository LanguageRepository { get { return _languageRepository.Value; } }
        public LoginTokensRepository LoginTokensRepository { get { return _loginTokensRepository.Value; } }
        public LoginFailsRepository LoginFailsRepository { get { return _loginFailsRepository.Value; } }
        public PluginRepository PluginRepository { get { return _pluginRepository.Value; } }
        public PluginOpenUpdatesRepository PluginOpenUpdatesRepository { get { return _pluginOpenUpdatesRepository.Value; } }
        public PluginUpdateSettingsRepository PluginUpdateSettingsRepository { get { return _pluginUpdateSettingsRepository.Value; } }
        public WebsiteInRepository WebsiteInRepository { get { return _websiteInRepository.Value; } }
        public RatingRepository RatingRepository { get { return _ratingRepository.Value; } }
        public ElmahRepository ElmahRepository { get { return _elmahRepository.Value; } }
        public LinksRepository LinkRepository { get { return _linkRepository.Value; } }
    }
}
