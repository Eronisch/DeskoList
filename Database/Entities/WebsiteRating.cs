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
    
    public partial class WebsiteRating
    {
        public int WebsiteID { get; set; }
        public string Ip { get; set; }
        public int Rating { get; set; }
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual Websites Websites { get; set; }
    }
}
