using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security;

namespace JBToolkit
{
    /// <summary>
    /// Configurations strings (i.e. connection string, smtp configuration etc) need to be set here
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Google related. Includes Google SMTP settings (useful for a 'fallback' SMTP service)
        /// </summary>
        public static class GoogleConfiguration
        {
            public static readonly SecureString GoogleApiKey = new NetworkCredential("", "<api key here>").SecurePassword;
            public static readonly string GoogleApiUrl = "https://maps.googleapis.com/maps/api";

            public static readonly string Smtp = "smtp.gmail.com";
            public static readonly string Port = "587";

            public static readonly string GmailUser = "<gmail username here>";

            public static readonly SecureString GmailPassword = new NetworkCredential("", "<gmail password here>").SecurePassword;
        }

        /// <summary>
        /// Domain / AD related
        /// </summary>
        public static class ADConfiguration
        {
            // Domain
            public static readonly string AdDomain = "<ad domain here>";
            public static readonly string AdUrl = "<ad domaun url here>";
            public static readonly string AdServerName = "<domain controller server host name here>";
            public static readonly string AdServerIP = "<domain controller server IP address here>";

            public static readonly SecureString AdAdminUser = new NetworkCredential("", "<ad admin username here>").SecurePassword;

            public static readonly SecureString AdAdminPassword = new NetworkCredential("", "<ad admin password here").SecurePassword;

            public static readonly SecureString DmzAdminUser = new NetworkCredential("", "dmz admin username here").SecurePassword;

            public static readonly SecureString DmzAdminPassword = new NetworkCredential("", "dmz admin password here").SecurePassword;
        }

        public interface IEmailConfiguration
        {
            string ProfileName { get; }
            string EmailAddress { get; set; }
            string DisplayName { get; set; }
            string Smtp { get; }
            string Port { get; }
            SecureString SecurePassword { get; }
        }

        public static class EmailConfiguration<T>
        {
            public static EmailConfiguration Profile { get { return Map(typeof(T)); } }
            public static string ProfileName { get { return Map(typeof(T)).ProfileName; } }
            public static string EmailAddress { get { return Map(typeof(T)).EmailAddress; } }
            public static string DisplayName { get { return Map(typeof(T)).DisplayName; } }
            public static string Smtp { get { return Map(typeof(T)).Smtp; } }
            public static string Port { get { return Map(typeof(T)).Port; } }
            public static SecureString SecurePassword { get { return Map(typeof(T)).SecurePassword; } }

            private static EmailConfiguration Map(Type t)
            {
                return (EmailConfiguration)Activator.CreateInstance(t);
            }
        }

        /// <summary>
        /// Abstract email profile class.
        /// </summary>
        public abstract class EmailConfiguration : IEmailConfiguration
        {
            public abstract string ProfileName { get; }
            public abstract string EmailAddress { get; set; }
            public abstract string DisplayName { get; set; }
            public virtual string Smtp { get; } = "smtp.office365.com";
            public virtual string Port { get; } = "587";
            public abstract SecureString SecurePassword { get; }

            /// <summary>
            /// Use like: EmailConfiguration.EmailProfile("admin@company.co.uk"));
            /// </summary>
            public static EmailConfiguration EmailProfile(string senderNameOrEmailAddress)
            {
                foreach (var emailProfile in Global.EmailProfile.GetAllEmailProfiles())
                {
                    if (senderNameOrEmailAddress.ToLower().In(emailProfile.DisplayName.ToLower(),
                                                              emailProfile.DisplayName.Replace(" ", "").ToLower(),
                                                              emailProfile.EmailAddress.ToLower()))
                    {
                        Type t = Type.GetType("JBToolkit.Global+EmailProfile+" + emailProfile.ProfileName);
                        return (EmailConfiguration)Activator.CreateInstance(t);
                    }
                }

                return null;
            }

