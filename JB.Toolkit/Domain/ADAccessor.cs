using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web;

namespace JBToolkit.Domain
{
    /// <summary>
    /// Accesses a given Active Directory server as specified in the web.config for retrieving additional user information
    /// </summary>
    public class AdAccessor
    {        /// <summary>
             /// Gets Active Directory attributes of a given user specified by username
             /// </summary>
             /// <param name="username">The username to retrieve AD attribute for</param>
             /// <param name="callingFromWithinDomain">Are we call from within AD or from a DMZ</param>
             /// <param name="adServerIpAddress">IP Address of AD controller</param>
             /// <param name="adServerHostName">Host name of AD controller</param>
             /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
             /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
             /// <returns>A custom AD object with limited attributes</returns>
        public static AdUser GetAdUser(
            string username,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            return GetAdUser(
                    username,
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    Environment.UserDomainName,
                    adAdminUsername,
                    adAdminPassword);
        }

        /// <summary>
        /// Gets Active Directory attributes of a given user specified by username
        /// </summary>
        /// <param name="username">The username to retrieve AD attribute for</param>
        /// <param name="callingFromWithinDomain">Are we call from within AD or from a DMZ</param>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adDomainName">I.e. Likely be the company name</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        /// <returns>A custom AD object with limited attributes</returns>
        public static AdUser GetAdUser(
        string username,
        bool callingFromWithinDomain,
        string adServerIpAddress,
        string adServerHostName,
        string adDomainName,
        string adAdminUsername,
        string adAdminPassword)
        {
            if (username.Contains("\\"))
            {
                username = username.Substring(username.IndexOf("\\") + 1);
            }

            try
            {
                // DEBUG
                string ldapAddress = "LDAP://" + (callingFromWithinDomain || string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? adServerHostName : adServerIpAddress);

                DirectoryEntry de = new DirectoryEntry(ldapAddress, adAdminUsername, adAdminPassword);

                DirectorySearcher dSearch = new DirectorySearcher(de)
                {
                    Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))"
                };

                SearchResult sResultSet = dSearch.FindOne();

                string managerDN = GetProperty(sResultSet, "manager");
                DirectoryEntry manager = new DirectoryEntry("LDAP://" + managerDN, null, null, AuthenticationTypes.Secure);

                string managerAccount = null;

                using (manager)
                {
                    if (manager.Properties.Contains("samaccountname"))
                    {
                        managerAccount = manager.Properties["samaccountname"][0].ToString();
                    }
                }

                AdUser usr = new AdUser
                {
                    DisplayName = GetProperty(sResultSet, "displayname"),
                    Username = username,
                    Domain = adDomainName,
                    UserAccount = adDomainName + "\\" + username,
                    JobTitle = GetProperty(sResultSet, "description"),
                    Office = GetProperty(sResultSet, "physicalDeliveryOfficeName"),
                    Department = GetProperty(sResultSet, "department"),
                    Telephone = GetProperty(sResultSet, "telephoneNumber"),
                    Email = GetProperty(sResultSet, "mail"),
                    Fax = GetProperty(sResultSet, "facsimileTelephoneNumber"),
                    IPPhone = GetProperty(sResultSet, "	ipPhone"),
                    Mobile = GetProperty(sResultSet, "	mobile"),
                    Manager = GetADUserManager(managerAccount, callingFromWithinDomain, adServerIpAddress, adServerHostName, adDomainName, adAdminUsername, adAdminPassword),
                    Memberships = new List<string>()
                };

                // AD groups / memberships associated to user

                int membershipCount = sResultSet.Properties["memberOf"].Count;
                string dn = string.Empty;
                int equalsIndex, commaIndex;

                for (int i = 0; i < membershipCount; i++)
                {
                    dn = (string)sResultSet.Properties["memberOf"][i];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);

                    if (-1 == equalsIndex)
                    {
                        break;
                    }

                    usr.Memberships.Add(dn.Substring((equalsIndex + 1),
                                                     (commaIndex - equalsIndex) - 1));
                }

                return usr;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // It doesn't really matter. We're not relying on this class for anything other than a little more
                // information of the current user, we can live without it - just return empty properties
#if DEBUG
                Console.Out.WriteLine("AD read error: " + e.Message);
#endif
                AdUser usr = new AdUser
                {
                    JobTitle = string.Empty,
                    Office = string.Empty,
                    Department = string.Empty,
                    Telephone = string.Empty,
                    Email = string.Empty,
                    Fax = string.Empty,
                    IPPhone = string.Empty,
                    Mobile = string.Empty,
                    Memberships = new List<string>()
                };

