using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk2nm
{
        class FiltersManager
        {
                // Standard Visual Studio "Headers Files" filter.
                public Filter HeaderFiles
                {
                        get;
                        set;
                }

                public Filter MakeFiles
                {
                        get;
                        set;
                }

                // Standard Visual Studio "Source Files" filter.
                public Filter SourceFiles
                {
                        get;
                        set;
                }

                public Dictionary<Guid, Filter> FilterMap
                {
                        get;
                        protected set;
                }

                public FiltersManager()
                {
                        FilterMap = new Dictionary<Guid, Filter>();

                        HeaderFiles = AddNewFilter("Header Files");
                        MakeFiles   = AddNewFilter("Make Files");
                        SourceFiles = AddNewFilter("Source Files");
                }

                // Add a new filter in the managed map; do not add already existing ones.
                public Filter AddNewFilter(String name)
                {
                        Filter f = GetFilterByName(name);
                        String[] t;     // Tokens.
                        String s = "";  // Single-step.

                        // Filter already exists.
                        if (f != null)
                        {
                                return f; 
                        }

                        // Unexisting filter case here:

                        f = new Filter(name);

                        // We don't want similar values. Continue in selecting an non-existing one.
                        while(FilterMap.ContainsKey(f.UniqueIdentifier))
                        {
                                f.UniqueIdentifier = Guid.NewGuid();
                        }

                        FilterMap.Add(f.UniqueIdentifier, f);

                        // Adds also all the intermediates filters.
                        t = name.Split('\\');

                        for (int i = 0; i < t.Length; i++)
                        {
                                if(i != 0)
                                {
                                        s += "\\";
                                }

                                s += t[i];

                                AddNewFilter(s);
                        }

                        return f;
                }

                // Gets a filter starting by it's include name.
                public Filter GetFilterByName(String name)
                {
                        foreach (KeyValuePair<Guid, Filter> k in FilterMap)
                        {
                                if (k.Value.Include.Equals(name))
                                {
                                        return k.Value;
                                }
                        }

                        return null;
                }
        }
}
