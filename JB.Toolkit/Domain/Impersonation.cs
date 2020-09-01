using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace JBToolkit.Domain.Impersonation
{
    /*
     * Developer: James Brindle
     * 24/01/2019
     */

    /*
     Usage:

         using (new Impersonator(ServerType.DOMAIN, "<username>", "<password>"))
         {
          
         }
         
       or

         using (new Impersonator(ServerType.DMZ, "<username>", "<password>", "DMZ-WEB01-D"))
         {
          
         }
    */

    public enum ServerType
    {
        DMZ,
        DOMAIN
    }

    public enum LogonType
    {
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, such as a user being logged
        /// on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching
        /// logon information for disconnected operations; therefore, it is inappropriate for some client/server applications,
        /// such as a mail server.
        /// </summary>
        LOGON32_LOGON_INTERACTIVE = 2,

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords.
        /// The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        LOGON32_LOGON_NETWORK = 3,

        /// <summary>
        /// This logon type is intended for batch servers, where processes may be executing on behalf of a user
        /// without their direct intervention. This type is also for higher performance servers that process many
        /// plaintext authentication attempts at a time, such as mail or web servers.
        /// </summary>
        LOGON32_LOGON_BATCH = 4,

        /// <summary>
        /// Indicates a service-type logon. The account provided must have the service privilege enabled.
        /// </summary>
        LOGON32_LOGON_SERVICE = 5,

        /// <summary>
        /// GINAs are no longer supported.
        /// Windows Server 2003 and Windows XP:  This logon type is for GINA DLLs that log on users who will be
        /// interactively using the computer. This logon type can generate a unique audit record that shows when
        /// the workstation was unlocked.
        /// </summary>
        LOGON32_LOGON_UNLOCK = 7,

        /// <summary>
        /// This logon type preserves the name and password in the authentication package, which allows the server
        /// to make connections to other network servers while impersonating the client. A server can accept plaintext
        /// credentials from a client, call LogonUser, verify that the user can access the system across the network,
        /// and still communicate with other servers.
        /// </summary>
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher

        /// <summary>
        /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        /// The new logon session has the same local identifier but uses different credentials for other network connections.
        /// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        /// </summary>
        LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
    };

    public enum LogonProvider
    {
        LOGON32_PROVIDER_DEFAULT = 0,
        LOGON32_PROVIDER_WINNT35 = 1,
        LOGON32_PROVIDER_WINNT40 = 2,
        LOGON32_PROVIDER_WINNT50 = 3
    };

    public enum ImpersonationLevel
    {
        /// <summary>
        /// The server cannot impersonate or identify the client.
        /// </summary>
        SecurityAnonymous = 0,

        /// <summary>
        /// The server can get the identity and privileges of the client, but cannot impersonate the client.
        /// </summary>
        SecurityIdentification = 1,

        /// <summary>
        /// The server can impersonate the client's security context on the local system.
        /// </summary>
        SecurityImpersonation = 2,

        /// <summary>
        /// The server can impersonate the client's security context on remote systems.
        /// </summary>
        SecurityDelegation = 3
    }

    class Win32NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(string lpszUserName,
             string lpszDomain,
             string lpszPassword,
             int dwLogonType,
             int dwLogonProvider,
             ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
              int impersonationLevel,
              ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
    }

    /// <summary>
    /// Used to perform execution of code under an elivated permission context
    /// </summary>
    public class Impersonator : IDisposable
    {
        private WindowsImpersonationContext _wic;

        /// <summary>
        /// Begins impersonation with the given credentials, Logon type and Logon provider.
        /// </summary>
        public Impersonator(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
        {
            Impersonate(userName, domainName, password, logonType, logonProvider);
        }

        /// <summary>
        /// Begins impersonation with the given credentials.
        /// </summary>
        ///
        public Impersonator(string userName, string domainName, string password)
        {
            Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Begins impersonation with the given credentials.
        /// </summary>
        ///
        public Impersonator(string userName, string password)
        {
            Impersonate(userName, Environment.UserDomainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Impersonator"/> class.
        /// </summary>
        public Impersonator()
        { }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_wic != null)
                _wic.Dispose();

            UndoImpersonation();
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        ///
        public void Impersonate(string userName, string domainName, string password)
        {
            Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        ///
        public void Impersonate(string userName, string password)
        {
            Impersonate(userName, Environment.UserDomainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        ///
        public void Impersonate(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
        {
            UndoImpersonation();

            IntPtr logonToken = IntPtr.Zero;
            IntPtr logonTokenDuplicate = IntPtr.Zero;
            try
            {
                // revert to the application pool identity, saving the identity of the current requestor
                _wic = WindowsIdentity.Impersonate(IntPtr.Zero);

                // do logon & impersonate
                if (Win32NativeMethods.LogonUser(userName,
                    domainName,
                    password,
                    (int)logonType,
                    (int)logonProvider,
                    ref logonToken) != 0)
                {
                    if (Win32NativeMethods.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) != 0)
                    {
                        var wi = new WindowsIdentity(logonTokenDuplicate);
                        wi.Impersonate(); // discard the returned identity context (which is the context of the application pool)
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                {
                    Win32NativeMethods.CloseHandle(logonToken);
                }

                if (logonTokenDuplicate != IntPtr.Zero)
                {
                    Win32NativeMethods.CloseHandle(logonTokenDuplicate);
                }
            }
        }

        /// <summary>
        /// Stops impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            try
            {
                // restore saved requestor identity
                if (_wic != null)
                {
                    _wic.Undo();
                }
            }
            catch { }

            _wic = null;
        }

        public class ImpersonatorCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Domain { get; set; } = Environment.UserDomainName;

            public static ImpersonatorCredentials GetImpersonatatorCredentials(
                ServerType serverType,
                string credentialUsename,
                string credentialPassword,
                string dmzHost = null)
            {
                var cred = new ImpersonatorCredentials();

                switch (serverType)
                {
                    case ServerType.DMZ:
                        cred.Username = credentialUsename;
                        cred.Password = credentialPassword;
                        cred.Domain = dmzHost;
                        break;
                    case ServerType.DOMAIN:
                        cred.Username = credentialUsename;
                        cred.Domain = Environment.UserDomainName;
                        cred.Password = credentialPassword;
                        break;
                    default:
                        return null;
                }

                return cred;
            }
        }
    }
}