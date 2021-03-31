﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Combat
{
    public class Player : Placeable
    {
        public Player(string name, string description, double rotation, string guid, string imagePath, double tileRatioX, double tileRatioY)
            : base(name, description, rotation, guid, imagePath, tileRatioX, tileRatioY)
        {

        }
    }
}
