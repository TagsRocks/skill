using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.Sounds
{
    /// <summary>
    /// Defines sound categories. one of benefits of categorizing sounds is to play sounds in deferent volumes and allow player choose volume of each category 
    /// </summary>
    public enum SoundCategory
    {
        /// <summary> None </summary>
        None,
        /// <summary> Sound Effects ( like : gunfire, collisions, ... ) </summary>
        FX,
        /// <summary> Background music </summary>
        Music,
        /// <summary> Dialog of characters </summary>
        Dialog
    }
}
