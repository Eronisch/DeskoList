//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Plugins
    {
        public Plugins()
        {
            this.AdminBreadcrumbs = new HashSet<AdminBreadcrumbs>();
            this.AdminNavigation = new HashSet<AdminNavigation>();
            this.PluginOpenUpdates = new HashSet<PluginOpenUpdates>();
            this.ActiveDlls = new HashSet<ActiveDlls>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string AuthorUrl { get; set; }
        public string Description { get; set; }
        public string UpdateUrl { get; set; }
        public bool Enabled { get; set; }
        public string Area { get; set; }
        public string Namespace { get; set; }
    
        public virtual ICollection<AdminBreadcrumbs> AdminBreadcrumbs { get; set; }
        public virtual ICollection<AdminNavigation> AdminNavigation { get; set; }
        public virtual ICollection<PluginOpenUpdates> PluginOpenUpdates { get; set; }
        public virtual ICollection<ActiveDlls> ActiveDlls { get; set; }
    }
}
