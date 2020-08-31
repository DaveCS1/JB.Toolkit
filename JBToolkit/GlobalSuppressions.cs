// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.AssemblyHelper.EmbeddedResourceHelper.GetEmbeddedResourcePath(System.Reflection.Assembly,System.String,System.String,System.Boolean)~System.String")]
[assembly: SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.AssemblyHelper.EmbeddedResourceHelper.InternalGetEmbeddedResourcePathFromJBToolkit(System.String,System.Boolean)~System.String")]
[assembly: SuppressMessage("Security", "CA5379:Do Not Use Weak Key Derivation Function Algorithm", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Encryption.StringEncrypt.Encrypt(System.String)~System.String")]
[assembly: SuppressMessage("Security", "CA5379:Do Not Use Weak Key Derivation Function Algorithm", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Encryption.StringEncrypt.Decrypt(System.String)~System.String")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "type", Target = "~T:JBToolkit.Domain.Impersonation.Impersonator")]
[assembly: SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>", Scope = "type", Target = "~T:JBToolkit.Domain.Impersonation.Win32NativeMethods")]
[assembly: SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Domain.Impersonation.Win32NativeMethods.LogonUser(System.String,System.String,System.String,System.Int32,System.Int32,System.IntPtr@)~System.Int32")]
[assembly: SuppressMessage("Style", "IDE0074:Use compound assignment", Justification = "<Pending>", Scope = "member", Target = "~P:JBToolkit.Images.ColourHelper.IntensityValue.ChildA")]
[assembly: SuppressMessage("Style", "IDE0074:Use compound assignment", Justification = "<Pending>", Scope = "member", Target = "~P:JBToolkit.Images.ColourHelper.IntensityValue.ChildB")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Zip.Native.ZipArchive.Dispose")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "type", Target = "~T:JBToolkit.Zip.Native.ZipArchive")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Windows.ConsoleHelper.ProgressBar.Dispose")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "type", Target = "~T:JBToolkit.Windows.ConsoleHelper.ProgressBar")]
[assembly: SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "<Pending>", Scope = "member", Target = "~M:JBToolkit.Windows.ConsoleHelper.Option.Parse``1(System.String,JBToolkit.Windows.ConsoleHelper.OptionContext)~``0")]