                return usr;
            }
        }

        /// <summary>
        /// Returns the manager ADUser object of the user
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static AdUser GetAdUserManager(
            string username,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword,
            bool recursivelyGetHierarchyAbove = false)
        {
            return GetADUserManager(
                    username,
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    Environment.UserDomainName,
                    adAdminUsername,
                    adAdminPassword,
                    recursivelyGetHierarchyAbove
                );
        }

        /// <summary>
        /// Returns the manager ADUser object of the user
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adDomainName">I.e. Likely be the company name</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static AdUser GetADUserManager(
        string username,
        bool callingFromWithinDomain,
        string adServerIpAddress,
        string adServerHostName,
        string adDomainName,
        string adAdminUsername,
        string adAdminPassword,

        bool recursivelyGetHierarchyAbove = false)
        {
            if (username.Contains("\\"))
            {
                username = username.Substring(username.IndexOf("\\") + 1);
            }

            try
            {
                // DEBUG
                string ldapAddress = "LDAP://" + (callingFromWithinDomain ||
                    string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? adServerHostName : adServerIpAddress);

                DirectoryEntry de = new DirectoryEntry(ldapAddress, adAdminUsername, adAdminPassword);

                DirectorySearcher dSearch = new DirectorySearcher(de)
                {
                    Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))"
                };

                SearchResult sResultSet = dSearch.FindOne();

                string managerAccount = null;

                if (recursivelyGetHierarchyAbove)
                {
                    string managerDN = GetProperty(sResultSet, "manager");
                    DirectoryEntry manager = new DirectoryEntry("LDAP://" + managerDN, null, null, AuthenticationTypes.Secure);

                    using (manager)
                    {
                        if (manager.Properties.Contains("samaccountname"))
                        {
                            managerAccount = manager.Properties["samaccountname"][0].ToString();
                        }
                    }
                }

                AdUser usr = new AdUser
                {
                    DisplayName = GetProperty(sResultSet, "displayname"),
                    Username = username,
                    Domain = adDomainName,
                    UserAccount = adDomainName + "\\" + username,
                    JobTitle = GetProperty(sResultSet, "description"),
                    Office = GetProperty(sResultSet, "physicalDeliveryOfficeName"),
                    Department = GetProperty(sResultSet, "department"),
                    Telephone = GetProperty(sResultSet, "telephoneNumber"),
                    Email = GetProperty(sResultSet, "mail"),
                    Fax = GetProperty(sResultSet, "facsimileTelephoneNumber"),
                    IPPhone = GetProperty(sResultSet, "	ipPhone"),
                    Mobile = GetProperty(sResultSet, "	mobile"),
                    Manager = recursivelyGetHierarchyAbove ? GetADUserManager(
                                                                managerAccount,
                                                                callingFromWithinDomain,
                                                                adServerIpAddress,
                                                                adServerHostName,
                                                                adDomainName,
                                                                adAdminUsername,
                                                                adAdminPassword) : null
                };

                return usr;
            }

#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // It doesn't really matter. We're not relying on this class for anything other than a little more
                // information of the current user, we can live without it - just return empty properties
#if DEBUG
                Console.Out.WriteLine("AD read error: " + e.Message);
