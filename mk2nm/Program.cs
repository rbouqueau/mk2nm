using System;
using System.Collections.Generic;
using System.IO;

namespace mk2nm
{
        class Program
        {
                static StreamWriter prjS = null;
                static StreamWriter filtS = null;
                static FiltersManager fmgr = new FiltersManager();
                static String root = null;
                static String relative = null;
                static List<CLGeneric> clitems = new List<CLGeneric>();

                static void Main(string[] args)
                {
                        if (args.Length != 2)
                        {
                                Console.WriteLine("Bad arguments!\nUsage: mk2nm.exe <Project Name> <Project sources root>\n");
                        }
                        else 
                        {
                                root = args[1];
                                relative = GetRelativePath(root, System.Reflection.Assembly.GetExecutingAssembly().Location);
                                
                                ProcessDirectory(root);

                                // Prepare custom project files.
                                PrepareOutputs(args[0]);
                        }
                }

                static string GetRelativePath(string filespec, string folder)
                {
                        Uri pathUri = new Uri(filespec);
                        Uri folderUri = new Uri(folder);

                        // Folders must end in a slash.
                        if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                                folder += Path.DirectorySeparatorChar;
                        }

                        return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
                }

                static void PrepareOutputs(String name)
                {
                        List<String> hi = new List<String>(); // Headers included.

                        prjS = new StreamWriter(".\\" + name + ".vcxproj", false);
                        filtS = new StreamWriter(".\\" + name + ".vcxproj.filters", false);

                        prjS.Write(
                                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                "<Project DefaultTargets=\"Build\" ToolsVersion=\"12.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\n" +
                                "    <ItemGroup Label=\"ProjectConfigurations\">\n" +
                                "        <ProjectConfiguration Include=\"Debug|Win32\">\n" +
                                "            <Configuration>Debug</Configuration>\n" +
                                "            <Platform>Win32</Platform>\n" +
                                "        </ProjectConfiguration>\n" +
                                "    </ItemGroup>\n");
                        
                        //
                        // Write the header item group section.
                        //

                        prjS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        { 
                                if(c.Type == CLType.Include)
                                {
                                        prjS.Write("        " + c.ToString(true));
                                }
                        }

                        prjS.Write("    </ItemGroup>\n");

                        // Write the makefile item group section.

                        prjS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        {
                                if (c.Type == CLType.None)
                                {
                                        prjS.Write("        " + c.ToString(true));
                                }
                        }

                        prjS.Write("    </ItemGroup>\n");
                        
                        //
                        // Write the source item group section.
                        //

                        prjS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        {
                                if (c.Type == CLType.Compile)
                                {
                                        prjS.Write("        " + c.ToString(true));
                                }
                        }

                        prjS.Write("    </ItemGroup>\n");

                        prjS.Write(
                                "    <PropertyGroup Label=\"Globals\">\n" +
                                "        <ProjectGuid>" + Guid.NewGuid() + "</ProjectGuid>\n" +
                                "        <Keyword>MakeFileProj</Keyword>\n" +
                                "    </PropertyGroup>\n" +
                                "    <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.Default.props\" />\n" +
                                "    <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|Win32'\" Label=\"Configuration\">\n" +
                                "        <ConfigurationType>Makefile</ConfigurationType>\n" +
                                "        <UseDebugLibraries>true</UseDebugLibraries>\n" +
                                "        <PlatformToolset>v120</PlatformToolset>\n" +
                                "    </PropertyGroup>\n" +
                                "    <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.props\" />\n" +
                                "    <ImportGroup Label=\"ExtensionSettings\">\n" +
                                "    </ImportGroup>\n" +
                                "    <PropertyGroup Label=\"UserMacros\" />\n" +
                                "    <ItemDefinitionGroup>\n" +
                                "    </ItemDefinitionGroup>\n" +
                                "    <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.targets\" />\n" +
                                "        <ImportGroup Label=\"ExtensionTargets\">\n" +
                                "    </ImportGroup>\n");


                        prjS.Write(
                                "    <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|Win32'\">\n" +
                                "        <NMakePreprocessorDefinitions></NMakePreprocessorDefinitions>\n" +
                                "        <NMakeIncludeSearchPath>");

                        // Include path where you have found any header.
                        foreach (CLGeneric c in clitems)
                        {
                                if (c.Type == CLType.Include)
                                {
                                        String s = relative + c.Filter.Include.Replace("Header Files", "") + "; ";

                                        // Avoid to insert already inserted header paths.
                                        if(!hi.Contains(s))
                                        {
                                                hi.Add(s);
                                                prjS.Write(s);
                                        }
                                }
                        }

                        prjS.Write(
                              "\n        </NMakeIncludeSearchPath>\n" +
                                "        <ExecutablePath />\n" +
                                "        <IncludePath />\n" +
                                "        <ReferencePath />\n" +
                                "        <LibraryPath />\n" +
                                "        <LibraryWPath />\n" +
                                "        <SourcePath></SourcePath>\n" +
                                "        <ExcludePath />\n" +
                                "    </PropertyGroup>\n");

                        prjS.Write(
                                "</Project>");

                        //
                        // End of vcxproj file.
                        //

                        filtS.Write(
                                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                "<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\n");

                        //
                        // Define the filters first.
                        //

                        filtS.Write("    <ItemGroup>\n");

                        //filtS.Write("        " + fmgr.SourceFiles.ToString() + "\n");
                        //filtS.Write("        " + fmgr.MakeFiles.ToString() + "\n");
                        //filtS.Write("        " + fmgr.HeaderFiles.ToString() + "\n");

                        foreach (Filter f in fmgr.FilterMap.Values)
                        {
                                filtS.Write("        " + f.ToString() + "\n");
                        }

                        filtS.Write("    </ItemGroup>\n");

                        //
                        // Header filters.
                        //

                        filtS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        {
                                if(c.Type == CLType.Include)
                                {
                                        filtS.Write("        " + c.ToString(false) + "\n");
                                }
                                
                        }

                        filtS.Write("    </ItemGroup>\n");

                        //
                        // Makefile filters.
                        //

                        filtS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        {
                                if (c.Type == CLType.None)
                                {
                                        filtS.Write("        " + c.ToString(false) + "\n");
                                }

                        }

                        filtS.Write("    </ItemGroup>\n");

                        //
                        // Compile filters.
                        //

                        filtS.Write("    <ItemGroup>\n");

                        foreach (CLGeneric c in clitems)
                        {
                                if (c.Type == CLType.Compile)
                                {
                                        filtS.Write("        " + c.ToString(false) + "\n");
                                }

                        }

                        filtS.Write("    </ItemGroup>\n");
                        
                        filtS.Write("</Project>");

                        //
                        // End of filters file.
                        //

                        prjS.Close();
                        filtS.Close();
                }

