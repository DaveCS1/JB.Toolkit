using System.Linq;

namespace JBToolkit.Windows
{
    /// <summary>
    /// An every growing list of common file type / format extensions
    /// </summary>
    public static class CommonFileTypeExtensions
    {
        public static string[] Image
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".eps", ".emf", ".svg", ".ico" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_All
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".svg", ".emf",
                                              ".threefr",".threeg2",".threegp",".a",".aai",".ai",".art",".arw",".avs",".b",".bgr",".bgra",".bgro",".brf",".c",".cal",".cals",".canvas",
                                              ".caption",".cin",".cip",".clip",".clipboard",".cmyk",".cmyka",".cr2",".cr3",".crw",".cube",".cur",".cut",".dcm",".dcr",".dcraw",".dcx",
                                              ".dds",".dfont",".dib",".dng",".dpx",".dxt1",".dxt5",".epdf",".epi",".eps",".epsi",".ept",".erf",".exr",".fax",".fits",".flif",".fractal",
                                              ".fts",".g",".g3",".g4",".gif87",".gradient",".gray",".graya",".group4",".hald",".hdr",".heic",".histogram",".hrz",".icb",".ico",".icon",
                                              ".iiq",".inline",".ipl",".isobrl",".isobrl6",".j2c",".j2k",".jng",".jnx",".jp2",".jpc",".jpe",".jpm",".jps",".jpt",".k",".k25",".kdc",".m",
                                              ".mac",".map",".mask",".mat",".matte",".mef",".miff",".mng",".mono",".mov",".mpc",".mrw",".msl",".mtv",".nef",".nrw",".null",".o",".orf",
                                              ".otb",".otf",".pal",".palm",".pam",".pango",".pattern",".pbm",".pcd",".pcds",".pcl",".pct",".pcx",".pdb",".pdfa",".pef",".pes",".pfa",".pfb",
                                              ".pfm",".pgm",".pgx",".picon",".pict",".pix",".pjpeg",".plasma",".pocketmod",".ppm",".ps",".ps2",".ps3",".psb",".psd",".ptif",".pwp",".r",
                                              ".radialgradient",".raf",".ras",".raw",".rgb",".rgb565",".rgba",".rgbo",".rgf",".rla",".rle",".rmf",".rw2",".scr",".screenshot",".sct",".sgi",
                                              ".shtml",".six",".sixel",".sparsecolor",".sr2",".srf",".stegano",".sun",".svg",".svgz",".tga",".thumbnail",".tile",".tim",".tm2",".ttc",".ubrl",
                                              ".ubrl6",".uil",".uyvy",".vda",".vicar",".viff",".vips",".vst",".webp",".wbmp",".wmf",".wpg",".x3f",".xbm",".xc",".xcf",".xpm",".xps",".xv",
                                              ".y",".ycbcr",".ycbcra",".yuv",".pjpeg", ".jjiff" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_Basic
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".svg", ".emf", ".ico" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_Complex
        {
            get
            {
                string[] ext = new string[] { ".threefr",".threeg2",".threegp",".a",".aai",".ai",".art",".arw",".avs",".b",".bgr",".bgra",".bgro",".brf",".c",".cal",".cals",".canvas",
                                              ".caption",".cin",".cip",".clip",".clipboard",".cmyk",".cmyka",".cr2",".cr3",".crw",".cube",".cur",".cut",".dcm",".dcr",".dcraw",".dcx",
                                              ".dds",".dfont",".dib",".dng",".dpx",".dxt1",".dxt5",".epdf",".epi",".eps",".epsi",".ept",".erf",".exr",".fax",".fits",".flif",".fractal",
                                              ".fts",".g",".g3",".g4",".gif87",".gradient",".gray",".graya",".group4",".hald",".hdr",".heic",".histogram",".hrz",".icb",".ico",".icon",
                                              ".iiq",".inline",".ipl",".isobrl",".isobrl6",".j2c",".j2k",".jng",".jnx",".jp2",".jpc",".jpe",".jpm",".jps",".jpt",".k",".k25",".kdc",".m",
                                              ".mac",".map",".mask",".mat",".matte",".mef",".miff",".mng",".mono",".mov",".mpc",".mrw",".msl",".mtv",".nef",".nrw",".null",".o",".orf",
                                              ".otb",".otf",".pal",".palm",".pam",".pango",".pattern",".pbm",".pcd",".pcds",".pcl",".pct",".pcx",".pdb",".pdfa",".pef",".pes",".pfa",".pfb",
                                              ".pfm",".pgm",".pgx",".picon",".pict",".pix",".pjpeg",".plasma",".pocketmod",".ppm",".ps",".ps2",".ps3",".psb",".psd",".ptif",".pwp",".r",
                                              ".radialgradient",".raf",".ras",".raw",".rgb",".rgb565",".rgba",".rgbo",".rgf",".rla",".rle",".rmf",".rw2",".scr",".screenshot",".sct",".sgi",
                                              ".shtml",".six",".sixel",".sparsecolor",".sr2",".srf",".stegano",".sun",".svg",".svgz",".tga",".thumbnail",".tile",".tim",".tm2",".ttc",".ubrl",
                                              ".ubrl6",".uil",".uyvy",".vda",".vicar",".viff",".vips",".vst",".webp",".wbmp",".wmf",".wpg",".x3f",".xbm",".xc",".xcf",".xpm",".xps",".xv",
                                              ".y",".ycbcr",".ycbcra",".yuv",".pjpeg", ".jjiff" };

                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Video
        {
            get
            {
                string[] ext = new string[] { ".swf", ".mov", ".mp4", ".m4v", ".3gp", ".3g2", ".flv", ".f4v", ".avi", ".mpgeg", ".mpg", ".wmv", ".asf", ".ram", ".mkv" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Audio
        {
            get
            {
                string[] ext = new string[] { ".mp3", ".wav", ".aif", ".aiff", ".mpa", ".m4a", ".wma", ".3gp", ".acc", ".aa", ".act", ".aiff", ".alac", ".amr", ".ape",
                                              ".au", ".awb", ".dss", ".dvf", ".flac", ".ivs", ".m4b", ".m4p", ".mmf", ".mpc", ".msv", ".nsf", ".oof", ".oga", ".mogg",
                                              ".opus", ".ra", ".rm", ".raw", ".rf64", "sln", ".tta", ".voc", ".vox", ".wv", "webm", "8svx", ".cda" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Html
        {
            get
            {
                string[] ext = new string[] { ".html", ".htm" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Adobe
        {
            get
            {
                string[] ext = new string[] { ".psd", ".ai", ".indd", ".ps", ".eps", ".prn", ".pdf" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] OpenOffice
        {
            get
            {
                string[] ext = new string[] { ".odt", ".odp", ".ods", ".odg", ".odf", ".sxw", ".sxi", ".sxc", ".sxd", ".stw" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Text
        {
            get
            {
                string[] ext = new string[] { ".txt" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Data
        {
            get
            {
                string[] ext = new string[] { ".fmat", ".roadtrip", ".quicken2017", ".h12", ".vmt", ".u10", ".zmc", ".bld", ".clp", ".ebuild", ".vcs", ".prs", ".potm", ".hst",
                    ".ppsm", ".xlc", ".dockzip", ".kpr", ".nitf", ".aifb", ".dvo", ".sqr", ".quicken2015", ".ali", ".hdf", ".mdl", ".nbp", ".dif", ".tdb", ".txd", ".not",
                    ".fcpevent", ".tax2019", ".edi", ".aw", ".cub", ".mls", ".jasper", ".xem", ".egp", ".bgt", ".kpf", ".nrl", ".bin", ".h13", "0.001", ".quickendata", ".dat",
                    ".cel", ".rox", ".prj", ".adx", ".net", ".menc", ".pptx", ".ppt", ".fsc", ".mmc", ".uwl", ".livereg", ".prdx", ".tmx", ".xmlper", ".ta9", ".mbg", ".mox",
                    ".h17", ".wjr", ".tax2010", ".drl", ".topc", ".tt18", ".lix", ".imt", ".rte", ".itmsp", ".t08", ".i5z", ".xft", ".met", ".tt12", ".hl", ".qmtf", ".twh",
                    ".pj2", ".dm2", ".dii", ".vsx", ".styk", ".hyv", ".sq", ".mosaic", ".pro6plx", ".capt", ".tax2018", ".otln", ".plw", ".ald", ".kdc", ".tbl", ".ldif", ".vok",
                    ".xdna", ".ncorx", ".liveupdate", ".t12", ".aby", ".dvdproj", ".qb2017", ".ckt", ".gedcom", ".mpkt", ".seo", ".mdf", ".contact", ".ggb", ".photoslibrary",
                    ".adt", ".pcb", ".blg", ".cdx", ".exx", ".bdic", ".pka", ".sub", ".jef", ".ip", ".wdf", ".vdf", ".tax2016", ".opju", ".snag", ".abcd", ".tar", ".tcc", ".fil",
                    ".trk", ".rbt", ".enl", ".lib", ".pps", ".ova", ".jph", ".wpc", ".ral", ".ink", ".q08", ".wgt", ".bci", ".rgo", ".qrp", ".pkt", ".fdb", ".grf", ".pdx", ".rfa",
                    ".zap", ".oo3", ".obj", ".opx", ".mno", ".vxml", ".rfo", ".ndx", ".rvt", ".bvp", ".l6t", ".grade", ".usr", ".aam", ".bgl", ".keychain", ".ppsx", ".pfc",
                    ".lbl", ".wab", ".fox", ".xpt", ".sav", ".pdb", ".phb", ".tsv", ".qpb", ".q09", ".lms", ".tra", ".xml", ".mdl", ".mwf", ".ofc", ".fdb", ".iif", ".btm",
                    ".xlf", ".dam", ".pkb", ".pcr", ".emlxpart", ".fcs", ".ttk", ".dsz", ".np", ".tax2009", ".jnt", ".odp", ".ptf", ".inp", ".csv", ".rsc", ".xlt", ".rpt",
                    ".idx", ".poi", ".ppf", ".cma", ".notebook", ".sdf", ".acc", ".mmp", ".mai", ".qdf", ".efx", ".mat", ".3dr", ".ofx", ".vcf", ".key", ".one", ".myi", ".bcm",
                    ".slp", ".jdb", ".vdb", ".gwk", ".tdl", ".ovf", ".oeaccount", ".sen", ".ii", ".sds", ".pks", ".mpx", ".flp", ".mnc", ".hda", ".emb", ".xpj", ".dfproj",
                    ".upoi", ".t18", ".sc45", ".wtb", ".npl", ".kpz", ".ev", ".pdx", ".lvm", ".t07", ".t11", ".aft", ".mth", ".itl", ".crtx", ".box", ".xsl", ".sdf", ".stm",
                    ".in", ".oft", ".rp", ".mpp", ".gs", ".slx", ".xfd", ".paf", ".xrdml", ".celtx", ".otp", ".xpg", ".uccapilog", ".iba", ".dcmd", ".mjk", ".ptn", ".gcw",
                    ".twb", ".anme", ".rcg", ".svf", ".tax2015", ".inx", ".abp", ".ptz", ".lcm", ".trd", ".gbr", ".qb2011", ".tax2011", ".t13", ".grk", ".tax2017", ".lp7",
                    ".lgi", ".trs", ".adcp", ".gdt", ".qb2013", ".out", ".pst", ".pds", ".cap", ".sps", ".idx", ".vce", ".mcdx", ".rte", ".rdb", ".jrprint", ".pptm", ".m",
                    ".fob", ".gpi", ".pmo", ".ffwp", ".potx", ".cdf", ".prj", ".ged", ".tst", ".tbk", ".dsb", ".mdm", ".sdp", ".vcd", ".4dv", ".lsf", ".windowslivecontact",
                    ".mmap", ".qb2014", ".sta", ".ima", ".qvw", ".ab3", ".enex", ".dcm", ".exp", ".xmcd", ".jrxml", ".gan", ".gtp", ".scd", ".pxl", ".cna", ".xrp", ".wgt",
                    ".pd4", ".pod", ".grr", ".te3", ".fpsl", ".lmx", ".rou", ".mdsx", ".dbd", ".t16", ".bjo", ".t05", ".t06", ".rodz", ".mmf", ".kth", ".pd5", ".tdm", ".vrd",
                    ".gno", ".tdt", ".phm", ".grv", ".moho", ".sca", ".tef", ".fmc", ".tt10", ".ddcx", ".id2", ".tkfl", ".dcf", ".hcu", ".sar", ".flo", ".t10", ".ppf", ".mdc",
                    ".npt", ".tet", ".rnq", ".wea", ".sqd", ".das", ".pdas", ".ndk", ".itx", ".ulf", ".mw", ".pkh", ".spv", ".mdj", ".tcx", ".vi", ".mcd", ".cdx", ".exif",
                    ".isf", ".kap", ".qfx", ".mdx", ".fxp", ".tpf", ".esx", ".stykz", ".ctf", ".tax2013", ".ftw", ".xfdf", ".tb", ".er1", ".dal", ".dsy", ".qif", ".mws",
                    ".kismac", ".rpp", ".ptb", ".cvd", ".clm", ".snapfireshow", ".mph", ".wb3", ".pcapng", ".qbw", ".kid", ".vdx", ".mbx", ".qb2012", ".tt13", ".trs", ".sgml",
                    ".mtw", ".ddc", ".brw", ".pjm", ".dmsp", ".mmp", ".t09", ".epw", ".xslt", ".fop", ".mex", ".vtx", ".csa", ".kpx", ".lgh", ".fro", ".hml", ".gc", ".spub",
                    ".cvn", ".opx", ".xdb", ".pxf", ".zdc", ".wnk", ".sle", ".lsl", ".ixb", ".shw", ".rod", ".swk", ".qb2009", ".ulz", ".tfa", ".tpb", ".mbg", ".blb", ".dwi",
                    ".wb2", ".kpp", ".odx", ".ond", ".hcc", ".lix", ".igc", ".omp", ".txf", ".itm", ".ivt", ".huh", ".qmbl", ".fhc", ".stp" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Database
        {
            get
            {
                string[] ext = new string[] { ".te", ".temx", ".accdt", ".accdc", ".teacher", ".ddl", ".gdb", ".eco", ".sqlitedb", ".pdb", ".trm", ".accft", ".fic", ".sqlite",
                                              ".dtsx", ".db", ".sqlite3", ".db.crypt8", ".nyf", ".itdb", ".mdf", ".4dl", ".v12", ".db.crypt", ".marshal", ".daschema", ".udl",
                                              ".alf", ".dbc", ".daconnections", ".crypt9", ".mar", ".his", ".p97", ".gdb", ".db", ".db.crypt12", ".accde", ".pdm", ".abs", ".wmdb",
                                              ".db3", ".fp3", ".crypt6", ".sql", ".pan", ".fp7", ".sdf", ".crypt12", ".sis", ".xmlff", ".db-wal", ".dp1", ".oqy", ".mdb", ".usr",
                                              ".adf", ".dbs", ".crypt7", ".trc", ".cdb", ".fpt", ".wdb", ".dlis", ".dbf", ".crypt8", ".ask", ".hdb", ".qvd", ".btr", ".flexolibrary",
                                              ".dcb", ".$er", ".frm", ".accdb", ".dxl", ".adp", ".xld", ".dsn", ".rpd", ".sdb", ".sdb", ".fdb", ".mwb", ".db-journal", ".odb", ".rodx",
                                              ".cpd", ".nnt", ".cdb", ".crypt5", ".sdc", ".mav", ".orx", ".dbx", ".fdb", ".rod", ".scx", ".grdb", ".abcddb", ".nsf", ".sdb", ".4dd",
                                              ".accdr", ".adb", ".myd", ".edb", ".cdb", ".sdb", ".mpd", ".ndf", ".kdb", ".maq", ".lwx", ".lgc", ".fmp", ".ib", ".nwdb", ".ihx", ".udb",
                                              ".pdb", ".fmp12", ".vvv", ".ora", ".trc", ".odb", ".vis", ".qry", ".db-shm", ".cma", ".jet", ".mdn", ".mdbhtml", ".accdw", ".db2",
                                              ".dacpac", ".rctd", ".dbv", ".nrmlib", ".kexi", ".ckp", ".fmpsl", ".spq", ".maw", ".epim", ".itw", ".pnz", ".dbt", ".ade", ".tps", ".idb",
                                              ".nv", ".dsk", ".sas7bdat", ".tmd", ".ecx", ".sbf", ".cat", ".maf", ".rsd", ".mud", ".rbf", ".mdt", ".dcx", ".adb", ".nv2", ".dqy", ".^^^",
                                              ".mas", ".fp5", ".fm5", ".dad", ".jtx", ".dct", ".owc", ".p96", ".ns2", ".erx", ".exb", ".kexis", ".rod", ".adn", ".abx", ".vpd", ".mrg",
                                              ".gwi", ".ns3", ".kexic", ".xdb", ".edb", ".mfd", ".dadiagrams", ".ns4", ".wrk", ".fcd", ".fp4", ".fol" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Developer
        {
            get
            {
                string[] ext = new string[] { ".lgo", ".appxbundle", ".rbxl", ".sb", ".sb2", ".class", ".cs", ".appx", ".xap", ".b", ".lua", ".mf", ".ino", ".arsc", ".py", ".ypr",
                    ".md", ".sln", ".java", ".cpp", ".rbxm", ".config", ".dex", ".sb3", ".sh", ".swc", ".vbp", ".cd", ".yml", ".c", ".xsd", ".nupkg", ".ocx", ".vdproj", ".fla",
                    ".patch", ".rpy", ".gmx", ".res", ".hpp", ".po", ".fpm", ".mo", ".w32", ".pas", ".resources", ".proto", ".vb", ".gmk", ".res", ".qpr", ".ise", ".s", ".capx",
                    ".pbg", ".abc", ".bbc", ".pwn", ".nls", ".def", ".targets", ".am7", ".ui", ".fbp", ".markdown", ".so", ".sma", ".gm81", ".vbproj", ".fs", ".csproj", ".asm",
                    ".smali", ".v12.suo", ".nxc", ".swd", ".pl", ".sym", ".gs", ".o", ".ex", ".cod", ".bas", ".rc", ".ml", ".bluej", ".dtd", ".y", ".4db", ".d", ".bet", ".cp",
                    ".pb", ".rbc", ".nib", ".idb", ".s19", ".f", ".ctp", ".suo", ".erb", ".ane", ".r", ".mfa", ".resx", ".ssi", ".swift", ".appxupload", ".vcxproj", ".frx",
                    ".tt", ".l", ".caproj", ".apa", ".trx", ".dox", ".xt", ".fxc", ".ymp", ".omo", ".ppc", ".bpl", ".ipr", ".fxml", ".rdlc", ".au3", ".gitattributes", ".vcproj",
                    ".tk", ".vhd", ".scc", ".m4", ".as3proj", ".mpx", ".ftl", ".nk", ".diff", ".rexx", ".gem", ".workspace", ".cxp", ".csx", ".t", ".rul", ".iml", ".hs", ".mk",
                    ".as", ".pbk", ".ads", ".am4", ".testrunconfig", ".testsettings", ".octest", ".dcp", ".gsproj", ".dbproj", ".pyw", ".mm", ".jspf", ".pyd", ".awk", ".csp",
                    ".pbxuser", ".hh", ".hbs", ".cbp", ".asc", ".i", ".pas", ".ipch", ".vbx", ".rb", ".cc", ".asi", ".sc", ".kdevprj", ".msix", ".xcworkspace", ".dmd", ".mxml",
                    ".ptl", ".w", ".dpr", ".idl", ".vm", ".yaml", ".svn-base", ".framework", ".pika", ".as2proj", ".ilk", ".m", ".autoplay", ".bb", ".pri", ".v", ".rsrc", ".cdf",
                    ".ism", ".aps", ".fsscript", ".mss", ".for", ".mshi", ".ccs", ".nvv", ".wiq", ".mv", ".kpl", ".nuspec", ".sltng", ".clw", ".iconset", ".pyx", ".ph", ".playground",
                    ".myapp", ".jsfl", ".alb", ".haml", ".wdw", ".wdl", ".dgml", ".rc2", ".xpp", ".erl", ".xcdatamodeld", ".oca", ".ltb", ".df1", ".dsgm", ".ipr", ".lds", ".lbs",
                    ".dproj", ".xamlx", ".pro", ".twig", ".vbg", ".xaml", ".iwb", ".pbxbtree", ".agi", ".xcconfig", ".hxx", ".mrt", ".cu", ".cxx", ".wsp", ".dpl", ".inc", ".owl",
                    ".a2w", ".lnt", ".src", ".mcp", ".m", ".cp", ".f90", ".nbc", ".pl1", ".pkgdef", ".cbl", ".gch", ".sup", ".hal", ".livecode", ".pbxproj", ".asm", ".pcp", ".sud",
                    ".mshc", ".in", ".msha", ".fsproj", ".lisp", ".forth", ".cls", ".xq", ".edml", ".pbj", ".xojo_xml_project", ".xoml", ".v11.suo", ".vssscc", ".exp", ".v", ".bsc",
                    ".dbml", ".wxs", ".pot", ".pl", ".nw", ".pm", ".idt", ".vdp", ".gitignore", ".vsmacros", ".lbi", ".sas", ".ist", ".pli", ".plc", ".edmx", ".ccn", ".dgsl",
                    ".uml", ".mer", ".inl", ".jic", ".dob", ".csi", ".ctl", ".a", ".ctxt", ".hpf", ".resw", ".nsi", ".pxd", ".ss", ".vc", ".pde", ".tlh", ".xql", ".wixmst",
                    ".lxsproj", ".bbproject", ".tld", ".pri", ".wixlib", ".textfactory", ".msl", ".fxpl", ".slogo", ".nqc", ".fsx", ".dm1", ".lucidsnippet", ".exw", ".wixpdb",
                    ".tmproj", ".vtm", ".wdgt", ".pkgundef", ".refresh", ".fxl", ".src.rpm", ".ncb", ".addin", ".wixout", ".tmlanguage", ".vspx", ".tli", ".mak", ".entitlements",
                    ".scriptterminology", ".fsproj", ".xojo_menu", ".dcuil", ".fgl", ".kdevelop", ".neko", ".ged", ".p3d", ".gs3", ".asvf", ".xcappdata", ".tur", ".mdzip",
                    ".tpu", ".iws", ".ftn", ".vsps", ".gemspec", ".pdm", ".gorm", ".pjx", ".has", ".vbz", ".dec", ".rbp", ".snippet", ".fxcproj", ".eql", ".xojo_binary_project",
                    ".gm6", ".clips", ".jpr", ".wdgtproj", ".mom", ".sqlproj", ".gld", ".rise", ".nsh", ".wdp", ".wpw", ".resjson", ".gszip", ".gameproj", ".rodl", ".dpkw",
                    ".bdsproj", ".wsc", ".cob", ".ned", ".bcp", ".prg", ".am6", ".ccgame", ".am5", ".vsz", ".xcsnapshots", ".xqm", ".ple", ".vspscc", ".gmo", ".licx", ".c",
                    ".tns", ".tcl", ".caf", ".vdm", ".tds", ".dba", ".lproj", ".licenses", ".dsp", ".4th", ".mod", ".xib", ".pch", ".bbprojectd", ".greenfoot", ".rnc", ".spec",
                    ".psc", ".rkt", ".ent", ".mpr", ".xcarchive", ".xcodeproj", ".dpk", ".gfar", ".dcproj", ".pod", ".rbw", ".groupproj", ".msp", ".xojo_project", ".lsproj",
                    ".cvsrc", ".vsmproj", ".wxl", ".vsmdi", ".rav", ".sbproj", ".psm1", ".csn", ".bs2", ".sdef", ".vtml", ".vtv", ".p", ".prg", ".bpg", ".acd", ".rss", ".brx",
                    ".vsp", ".odl", ".r", ".dfm", ".csi", ".lhs", ".xquery", ".fsi", ".wixmsp", ".wixobj", ".wxi", ".deviceids", ".lit", ".vcp", ".ssc", ".dbo", ".dbpro", ".dba",
                    ".vgc", ".cfc", ".dcu", ".magik", ".scriptsuite", ".nfm", ".artproj", ".groovy", ".wixproj", ".tu", ".gmd", ".jpx", ".r", ".rbxlx", ".fcf", ".sjr", ".rbxl",
                    ".pck", ".asi", ".rbxs", ".sprite2", ".ppsm", ".htt", ".frs", ".am1", ".gradle", ".cmd", ".sb2", ".bml", ".rbxmx", ".rpyc", ".scm", ".cfg", ".uasset", ".rbz",
                    ".ahk", ".dll", ".oiv", ".arena", ".luc", ".lua", ".vdf", ".nt", ".modpkg", ".py", ".ac", ".jsx", ".scar", ".scs", ".plsc", ".cgi", ".wsh", ".fdr", ".fsc",
                    ".acr", ".aro", ".gmres", ".mif", ".sdz", ".dvc", ".nvp", ".ocr", ".pkz", ".eix", ".rbxm", ".iwd", ".sh", ".profile", ".ms", ".sb3", ".bash_profile", ".gmx",
                    ".alm", ".ex4", ".abc", ".pwn", ".gml", ".aspx", ".pl", ".wbt", ".sga", ".r", ".mission", ".usr", ".bkup", ".rgs", ".ipr", ".rpy", ".a8s", ".sam", ".csx",
                    ".asp", ".mrc", ".rpj", ".cmake", ".jsl", ".sbk", ".bashrc", ".ins", ".vbx", ".rhtml", ".iim", ".pif", ".mel", ".scpt", ".spt", ".comiclife", ".gmz", ".ism",
                    ".fdx", ".sc45", ".wcx", ".ex5", ".plx", ".ph", ".fsscript", ".dsgm", ".clt", ".haml", ".sounds", ".d3md", ".int", ".mq4", ".ecl", ".tcz", ".epk", ".jmp",
                    ".fdt", ".ebs", ".ssl", ".mzp", ".gs", ".dsa", ".ptz", ".qc", ".rhr", ".pro", ".cfm", ".jsxbin", ".ist", ".tds", ".montage", ".kix", ".vis", ".av", ".skse",
                    ".pie", ".shader", ".ptx", ".qpw", ".ms", ".fsx", ".wsc", ".pdm", ".npt", ".ognc", ".pkh", ".mse", ".sca", ".gld", ".nsh", ".scriv", ".scrivx", ".colz",
                    ".tardist", ".scriptterminology", ".sd7", ".msct", ".fbq", ".sc4", ".warc", ".reb", ".ssc", ".storyisttheme", ".arscript", ".dse", ".vym", ".mqh", ".tbx",
                    ".scexcludb", ".screstorelog", ".tdb", ".fdf", ".love", ".tst", ".jse", ".bsh", ".soundscript", ".map", ".kx", ".ctx", ".wb3", ".psc", ".as", ".win", ".tsc",
                    ".sdef", ".rbw", ".msp", ".fdxt", ".nl2script", ".cvsrc", ".aut", ".vct", ".lsa", ".dnh", ".mex", ".sbs", ".dist", ".owx", ".a3x", ".wb2", ".scriptsuite",
                    ".fpid", ".dam", ".xaf", ".scriptlibrary", ".cphd", ".ncl", ".sts", ".ld", ".zs", ".scw", ".avc", ".r", ".jsh", };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Compressed
        {
            get
            {
                string[] ext = new string[] { ".s00", ".comppkg.hauptwerk.rar", ".ice", ".arduboy", ".mpkg", ".pup", ".rar", ".gz2", ".rte", ".deb", ".vip", ".gzip", ".xapk",
                    ".tbz", ".pkg.tar.xz", ".lemon", ".sy_", ".sit", ".b6z", ".7z", ".bndl", ".dl_", ".dz", ".pkg", ".bz2", ".r00", ".uha", ".zpi", ".jar.pack", ".fzpz",
                    ".pbi", ".wa", ".mint", ".gza", ".sqx", ".qda", ".sifz", ".r2", ".rp9", ".dar", ".lzm", ".7z.002", ".smpf", ".hbe", ".pf", ".ecs", ".tar.lzma", ".zip",
                    ".par", ".b1", ".cbr", ".pak", ".cbz", ".rev", ".tar.xz", ".jsonlz4", ".ita", ".nex", ".kgb", ".taz", ".rpm", ".pit", ".pwa", ".npk", ".ark", ".f",
                    ".sfg", ".xip", ".pcv", ".spd", ".cdz", ".bh", ".gz", ".a02", ".pea", ".c00", ".tx_", ".piz", ".bundle", ".s7z", ".car", ".7z.001", ".hki", ".f3z",
                    ".apz", ".lzma", ".tar.lz", ".zix", ".ar", ".cxarchive", ".sfx", ".alz", ".opk", ".ari", ".zl", ".czip", ".z03", ".ctz", ".arc", ".r01", ".bz", "0",
                    ".tar.bz2", ".ipk", ".dd", ".snb", ".war", ".c01", ".sitx", ".zipx", ".z", ".gmz", ".a01", ".ace", ".xx", ".xz", ".tbz2", ".fdp", ".r03", ".shar",
                    ".sdn", ".voca", ".lpkg", ".rnc", ".zst", ".s01", ".s02", ".c10", ".ctx", ".oz", ".ufs.uzip", ".ba", ".pa", ".oar", ".gca", ".cb7", ".mbz", ".cbt",
                    ".p19", ".package", ".tgz", ".sdc", ".sea", ".r30", ".arj", ".pup", ".snappy", ".spt", ".par2", ".rz", ".archiver", ".tar.gz", ".mzp", ".a00", ".sfs",
                    ".zfsendtotarget", ".xez", ".sh", ".gzi", ".r0", ".lhzd", ".pack.gz", ".edz", ".jhh", ".fp8", ".paq8p", ".yz", "0", ".lz", ".rk", ".z02", ".lbr",
                    ".zsplit", ".jgz", ".whl", ".xar", ".hyp", ".shr", ".pet", ".j", ".tcx", ".bzip2", ".iadproj", ".lzh", ".srep", ".zi", ".zoo", ".pax", ".z01", ".fzbz",
                    ".lqr", ".warc", ".paq8f", ".zz", ".ize", ".nz", ".wdz", ".vmcz", ".agg", ".hki1", ".vsi", ".lzo", ".lnx", ".hki3", ".dgc", ".bza", ".efw", ".asr",
                    ".ipg", ".libzip", ".egg", ".cpt", ".cba", ".z04", ".mzp", ".zw", ".md", ".xmcdz", ".uc2", ".r04", ".tz", ".ain", ".isx", ".arh", ".epi", ".hbc",
                    ".txz", ".cpgz", ".wastickers", ".lha", ".layout", ".rss", ".zap", ".spm", ".b64", ".tg", ".wux", ".hpk", ".xzm", ".c02", ".vpk", ".sar", ".pae",
                    ".yz1", ".uzip", ".bzip", ".hpkg", ".jic", ".xef", ".paq6", ".tar.z", ".waff", ".puz", ".zi_", ".bdoc", ".r02", ".mou", ".tlz", ".kz", ".trs",
                    ".dist", ".sbx", ".r21", ".sen", ".vem", ".tlzma", ".r1", ".cp9", ".hbc2", ".ha", ".psz", ".jex", ".tar.gz2", ".comppkg_hauptwerk_rar", ".stproj",
                    ".pvmz", ".wlb", ".hki2", ".tzst", ".spl", ".vwi", ".s09", ".prs", ".p7z", ".p01", ".apex", ".paq8l", ".lzx", ".paq7", ".vfs", ".daf", ".ish",
                    ".boo", ".si", ".paq8", ".y", ".sbx", ".pim", ".wot", ".shk", ".sqz", ".ecsbx", ".gar", };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public class MicrosoftOffice
        {
            public static string[] Word
            {
                get
                {
                    string[] ext = new string[] { ".doc", ".dot", ".wbk", ".docx", ".docm", ".dotx", ".dotm", ".docb", ".rtf", ".wpd", ".odt" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] Excel
            {
                get
                {
                    string[] ext = new string[] { ".xls", ".xlt", ".xlm", ".xlsx", ".xlsm", ".xltx", ".xltm", ".xlsb", ".xla", ".xlam", ".xll", ".xlw", ".ods" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] PowerPoint
            {
                get
                {
                    string[] ext = new string[] { ".ppt", ".pot", ".pps", ".pptx", ".pptm", ".potx", ".potm", ".ppsx", ".ppsm", ".sldx", ".sldm", ".odp" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] Access
            {
                get
                {
                    string[] ext = new string[] { ".accdb ", ".accde", ".accdt ", ".accdr", };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Visio
            {
                get
                {
                    string[] ext = new string[] { ".vsd ", ".vsdx" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Project
            {
                get
                {
                    string[] ext = new string[] { ".mpp ", ".mpt" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Publisher
            {
                get
                {
                    string[] ext = new string[] { ".pub" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Outlook
            {
                get
                {
                    string[] ext = new string[] { ".msg", ".eml", ".ics", ".vcf" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] XPS
            {
                get
                {
                    string[] ext = new string[] { ".xps" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }
        }

        public static string[] MFilesAssetCertificateValidTypes
        {
            get
            {
                return new string[] { ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".doc", ".docx", ".bmp", ".raw", ".otd", ".pjpeg", ".jjiff" };
            }
        }

        public static string[] FileTypesSometimesFoundInFolderYouWantToIgnore
        {
            get
            {
                string[] ext = new string[] { ".lnk", ".thmx", ".db", ".css", ".gif", ".tmp",". nh", ".aspx", ".b", ".dat", ".d", ".fil", ".gif", ".heic", ".ics", ".bat",
                                              ".cmd", ".jnt", ".js", ".km", ".lnk", ".lst", ".mht", ".n", ".nbf", ".nrg", ".nrp", ".shs", ".thmx", ".exe", ".dll", ".url", ".cs", ".mdf" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }
    }
}