#endif
                AdUser usr = new AdUser
                {
                    JobTitle = string.Empty,
                    Office = string.Empty,
                    Department = string.Empty,
                    Telephone = string.Empty,
                    Email = string.Empty,
                    Fax = string.Empty,
                    IPPhone = string.Empty,
                    Mobile = string.Empty,
                    Memberships = new List<string>()
                };

                return usr;
            }
        }

        public enum IdentityType
        {
            LogonIdenity,
            Environment,
            PrincipleIdentity
        }

        /// <summary>
        /// Returns the username from an AD search by email address
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static string GetUsernameFromEmailAddress(
            string emailAddress,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                string username = GetAdSearchResult(
                    emailAddress,
                    "mail",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["samaccountname"][0].ToString();
                return username;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the full name from an AD search by email address
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static string GetNameFromEmailAddress(
            string emailAddress,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                string fullName = GetAdSearchResult(
                    emailAddress,
                    "mail",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["DisplayName"][0].ToString();

                return fullName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the full name from an AD search by username
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static string GetNameFromUsername(
            string username,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                string fullName = GetAdSearchResult(
                    username,
                    "samaccountname",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["DisplayName"][0].ToString();

                return fullName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the email from an AD search by username
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static string GetEmailFromUsername(
            string username,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                string email = GetAdSearchResult(
                    username,
                    "samaccountname",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["mail"][0].ToString();

                return email;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the logged in user email (don't use this if deploying to IIS without Windows authentication)
        /// </summary>
        public static string GetLoggedInUserEmail()
        {
            return UserPrincipal.Current.EmailAddress;
        }

        /// <summary>
        /// Gets the photo stored in AD user
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static byte[] GetAdPhotoFromEmail(
            string email,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                byte[] bb = (byte[])GetAdSearchResult(
                    email,
                    "mail",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["thumbnailPhoto"][0];

                return bb;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the photo stored in AD user
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static byte[] GetAdPhotoFromUsername(
            string username,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                byte[] bb = (byte[])GetAdSearchResult(
                    username,
                    "samaccountname",
                    callingFromWithinDomain,
                    adServerIpAddress,
                    adServerHostName,
                    adAdminUsername,
                    adAdminPassword).Properties["thumbnailPhoto"][0];

                return bb;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all users of the domain
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static SearchResultCollection GetAllUsers(
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            try
            {
                SearchResultCollection au = GetAdSearchResults(
                                                null,
                                                null,
                                                callingFromWithinDomain,
                                                adServerIpAddress,
                                                adServerHostName,
                                                adAdminUsername,
                                                adAdminPassword);
                return au;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a DirectoryServices SearchResult object
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static SearchResult GetAdSearchResult(
            string searchString,
            string searchFilter,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            string ldapAddress = "LDAP://" + (callingFromWithinDomain ||
                string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? adServerHostName : adServerIpAddress);

            DirectoryEntry adEntry = new DirectoryEntry(ldapAddress)
            {
                Username = adAdminUsername,
                Password = adAdminPassword
            };

            DirectorySearcher adSearcher = new DirectorySearcher(adEntry)
            {
                Filter = "(&((&(objectCategory=Person)(objectClass=User)))" +
                    (string.IsNullOrEmpty(searchFilter) && string.IsNullOrEmpty(searchString) ? "" : "(" + searchFilter + "=" + searchString + ")") + ")"
            };
            SearchResult result = adSearcher.FindOne();

            return result;
        }

        /// <summary>
        /// Returns a DirectoryServices SearchResult object
        /// </summary>
        /// <param name="adServerIpAddress">IP Address of AD controller</param>
        /// <param name="adServerHostName">Host name of AD controller</param>
        /// <param name="adAdminUsername">A user username who's able to read the AD DB</param>
        /// <param name="adAdminPassword">A user password who's able to read the AD DB</param>
        public static SearchResultCollection GetAdSearchResults(
            string searchString,
            string searchFilter,
            bool callingFromWithinDomain,
            string adServerIpAddress,
            string adServerHostName,
            string adAdminUsername,
            string adAdminPassword)
        {
            string ldapAddress = "LDAP://" + (callingFromWithinDomain ||
                string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? adServerHostName : adServerIpAddress);

            DirectoryEntry adEntry = new DirectoryEntry(ldapAddress)
            {
                Username = adAdminUsername,
                Password = adAdminPassword
            };

            DirectorySearcher adSearcher = new DirectorySearcher(adEntry)
            {
                Filter = "(&((&(objectCategory=Person)(objectClass=User)))" +
                    (string.IsNullOrEmpty(searchFilter)
                        && string.IsNullOrEmpty(searchString)
                                ? ""
                                : "(" + searchFilter + "=" + searchString + ")") + ")"
            };
            SearchResultCollection results = adSearcher.FindAll();

            return results;
        }

        /// <summary>
        /// Get username based on different types of 'IdentityType'
        /// </summary>
        public static string GetUsername(IdentityType identityType)
        {
            switch (identityType)
            {
                case IdentityType.LogonIdenity:
                    return HttpContext.Current.Request.LogonUserIdentity.Name;
                case IdentityType.Environment:
                    return Environment.UserName;
                case IdentityType.PrincipleIdentity:
                    return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }

            return null;
        }

        /// <summary>
        /// Searches the AD entry for a given attribtues
        /// </summary>
        /// <param name="searchResult">AD object to search within</param>
        /// <param name="PropertyName">The attribute or property we wish to retrieve</param>
        /// <returns></returns>
        private static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Object to store user details, such as username, full name, email, job title etc and what roles and permissions they have. Also can get user profile image
    /// </summary>
    [Serializable]
    public class AdUser
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public string UserAccount { get; set; }
        public string JobTitle { get; set; }
        public string Office { get; set; }
        public string Department { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string IPPhone { get; set; }
        public string Email { get; set; }
        public AdUser Manager { get; set; }
        public List<string> Memberships { get; set; }
    }
}