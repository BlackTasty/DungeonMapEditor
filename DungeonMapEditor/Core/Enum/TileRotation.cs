using DungeonMapEditor.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Enum
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TileRotation
    {
        [Description("0 Degrees")]
        Degrees_0,
        [Description("90 Degrees")]
        Degrees_90,
        [Description("180 Degrees")]
        Degrees_180,
        [Description("270 Degrees")]
        Degrees_270
    }
}
