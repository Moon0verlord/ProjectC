﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace  CaveroClubhuis.Areas.Identity.Pages.Account.Manage
{
   
    public static class ManageNavPages
    {
       // homepage settings
        public static string Index => "Index";

        // email page
        public static string Email => "Email";

       // change password
        public static string ChangePassword => "ChangePassword";

        // external login page possible
        public static string ExternalLogins => "ExternalLogins";

       // 2fac page popssible
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        // team settings page
        public static string Team => "Team";

       // methods for guiding nav page
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);


        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        public static string TeamNavClass(ViewContext viewContext) => PageNavClass(viewContext, Team);

        // method for getting paths
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
