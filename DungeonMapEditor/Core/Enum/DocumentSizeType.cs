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
    public enum DocumentSizeType
    {
        [Description("A3 sheet")]
        Document_A3,
        [Description("A4 sheet")]
        Document_A4,
        [Description("A5 sheet")]
        Document_A5,
        [Description("HD file")]
        Image_HD,
        [Description("FullHD file")]
        Image_FullHD,
        [Description("2K file")]
        Image_2K,
        [Description("4K file")]
        Image_4K,
        [Description("Custom size")]
        Custom
    }
}
