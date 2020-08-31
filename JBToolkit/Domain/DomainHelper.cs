using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Web;

namespace JBToolkit.Domain
{
    /// <summary>
    /// Domain tools
    /// </summary>
    public class DomainHelper
    {
        /// <summary>
        /// Current context machine name (where site is hosted)
        /// </summary>
        public static string ServerName
        {
            get
            {
                return HttpContext.Current.Server.MachineName;
            }
        }

        /// <summary>
        /// Returns whether this site in on a host that is part of the AD domain or not
        /// </summary>
        public static bool SiteOnDomain
        {
            get
            {
                try
                {
                    return IPGlobalProperties.GetIPGlobalProperties().DomainName.ToLower().In(Global.ADConfiguration.AdServerName.ToLower(), Global.ADConfiguration.AdUrl.ToLower());
                }

                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the current Domain name
        /// </summary>
        private static string DomainName
        {
            get
            {
                return IPGlobalProperties.GetIPGlobalProperties().DomainName;

            }
        }

        /// <summary>
        /// Can only be used while developing - for debugging purposes. Return the current NT logged in user
        /// </summary>
        public static string UserNT
        {
            get
            {
                return WindowsIdentity.GetCurrent().Name;
            }
        }

        /// <summary>
        /// Is user a doimain admin?
        /// </summary>
        /// <param name="identity">Identity object of user</param>
        /// <returns>True if domain admin, false otherwise</returns>
        public static bool IsDomainAdmin(WindowsIdentity identity)
        {
            System.DirectoryServices.ActiveDirectory.Domain d = System.DirectoryServices.ActiveDirectory.Domain.GetDomain(new
                DirectoryContext(DirectoryContextType.Domain, DomainName));
            using (DirectoryEntry de = d.GetDirectoryEntry())
            {
                byte[] domainSIdArray = (byte[])de.Properties["objectSid"].Value;
                SecurityIdentifier domainSId = new SecurityIdentifier(domainSIdArray, 0);
                SecurityIdentifier domainAdminsSId = new SecurityIdentifier(
                    WellKnownSidType.AccountDomainAdminsSid, domainSId);
                WindowsPrincipal wp = new WindowsPrincipal(identity);

                return wp.IsInRole(domainAdminsSId);
            }
        }
    }
}
