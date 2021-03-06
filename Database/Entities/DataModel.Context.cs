﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class deskoTopsiteEntities : DbContext
    {
        public deskoTopsiteEntities()
            : base("name=deskoTopsiteEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActiveDlls> ActiveDlls { get; set; }
        public virtual DbSet<AdminBreadcrumbs> AdminBreadcrumbs { get; set; }
        public virtual DbSet<AdminNavigation> AdminNavigation { get; set; }
        public virtual DbSet<AdminPages> AdminPages { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Dlls> Dlls { get; set; }
        public virtual DbSet<DynamicPages> DynamicPages { get; set; }
        public virtual DbSet<ELMAH_Error> ELMAH_Error { get; set; }
        public virtual DbSet<EmailAccounts> EmailAccounts { get; set; }
        public virtual DbSet<EmailTemplates> EmailTemplates { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        public virtual DbSet<LoginFails> LoginFails { get; set; }
        public virtual DbSet<LoginTokens> LoginTokens { get; set; }
        public virtual DbSet<NavigationPages> NavigationPages { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<OpenUpdates> OpenUpdates { get; set; }
        public virtual DbSet<PluginOpenUpdates> PluginOpenUpdates { get; set; }
        public virtual DbSet<Plugins> Plugins { get; set; }
        public virtual DbSet<PluginUpdateSettings> PluginUpdateSettings { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<PollAnswers> PollAnswers { get; set; }
        public virtual DbSet<PollVotes> PollVotes { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<SeoPages> SeoPages { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<SoftwareUpdateSettings> SoftwareUpdateSettings { get; set; }
        public virtual DbSet<ThemeOpenUpdates> ThemeOpenUpdates { get; set; }
        public virtual DbSet<Themes> Themes { get; set; }
        public virtual DbSet<ThemeUpdateSettings> ThemeUpdateSettings { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<WebsiteBlackList> WebsiteBlackList { get; set; }
        public virtual DbSet<WebsiteIn> WebsiteIn { get; set; }
        public virtual DbSet<WebsiteInDaily> WebsiteInDaily { get; set; }
        public virtual DbSet<WebsiteOut> WebsiteOut { get; set; }
        public virtual DbSet<WebsiteOutDaily> WebsiteOutDaily { get; set; }
        public virtual DbSet<WebsiteRating> WebsiteRating { get; set; }
        public virtual DbSet<Websites> Websites { get; set; }
        public virtual DbSet<WidgetOpenUpdates> WidgetOpenUpdates { get; set; }
        public virtual DbSet<Widgets> Widgets { get; set; }
        public virtual DbSet<WidgetsTheme> WidgetsTheme { get; set; }
        public virtual DbSet<WidgetsThemeSection> WidgetsThemeSection { get; set; }
        public virtual DbSet<WidgetUpdateSettings> WidgetUpdateSettings { get; set; }
    
        public virtual ObjectResult<string> ELMAH_GetErrorsXml(string application, Nullable<int> pageIndex, Nullable<int> pageSize, ObjectParameter totalCount)
        {
            var applicationParameter = application != null ?
                new ObjectParameter("Application", application) :
                new ObjectParameter("Application", typeof(string));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("PageIndex", pageIndex) :
                new ObjectParameter("PageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ELMAH_GetErrorsXml", applicationParameter, pageIndexParameter, pageSizeParameter, totalCount);
        }
    
        public virtual ObjectResult<string> ELMAH_GetErrorXml(string application, Nullable<System.Guid> errorId)
        {
            var applicationParameter = application != null ?
                new ObjectParameter("Application", application) :
                new ObjectParameter("Application", typeof(string));
    
            var errorIdParameter = errorId.HasValue ?
                new ObjectParameter("ErrorId", errorId) :
                new ObjectParameter("ErrorId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ELMAH_GetErrorXml", applicationParameter, errorIdParameter);
        }
    
        public virtual int ELMAH_LogError(Nullable<System.Guid> errorId, string application, string host, string type, string source, string message, string user, string allXml, Nullable<int> statusCode, Nullable<System.DateTime> timeUtc)
        {
            var errorIdParameter = errorId.HasValue ?
                new ObjectParameter("ErrorId", errorId) :
                new ObjectParameter("ErrorId", typeof(System.Guid));
    
            var applicationParameter = application != null ?
                new ObjectParameter("Application", application) :
                new ObjectParameter("Application", typeof(string));
    
            var hostParameter = host != null ?
                new ObjectParameter("Host", host) :
                new ObjectParameter("Host", typeof(string));
    
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            var messageParameter = message != null ?
                new ObjectParameter("Message", message) :
                new ObjectParameter("Message", typeof(string));
    
            var userParameter = user != null ?
                new ObjectParameter("User", user) :
                new ObjectParameter("User", typeof(string));
    
            var allXmlParameter = allXml != null ?
                new ObjectParameter("AllXml", allXml) :
                new ObjectParameter("AllXml", typeof(string));
    
            var statusCodeParameter = statusCode.HasValue ?
                new ObjectParameter("StatusCode", statusCode) :
                new ObjectParameter("StatusCode", typeof(int));
    
            var timeUtcParameter = timeUtc.HasValue ?
                new ObjectParameter("TimeUtc", timeUtc) :
                new ObjectParameter("TimeUtc", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ELMAH_LogError", errorIdParameter, applicationParameter, hostParameter, typeParameter, sourceParameter, messageParameter, userParameter, allXmlParameter, statusCodeParameter, timeUtcParameter);
        }
    }
}
