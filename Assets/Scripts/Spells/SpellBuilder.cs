using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;



public class SpellBuilder
{

    Dictionary<string, Spell> spell_types = new Dictionary<string, Spell>();

    public Spell Build(SpellCaster owner)
    {
        return new Spell(owner);
    }

   
    public SpellBuilder()
    {        

    }

    void Start()
    {
        var spelltext = Resources.Load<TextAsset>("spells");

        JToken jo = JToken.Parse(spelltext.text);

        foreach (var spell in jo) {
            Spell s = spell.ToObject<Spell>();
            spell_types[s.GetName()] = s;
            Debug.Log(s.GetName());
        }
    }

}