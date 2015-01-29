using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk2nm
{
        class CLGeneric
        {
                public Filter Filter
                { 
                        get; 
                        set; 
                }

                public String Include
                {
                        get;
                        set;
                }

                public CLType Type
                {
                        get;
                        set;
                }

                public string ToString(bool compact)
                {
                        String ret;
                        String t;

                        switch (Type)
                        { 
                                case CLType.Compile:
                                        t = "ClCompile";
                                        break;

                                case CLType.Include:
                                        t = "ClInclude";
                                        break;

                                default:
                                        t = "None";
                                        break;
                        }

                        if (compact)
                        {
                                ret = "<" + t + " Include=\"" + Include + "\" />";
                        }
                        else
                        {
                                ret = "<" + t + " Include=\"" + Include + "\">";
                                ret += "<Filter>" + Filter.Include + "</Filter>";
                                ret += "</" + t + ">";
                        }

                        return ret;
                }
        }
}
