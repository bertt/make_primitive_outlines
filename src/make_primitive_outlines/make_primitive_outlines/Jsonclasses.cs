using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace make_primitive_outlines
{
}

public class Rootobject
{
    public string[] extensionsUsed { get; set; }
}


//public class Rootobject
//{
//    public Extensions extensions { get; set; }
//}

public class Extensions
{
    public CESIUM_Primitive_Outline CESIUM_primitive_outline { get; set; }
}

public class CESIUM_Primitive_Outline
{
    public int indices { get; set; }
}
