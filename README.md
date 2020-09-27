# JB Toolkit

&nbsp;
[![N|Solid](https://portfolio.jb-net.co.uk/shared/Logo-Only-100px.png)](https://github.com/jamesbrindle/JB.Toolkit)

A collection of useful classes, extensions and tools written in C# to aid in a wide variety of development. With components for ASP Net MVC, Winform and Console applications. Integrations for Google, AD and SQL and pre-written implementations / helpers for a variety of tasks... Just Because (see below).

&nbsp;
&nbsp;


## Nuget Package

&nbsp;
```
Install-Package JB.Toolkit
```
&nbsp;
&nbsp;

## Tools Include, But Not Limited To

* **Assembly Helper**
	* Retrive embedded resources
	* Culture helper
* **CSV Helper**
	* Convert CSV to and from DataTable
	* Convert CSV to and from object
* **Collection of useful extention methods**
	* Type conversions
	* DateTime helpers
	* string manipulation
	* Html Razor extentions (email attachment)
	* Roslyn source code formatter
* **Console App Helper**
	* Spinner
	* Progress bar
	* Mono.Options (Command line arguments: http://docs.go-mono.com/index.aspx?link=T:Mono.Options.OptionSet)
	* Any console foreground and background colour (ColourFul.Console library)
	* FIGLet fonts (ASCII art)
* **Database**
	* Get data table, get scaler, execute non-Query, execute non-query with scoped identity
	* Dapper implementations
	* Tree view (i.e. jstree) cache storage
* **Domain**
    * AD DB lookup
    * Impersonation
    * AD Tools
* **Email**
    * Send email
    * Easy send error report
    * Easy send results table (i.e. from List or DataTable) to email with a nicely formatted HTML table
* **SharePoint API CSOM Implementation**
    * Authenticate with app add-in client & secret key or user credentials
    * Upload and download files from a document library
    * Copy and move files and folders in a document library
    * Retrive file and folder lists from document library
* **Encryption**
	* Typical string encryption
	* Simple Connection string encryption
* **Fuzzy Logic**
	* Variety of algorithms (Levenshtein Distance, Ratcliff Obserhelp Simularity, Sorensen Dice Distance etc)
	* Test approximately equal
	* Check best match in collection
* **Google APIs**
	* GeoCoding
	* GeoVision (i.e. reading text from image)
* **Imaging**
	* Image type converter
	* Image manipulation
	* Icon helper
			* Get icon from file type (many embedded images)
	* Web image helper (mime type helepr, base64 image, get image bytes from URL etc)
			* Mime type descriptor from image byte array using magic number detection
	* Colour helper
	* OCR
* **Interprocess Communication**
	* Memory Mapped file
	* Message queues
	* Named pipes
	* Net remoting
	* Sockets
	* Wcf
	* Wmp Copy data
* **JQuery DataTable Models / Views**
    * HTMML DataTable
    * Ajax DataTable
    * Feed DataTable
    * Editor
    * MVC Jquery DataTable views
* **Logger**
	* DB logger
	* File logger
	* Windows event logger
* **Microsoft Windows Helper**
	* File and directory helpers
		* Transversively (recursively) get paths (orders them differently)
		* Get Windows special folder paths (Music, Videos, Documents, Pictures etc)
		* Fast directory enumerator
	* Process helper
		* Remote run, kill
	* Service helper
		* Remote start, stop, restart, aggressively restart
	* UAT helper
* **PDF Tools**
	* Converter
		* Convert from DocX
		* Convert from HTML
		* Convert from PDF
		* Convert to PDF
	* PDF merger
	* PDF text parser
* **Regular Expressions**
	* Common regular expression lists
	* Try convert any string format of a date to DateTime
	* Match all dates in a string
* **Stream Helper**
	* Safe Stream
	* Read file without loc
	* Throttle Stream (with percent complete and bytes per second event)
* **Web**
    *  Access logger
    * Authentication - Access token generator
	* HTTP Helper
	* IP Helper
	* Json Helper
	* Memory File (i.e. For download file or creating an email attachment)
* **WinForms**
	* Collection of custom controls
	* Optimised Metro Form
	* Taskbar icon helper
	* Imaging tools
* **XML Doc (Word, Excel, PowerPoint, PDF) Tools**
	* Converter
		* HTML / MS Office / Image to PDF
		* PDF to MS Office / HTML / Image
		* Bulk convert MS Office to PDF
		* MS Office / PDF / Image (OCR) to text file (.txt)
	* Mail merge
	* Find and replace
	* Image extractor
* **Zip**
	* Compress / create archive
	* Extract archive
	* Extract other compression archive filetypes, i.e. .rar, .7z, .tar.gz etc
