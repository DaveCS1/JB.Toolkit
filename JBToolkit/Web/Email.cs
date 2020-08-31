using JBToolkit.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static JBToolkit.Global;

namespace JBToolkit.Web
{
    /// <summary>
    /// Contains methods for validating and sending emails (validating being thing such as validating the attached file size or extention)
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Basic send email method
        /// </summary>
        /// <returns>EmailSendResult Object</returns>
        public static EmailResult SendMail(MailMessage email)
        {
            bool impersonate = false;

            EmailConfiguration emailProfile;
            if (EmailConfiguration.EmailProfile(email.From) != null)
                emailProfile = EmailConfiguration.EmailProfile(email.From.Address);
            else
            {
                emailProfile = EmailConfiguration<EmailProfile.StandardEmailProfile>.Profile;
                impersonate = true;
            }

            using (var client = new SmtpClient(
                                        emailProfile.Smtp,
                                        emailProfile.Port.ToInt()))
            {
                if (emailProfile.SecurePassword == null)
                {
                    client.UseDefaultCredentials = true;
                    client.EnableSsl = false;
                }
                else
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(emailProfile.EmailAddress,
                                                               emailProfile.SecurePassword);
                }

                if (impersonate)
                {
                    emailProfile.EmailAddress = email.From.Address;
                    emailProfile.DisplayName = email.From.DisplayName;
                }

                try
                {
                    client.Send(email);
                    return new SuccessEmailResult();
                }
                catch (Exception ex)
                {
                    return new FailureEmailResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// Basic send email method
        /// </summary>
        /// <returns>EmailSendResult Object</returns>
        public static EmailResult SendMail(EmailConfiguration emailProfile, MailMessage email)
        {
            using (var client = new SmtpClient(
                                        emailProfile.Smtp,
                                        emailProfile.Port.ToInt()))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(emailProfile.EmailAddress,
                                                           emailProfile.SecurePassword);
                try
                {
                    client.Send(email);
                    return new SuccessEmailResult();
                }
                catch (Exception ex)
                {
                    return new FailureEmailResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// Easy call method to send an email returning a Email Send Result object
        /// </summary>
        /// <returns>EmailSendResult Object</returns>
        public static EmailResult SendMail(
            string senderName,
            string senderEmail,
            string emailTo,
            string ccTo,
            string subject,
            string body,
            List<HttpPostedFileBase> attachments,
            bool useWebConfig = true)
        {
            try
            {
                string errMsg = string.Empty;
                if (SendMail(senderName, senderEmail, emailTo, ccTo, subject, body, attachments, out errMsg, useWebConfig))
                    return new SuccessEmailResult();
                else
                    return new FailureEmailResult(errMsg);
            }
            catch (Exception ex)
            {
                return new FailureEmailResult(ex.Message);
            }
        }

        /// <summary>
        /// Easy call method to send an email
        /// </summary>
        /// <returns>True if sending successful</returns>
        public static bool SendMail(
            string senderName,
            string senderEmail,
            string emailTo,
            string ccTo,
            string subject,
            string body,
            List<HttpPostedFileBase> attachments,
            out string warningMessage,
            bool useWebConfig = false)
        {
            try
            {
                warningMessage = null;
                bool impersonate = false;

                EmailConfiguration emailProfile;
                if (EmailConfiguration.EmailProfile(senderEmail) != null)
                    emailProfile = EmailConfiguration.EmailProfile(senderEmail);
                else if (EmailConfiguration.EmailProfile(senderName) != null)
                    emailProfile = EmailConfiguration.EmailProfile(senderName);
                else
                {
                    impersonate = true;
                    emailProfile = EmailConfiguration<EmailProfile.StandardEmailProfile>.Profile;
                }

                SmtpClient client = new SmtpClient
                {
                    Timeout = 30000, // 30 seconds
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if (!useWebConfig)
                {
                    client.Host = emailProfile.Smtp;
                    client.Port = emailProfile.Port.ToInt();

                    if (emailProfile.SecurePassword == null)
                    {
                        client.UseDefaultCredentials = true;
                        client.EnableSsl = false;
                    }
                    else
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(emailProfile.EmailAddress,
                                                                   emailProfile.SecurePassword);
                    }
                }

                if (impersonate)
                {
                    emailProfile.EmailAddress = senderEmail;
                    emailProfile.DisplayName = senderName;
                }

                MailMessage mm = new MailMessage()
                {
                    From = new MailAddress(string.Format("{0} <{1}>", senderName, senderEmail)),
                    Body = body,
                    Subject = subject,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };

                foreach (var email in emailTo.Replace(" ", "").Split(';'))
                    mm.To.Add(new MailAddress(email));

                if (!string.IsNullOrEmpty(ccTo))
                    foreach (var email in ccTo.Replace(" ", "").Split(';'))
                        mm.CC.Add(new MailAddress(email));

                // Do some validation on attachments - If done correctly, this would have already been done as part of the 
                // attribute tag on the modal propertly, however if this method is used elsewhere and it's not added, this is a fallback.

                if (attachments != null)
                {
                    if (attachments.Count > 0)
                    {
                        int filesSize = 0;

                        foreach (var postedFileBase in attachments)
                        {
                            var fileExt = System.IO.Path.GetExtension(postedFileBase.FileName).Substring(1).ToLower();

                            // may need to remove some files with unsupported file types
                            if (System.Web.Mvc.HtmlExtensions.UnsupportedFilesTypes.Contains(fileExt))
                            {
                                if (string.IsNullOrEmpty(warningMessage))
                                    warningMessage = "Some files with unsupported file types have been removed: ";

                                warningMessage += postedFileBase.FileName + ",";
                            }
                            else
                            {
                                filesSize += postedFileBase.ContentLength;

                                var attachment = new Attachment(postedFileBase.InputStream, postedFileBase.FileName);
                                mm.Attachments.Add(attachment);
                            }
                        }

                        // 24mb (1mb below thje 25mb max just in case)
                        if (filesSize > 24 * 1024 * 1024)
                        {
                            // fail

                            warningMessage = "Attached files exceeds 24MB.";
                            return false;
                        }
                    }
                }

                // remove trailing comma ',' if needs be
                if (!string.IsNullOrEmpty(warningMessage))
                    warningMessage = warningMessage.Substring(0, warningMessage.Length - 2);

                client.Send(mm);

                return true;
            }
            catch (Exception e)
            {
                try
                {
                    warningMessage = null;

                    SmtpClient client = new SmtpClient
                    {
                        Timeout = 30000, // 30 seconds
                        DeliveryMethod = SmtpDeliveryMethod.Network,

                    };

                    client.Host = GoogleConfiguration.Smtp;
                    client.Port = GoogleConfiguration.Port.ToInt();
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(GoogleConfiguration.GmailUser,
                                                               GoogleConfiguration.GmailPassword);

                    MailMessage mm = new MailMessage()
                    {
                        From = new MailAddress(string.Format("{0} <{1}>", senderName, GoogleConfiguration.GmailUser)),
                        Body = body,
                        Subject = subject,
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    foreach (var email in emailTo.Replace(" ", "").Split(';'))
                        mm.To.Add(new MailAddress(email));

                    if (!string.IsNullOrEmpty(ccTo))
                        foreach (var email in ccTo.Replace(" ", "").Split(';'))
                            mm.CC.Add(new MailAddress(email));

                    // Do some validation on attachments - If done correctly, this would have already been done as part of the 
                    // attribute tag on the modal propertly, however if this method is used elsewhere and it's not added, this is a fallback.

                    if (attachments != null)
                    {
                        if (attachments.Count > 0)
                        {
                            int filesSize = 0;

                            foreach (var postedFileBase in attachments)
                            {
                                var fileExt = System.IO.Path.GetExtension(postedFileBase.FileName).Substring(1).ToLower();

                                // may need to remove some files with unsupported file types
                                if (System.Web.Mvc.HtmlExtensions.UnsupportedFilesTypes.Contains(fileExt))
                                {
                                    if (string.IsNullOrEmpty(warningMessage))
                                        warningMessage = "Some files with unsupported file types have been removed: ";

                                    warningMessage += postedFileBase.FileName + ",";
                                }
                                else
                                {
                                    filesSize += postedFileBase.ContentLength;

                                    var attachment = new Attachment(postedFileBase.InputStream, postedFileBase.FileName);
                                    mm.Attachments.Add(attachment);
                                }
                            }

                            // 24mb (1mb below thje 25mb max just in case)
                            if (filesSize > 24 * 1024 * 1024)
                            {
                                // fail

                                warningMessage = "Attached files exceeds 24MB.";
                                return false;
                            }
                        }
                    }

                    // remove trailing comma ',' if needs be
                    if (!string.IsNullOrEmpty(warningMessage))
                        warningMessage = warningMessage.Substring(0, warningMessage.Length - 2);

                    client.Send(mm);

                    return true;
                }
                catch (Exception ex)
                {
                    FileLogger.LogError(e);
                    warningMessage = ex.Message;

                    return false;
                }
            }
        }

        /// <summary>
        /// Send a DatTable to be formatted and displayed in a HTML email. Column names are wordified.
        /// </summary>
        public static EmailResult SendResultsEmail(
            DataTable dataTable,
            string senderName,
            string senderEmail,
            string emailTo,
            string ccTo,
            string subject,
            string body,
            string footer,
            List<HttpPostedFileBase> attachments,
            bool useWebConfig = true)
        {
            try
            {
                if (SendResultsEmail(
                        dataTable,
                        senderName,
                        senderEmail,
                        emailTo,
                        ccTo,
                        subject,
                        body,
                        footer,
                        attachments,
                        out string errMsg,
                        useWebConfig))
                {
                    return new SuccessEmailResult();
                }
                else
                {
                    return new FailureEmailResult(errMsg);
                }
            }
            catch (Exception ex)
            {
                return new FailureEmailResult(ex.Message);
            }
        }

        /// <summary>
        /// Send a DatTable to be formatted and displayed in a HTML email. Column names are wordified.
        /// </summary>
        /// <returns>True if sending successful</returns>
        public static bool SendResultsEmail(
            DataTable dataTable,
            string senderName,
            string senderEmail,
            string emailTo,
            string ccTo,
            string subject,
            string body,
            string footer,
            List<HttpPostedFileBase> attachments,
            out string warningMessage,
            bool useWebConfig = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<body style='font-family: Arial,Helvetica Neue,Helvetica,sans-serif;'>
	                    <h3>" + (subject ?? "") + @"</h3>
	                    <br /><br />" + (body != null ? body + @"<br /><br /><br />" : ""));


            sb.Append(@"<table style='width:100%;border-collapse: collapse; border: 1px solid black; padding: 2px; text-align: center;font-family: Arial,Helvetica Neue,Helvetica,sans-serif; font-size: 10pt;' >
		                    <tr style='color: white; background-color: #0c216b;'>");

            foreach (DataColumn column in dataTable.Columns)
                sb.Append("<th style='font-weight: bold;border:1px solid black;'>" + column.ColumnName.Wordify() + @"</th>");

            sb.Append("</tr>");

            foreach (DataRow row in dataTable.Rows)
            {
                sb.Append(@"<tr style='border: 1px solid black;text-align: center;'>");

                foreach (var item in row.ItemArray)
                    sb.Append("<td style='border: 1px solid black;text-align: center;'>" + item.ToString() + @"</td>");

                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append(!string.IsNullOrEmpty(footer) ? "<br /><br />" + footer : "");
            sb.Append(@"<br /><br /><br />
	                    Kind regards,
	                        <br /><br />
	                    " + senderName + @"
                            <br /><br />
	                    </body>");

            try
            {
                SendMail(
                    senderName,
                    senderEmail,
                    emailTo,
                    ccTo,
                    subject,
                    sb.ToString(),
                    attachments,
                    out warningMessage,
                    useWebConfig);
                return true;
            }
            catch (Exception e)
            {
                warningMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Standardise easy sent email of an error or issue
        /// </summary>
        /// <returns>EmailSendResult Object</returns>
        public static void EmailError(
            string siteOrApplicationName,
            string senderName,
            string senderEmail,
            string recipient,
            string subject,
            Exception e,
            string additionalMessage = "",
            bool useWebConfig = false)
        {
            EmailError(siteOrApplicationName, senderName, senderEmail, recipient, string.Format("{0} Error - {1}", siteOrApplicationName, subject),
               string.Format("{0}<h4>Error:</h4>{1}<br /><br /><h4>Stack Trace:</h4>{2}",
                        string.IsNullOrEmpty(additionalMessage)
                                    ? ""
                                    : "<h4>" + additionalMessage + "<h4><br /><br />",
                        e.Message, e.StackTrace),
               useWebConfig);
        }

        /// <summary>
        /// Standardise easy sent email of an error or issue
        /// </summary>
        public static void EmailError(
            string siteOrApplicationName,
            string senderName,
            string senderEmail,
            string recipient,
            string subject,
            string message,
            bool useWebConfig = false)
        {
            string htmlBody = string.Format(@"<body style='font-family: Arial,Helvetica Neue,Helvetica,sans-serif; '>
	                    <h3>{0}</h3>
	                    <br /><br />", subject);

            htmlBody += message;

            htmlBody += @"<br /><br /><br />
	                    Kind regards,
	                        <br /><br />
	                    " + senderName + @"
                            < br /><br />
	                    </body>";
            try
            {
                SendMail(
                    siteOrApplicationName + senderName,
                    senderEmail,
                    recipient,
                    null,
                    subject,
                    htmlBody,
                    null,
                    out string msg,
                    useWebConfig);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Determines is a given string is valid email address using regular expression: ^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$
        /// </summary>
        /// <param name="email">Email string</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool EmailAddressIsValid(string email)
        {
            string expresion;

            expresion = RegularExpressions.Common.Pattern_Email;

            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the logged in user email (don't use this if deploying to IIS without Windows authentication)
        /// </summary>
        public static string GetLoggedInUserEmail()
        {
            return UserPrincipal.Current.EmailAddress;
        }
    }

    /// <summary>
    /// Contains email sent status information
    /// </summary>
    public class EmailResult
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
    }

    /// <summary>
    /// Contains email sent 'success' information
    /// </summary>
    public class SuccessEmailResult : EmailResult
    {
        public SuccessEmailResult()
        {
            Success = true;
            Message = "Message Sent Successfully";
        }
    }

    /// <summary>
    /// Contains email sent 'success' information
    /// </summary>
    public class FailureEmailResult : EmailResult
    {
        public FailureEmailResult(string exceptionMessage)
        {
            Success = false;
            Message = $"Error sending message: {exceptionMessage}";
        }
    }
}