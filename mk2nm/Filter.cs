using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk2nm
{
        class Filter
        {
                public String Include 
                { 
                        get; 
                        set; 
                }

                public Guid UniqueIdentifier
                {
                        get;
                        set;
                }

                public Filter(String name)
                {
                        Include = name;
                        UniqueIdentifier = Guid.NewGuid();
                }

                public override string ToString()
                {
                        String ret = "<Filter Include=\"" + Include + "\">";

                        if (UniqueIdentifier != null)
                        {
                                ret += "<UniqueIdentifier>" + UniqueIdentifier.ToString() + "</UniqueIdentifier>";
                        }

                        ret += "</Filter>";

                        return ret;
                                
                }
        }
}