                static void ProcessDirectory(String path)
                {
                        if(!Directory.Exists(path))
                        {
                                return;
                        }

                        Console.WriteLine("Processing " + path + "...");

                        Filter f = null;

                        string [] subDirs = Directory.GetDirectories(path);
                        string [] subFiles = Directory.GetFiles(path);
                        
                        // Process every file first.
                        for (int i = 0; i < subFiles.Length; i++)
                        {

                                CLGeneric c = null;

                                // Source detected.
                                if (Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".c") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".cc") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".cpp") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".cxx") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".s") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".asm"))
                                {
                                        c = new CLCompile();
                                        c.Include = relative + subFiles[i].Replace(root, "");

                                        // First loop.
                                        if (!path.Equals(root))
                                        {
                                                f = fmgr.AddNewFilter(fmgr.SourceFiles.Include + "\\" + path.Replace(root + "\\", ""));
                                        }

                                        // First loop.
                                        if (f == null)
                                        {
                                                c.Filter = fmgr.SourceFiles;
                                        }
                                        else
                                        {
                                                c.Filter = f;
                                        }
                                }

                                // Header detected.
                                if (Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".h") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".hh") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".hpp") ||
                                        Path.GetExtension(subFiles[i]).ToLowerInvariant().Equals(".hxx"))
                                {
                                        c = new CLInclude();
                                        c.Include = relative + subFiles[i].Replace(root, "");

                                        // First loop.
                                        if (!path.Equals(root))
                                        {
                                                f = fmgr.AddNewFilter(fmgr.HeaderFiles.Include + "\\" + path.Replace(root + "\\", ""));
                                        }

                                        // First loop.
                                        if (f == null)
                                        {
                                                c.Filter = fmgr.HeaderFiles;
                                        }
                                        else
                                        {
                                                c.Filter = f;
                                        }
                                }

                                // Makefile detected.
                                if (Path.GetFileName(subFiles[i]).ToLowerInvariant().Equals("makefile"))
                                {
                                        c = new CLNone();
                                        c.Include = relative + subFiles[i].Replace(root, "");

                                        // First loop.
                                        if (!path.Equals(root))
                                        {
                                                f = fmgr.AddNewFilter(fmgr.MakeFiles.Include + "\\" + path.Replace(root + "\\", ""));
                                        }

                                        // First loop.
                                        if (f == null)
                                        {
                                                c.Filter = fmgr.MakeFiles;
                                        }
                                        else 
                                        {
                                                c.Filter = f;
                                        }
                                }

                                if(c != null)
                                {
                                        clitems.Add(c);
                                }
                        }

                        // Recursively process directories.
                        for (int i = 0; i < subDirs.Length; i++)
                        {
                                ProcessDirectory(subDirs[i]);
                        }
                }
        }
}
