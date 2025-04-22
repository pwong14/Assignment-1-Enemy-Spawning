using System;
using System.Collections.Generic;

[Serializable]
public class Level
{
    public string      name;                 // "Easy", "Medium", …
    public int         waves = 0;            // 0 ⇒ endless
    public List<Spawn> spawns;
}
