
namespace JBToolkit.RegularExpressions
{
    public class Common
    {
        /// <summary>
        /// Includes uncomon email addresses
        /// </summary>
        public static readonly string Pattern_Email = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        /// <summary>
        /// Should have 1 lowercase letter, 1 uppercase letter, 1 number, and be at least 8 characters long
        /// </summary>
        public static readonly string Pattern_ModeratePassword = @"(?=(.*[0-9]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;""'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";

        /// <summary>
        /// 1 lowercase letter, 1 uppercase letter, 1 number, 1 special character and be at least 8 characters long 
        /// </summary>
        public static readonly string Pattern_ComplexPassword = @"(?=(.*[0-9]))((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.{8,}$";

        /// <summary>
        /// Has to include protocol (http / https)
        /// </summary>
        public static readonly string Pattern_URLWithProtocol = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#()?&//=]*)";

        /// <summary>
        /// Protocal (http / https optional)
        /// </summary>
        public static readonly string Pattern_URL = @"/^((https?|ftp|file):\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/";

        /// <summary>
        /// International Phone Number (Optional country code / extension)
        /// </summary>
        public static readonly string Pattern_PhoneNumberInternational = @"^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$";

        /// <summary>
        /// Basic phone number
        /// </summary>
        public static readonly string Pattern_PhoneNumberBasic = @"^\b\d{3}[-.]?\d{3}[-.]?\d{4}\b$";

        /// <summary>
        /// File path with directory, name, extension
        /// </summary>
        public static readonly string Pattern_FilePath = @"((\/|\\|\/\/|https?:\\\\|https?:\/\/)[a-z0-9 _@\-^!#$%&+={}.\/\\\[\]]+)+\.[a-z]+$";

        /// <summary>
        /// File Path with optional Filename, extension
        /// </summary>
        public static readonly string Pattern_FilePathExtensionOptional = @"^(.+)/([^/]+)$";

        /// <summary>
        /// Administration number optional
        /// </summary>
        public static readonly string Pattern_NationalInsurance = @"^[A-CEGHJ-PR-TW-Z]{1}[A-CEGHJ-NPR-TW-Z]{1}[0-9]{6}[A-DFM]{0,1}$";

        /// <summary>
        /// UK post code
        /// </summary>
        public static readonly string Pattern_PostCode = @"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})";

        /// <summary>
        /// ipv4
        /// </summary>
        public static readonly string Pattern_IPv4Address = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        /// <summary>
        /// ipv6
        /// </summary>
        public static readonly string Pattern_IPv6Address = @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$";

        /// <summary>
        /// String is HTML / XML Tag
        /// </summary>
        public static readonly string Pattern_HtmlTag = @"<\/?[\w\s]*>|<.+[\W]>";

        /// <summary>
        /// URL Slug
        /// </summary>
        public static readonly string Pattern_Slug = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";

        /// <summary>
        /// JavaScript Handler
        /// </summary>
        public static readonly string Pattern_JavaScriptHandler = @"\bon\w+=\S+(?=.*>)";

        /// <summary>
        /// JavaScript Handler with element4
        /// </summary>
        public static readonly string Pattern_JavaScriptHandlerWithElement = @"(?:<[^>]+\s)(on\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?";

        /// <summary>
        /// Hex Colour
        /// </summary>
        public static readonly string Pattern_HexColour = @"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$";

        /// <summary>
        /// Date Format dd-mmm-YYYY or dd/mmm/YYYY or dd.mmm.YYYY
        /// </summary>
        public static readonly string Pattern_Date_ddmmmmYYYY = @"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$";

        /// <summary>
        /// Date Format dd-MM-YYYY or dd/MM/YYYY or dd.MM.YYYY
        /// </summary>
        public static readonly string Pattern_Date_ddMMYYYY = @"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$";

        /// <summary>
        /// Date Format YYYY-MM-dd using separator -
        /// </summary>
        public static readonly string Pattern_Date_YYYYMMdd = @"([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))";

        /// <summary>
        /// Time only 12 hour
        /// </summary>
        public static readonly string Pattern_Time_12Hour = @"^(0?[1-9]|1[0-2]):[0-5][0-9]$";

        /// <summary>
        /// Time with AM PM
        /// </summary>
        public static readonly string Pattern_Time_AmPm = @"((1[0-2]|0?[1-9]):([0-5][0-9]) ?([AaPp][Mm]))";

        /// <summary>
        /// Time 24 hour
        /// </summary>
        public static readonly string Pattern_Time_24Hour = @"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
    }
}
