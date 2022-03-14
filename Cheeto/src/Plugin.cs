using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace Cheeto
{
    [BepInPlugin("warmode-cheeto", "cucumba", "1.4.6")]
    public class Plugin : BepInEx.IL2CPP.BasePlugin
    {
        public override void Load()
        {
            Log.LogInfo("cucumba loaded");
            Cheeto.Run(this);
        }
    }
}
