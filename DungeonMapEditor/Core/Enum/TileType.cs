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
    public enum TileType
    {
        [Description("Wall")]
        Wall,
        [Description("Outer corner")]
        Corner,
        [Description("Inner corner")]
        Corner_Inner,
        [Description("Door")]
        Door
    }
}
