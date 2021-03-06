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
    
    public partial class Users
    {
        public Users()
        {
            this.LoginTokens = new HashSet<LoginTokens>();
            this.News = new HashSet<News>();
            this.PollVotes = new HashSet<PollVotes>();
            this.WebsiteRating = new HashSet<WebsiteRating>();
            this.Websites = new HashSet<Websites>();
        }
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Question { get; set; }
        public string Answer { get; set; }
        public Nullable<System.DateTime> BannedStartDate { get; set; }
        public Nullable<System.DateTime> BannedEndDate { get; set; }
        public string EmailVerificationCode { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsAdminVerified { get; set; }
        public string Password { get; set; }
        public string NewEmail { get; set; }
        public string NewEmailVerificationCode { get; set; }
    
        public virtual ICollection<LoginTokens> LoginTokens { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<PollVotes> PollVotes { get; set; }
        public virtual ICollection<WebsiteRating> WebsiteRating { get; set; }
        public virtual ICollection<Websites> Websites { get; set; }
    }
}
