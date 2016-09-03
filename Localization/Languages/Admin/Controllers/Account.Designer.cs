﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization.Languages.Admin.Controllers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Account {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Account() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.Languages.Admin.Controllers.Account", typeof(Account).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The account has been updated..
        /// </summary>
        public static string AccountUpdatedSuccesfully {
            get {
                return ResourceManager.GetString("AccountUpdatedSuccesfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail is already in use for another user..
        /// </summary>
        public static string EmailAlreadyExists {
            get {
                return ResourceManager.GetString("EmailAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid end date..
        /// </summary>
        public static string InvalidEndDate {
            get {
                return ResourceManager.GetString("InvalidEndDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This account doesn&apos;t exist (anymore)..
        /// </summary>
        public static string NoAccountFound {
            get {
                return ResourceManager.GetString("NoAccountFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user has been verified by an administrator..
        /// </summary>
        public static string UserAdminStatusVerifiedSuccessfully {
            get {
                return ResourceManager.GetString("UserAdminStatusVerifiedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user has been banned..
        /// </summary>
        public static string UserBannedSuccessfully {
            get {
                return ResourceManager.GetString("UserBannedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The email from the user has already been verified..
        /// </summary>
        public static string UserEmailAlreadyVerified {
            get {
                return ResourceManager.GetString("UserEmailAlreadyVerified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The email has been verified from the user..
        /// </summary>
        public static string UserEmailVerified {
            get {
                return ResourceManager.GetString("UserEmailVerified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user has been unbanned successfully..
        /// </summary>
        public static string UserUnbannedSuccessfully {
            get {
                return ResourceManager.GetString("UserUnbannedSuccessfully", resourceCulture);
            }
        }
    }
}