            /// <summary>
            /// Use like: EmailConfiguration.EmailProfile(client.From));
            /// </summary>
            public static EmailConfiguration EmailProfile(MailAddress mailAddress)
            {
                foreach (var emailProfile in Global.EmailProfile.GetAllEmailProfiles())
                {
                    if (mailAddress.DisplayName.ToLower().In(emailProfile.DisplayName.ToLower(),
                                                             emailProfile.DisplayName.Replace(" ", "").ToLower()))
                    {
                        Type t = Type.GetType("JBToolkit.Global+EmailProfile+" + emailProfile.ProfileName);
                        return (EmailConfiguration)Activator.CreateInstance(t);
                    }

                    if (mailAddress.Address.ToLower().In(emailProfile.EmailAddress.ToLower()))
                    {
                        Type t = Type.GetType(emailProfile.ProfileName);
                        return (EmailConfiguration)Activator.CreateInstance(t);
                    }
                }

                return null;
            }

            /// <summary>
            /// Use like: EmailConfiguration.EmailProfile(typeof(EmailProfile.StandardEmailProfile));
            /// </summary>
            public static EmailConfiguration EmailProfile(Type EmailProfile)
            {
                EmailConfiguration.EmailProfile(typeof(EmailProfile.StandardEmailProfile));
                return (EmailConfiguration)Activator.CreateInstance(EmailProfile);
            }
        }

        /// <summary>
        /// Container for email profiles
        /// </summary>
        public class EmailProfile
        {
            /// <summary>
            /// Returns all known service email profiles
            /// </summary>
            public static List<EmailConfiguration> GetAllEmailProfiles()
            {
                return new List<EmailConfiguration>()
                {
                   EmailConfiguration<StandardEmailProfile>.Profile,
                };
            }

            /// <summary>
            /// Standard email profile (no-reply@company.co.uk)
            /// </summary>
            public class StandardEmailProfile : EmailConfiguration
            {
                public override string ProfileName { get; } = typeof(StandardEmailProfile).Name;
                public override string EmailAddress { get; set; } = "<email address here>";
                public override string DisplayName { get; set; } = "<email sender name here>";
                public override SecureString SecurePassword { get; } = null;
                public override string Smtp { get; } = "<smtp host here>";
                public override string Port { get; } = "<smtp port here>";
            }
        }

        /// <summary>
        /// Specific database configurations accessible from 'Global'
        /// </summary>
        public static class DatabaseConfiguration
        {
            /// <summary>
            /// I.e. Live, development, upgrade
            /// </summary>
            public enum DatabaseEnvironmentType
            {
                Production,
                Staging,
                Testing
            }

            /// <summary>
            /// Specific databases accessible from 'Global'
            /// </summary>
            public static class Database
            {
                public static readonly SecureString ProductionDBConnectionString = new NetworkCredential("", "<connection string here>").SecurePassword;

                public static readonly SecureString StagingDBConnectionString = new NetworkCredential("", "<connection string here>").SecurePassword;

                public static readonly SecureString TestingDBConnectionString = new NetworkCredential("", "<connection string here>").SecurePassword;

                /// <summary>
                /// Returns a database connector string for a given environment
                /// </summary>
                public static SecureString GetEnvironmentConnectionString(DatabaseEnvironmentType environmentType)
                {
                    SecureString connectionString = new SecureString();
                    switch (environmentType)
                    {
                        case DatabaseEnvironmentType.Production:
                            connectionString = ProductionDBConnectionString;
                            break;
                        case DatabaseEnvironmentType.Staging:
                            connectionString = StagingDBConnectionString;
                            break;
                        case DatabaseEnvironmentType.Testing:
                            connectionString = TestingDBConnectionString;
                            break;
                        default:
                            break;
                    }

                    return connectionString;
                }

                public enum StandardProductionDBInstances
                {
                    DBProductionInstance
                }

                public enum StandardStagingDBInstances
                {
                    DBStagingInstance
                }

                public enum StandardTestingDBInstances
                {
                    DBTestingInstance
                }
            }
        }
    }
}
