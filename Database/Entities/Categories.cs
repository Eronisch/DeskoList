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
    
    public partial class Categories
    {
        public Categories()
        {
            this.Websites = new HashSet<Websites>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
    
        public virtual ICollection<Websites> Websites { get; set; }
    }
}
