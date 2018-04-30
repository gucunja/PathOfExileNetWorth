using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfExileNetWorth
{
    class League
    {
        public string id { get; set; }
        public bool selected { get; set; }
    }

    class Stash 
    {
        public int numTabs { get; set; }
        public List<Tab> tabs { get; set; }
        public List<Item> items { get; set; }
        //public List<CurrencyLayout> currencyLayout { get; set; }
    }

    class StashTabOnForm
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool active { get; set; }
        public string id { get; set; }

        public StashTabOnForm(Tab t)
        {
            name = t.n;
            type = t.type;
            active = false;
            id = t.id;
        }

        public void ActivateDeactivateStashTab(Dictionary<string,bool> activeStashTabs)
        {
            if(activeStashTabs.ContainsKey(id)) { active = activeStashTabs[id]; }
        }
    }

    public class Tab
    {
        public string n { get; set; }
        public int i { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public bool hidden { get; set; }
        public bool selected { get; set; }
        public Colour colour { get; set; }
        public string srcL { get; set; }
        public string srcC { get; set; }
        public string srcR { get; set; }
    }

    public class Colour
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
    }

    public class CurrencyLayout
    {
        public string x { get; set; }
        public string y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }

    class CharacterInventory
    {
        public List<Item> items { get; set; }
        public Character character { get; set; }
    }

    public class Character
    {
        public string name { get; set; }
        public string league { get; set; }
        public int classId { get; set; }
        public int ascendancyClass { get; set; }
        public string @class { get; set; }
        public int level { get; set; }
    }

    public class CharacterOnForm
    {
        public string name { get; set; }
        public string @class { get; set; }
        public int level { get; set; }
        public bool active { get; set; }
        
        public CharacterOnForm(Character c)
        {
            name = c.name;
            @class = c.@class;
            level = c.level;
            active = false;
        }

        public void ActivateDeactivateCharacter(Dictionary<string, bool> activeCharacters)
        {
            if (activeCharacters.ContainsKey(name)) { active = activeCharacters[name]; }
        }
    }

    class SkillTree
    {
        public int[] hashes { get; set; }
        public List<Item> items { get; set; }
        public List<JewelSlot> jewel_slots { get; set; }
    }

    public class JewelSlot
    {
        public string id { get; set; }
        public PassiveSkill passiveSkill { get; set; }
    }

    public class PassiveSkill
    {
        public string id { get; set; }
        public int hash { get; set; }
        public string name { get; set; }
    }

    public class Item
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string id { get; set; } //item id, will change if you use currency on it
        public List<Socket> sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public bool lockedToCharacter { get; set; }
        public string note { get; set; }
        public List<PropertyRequirement> properties { get; set; }
        public List<PropertyRequirement> requirements { get; set; }
        public List<string> explicitMods { get; set; }
        public List<string> implicitMods { get; set; }
        public List<string> enchantMods { get; set; }
        public List<string> craftedMods { get; set; }
        public List<string> flavourText { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public List<Item> socketedItems { get; set; }
        public List<PropertyRequirement> additionalProperties { get; set; }
        public string secDescrText { get; set; }
        public string descrText { get; set; }
        public string artFilename { get; set; } //Divination card
        public bool duplicated { get; set; }
        public int maxStackSize { get; set; }
        public List<PropertyRequirement> nextLevelRequirements { get; set; }
        public int stackSize { get; set; }
        public int talismanTier { get; set; }
        public List<string> utilityMods { get; set; }//flask utility mods
        public string prophecyDiffText { get; set; }
        public string prophecyText { get; set; }
        public bool isRelic { get; set; }
    }

    public class Socket
    {
        public int group { get; set; }
        public string attr { get; set; }
    }

    public class PropertyRequirement
    {
        public string name { get; set; }
        public List<object> values { get; set; }
        public int displayMode { get; set; }
        public int @type { get; set; }
        public double progress { get; set; }
    }

    public class PropertyRequirementValue
    {
        public string variable1 { get; set; }
        public int variable2 { get; set; }
    }

    public class ItemOnForm
    {
        public string name { get; set; }
        public string source { get; set; }
        public int numberOfItems { get; set; }
        public float price { get; set; }
        public string id { get; set; } 

        public ItemOnForm(Item i, Dictionary<string, float> priceOf)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (Socket s in i.sockets ?? Enumerable.Empty<Socket>())
            {
                if (dict.ContainsKey(s.group)) { dict[s.group]++; } else { dict.Add(s.group, 1); }
            }

            int links = 0;
            if (dict.Count !=0) { links = dict.Values.Max(); }
            if (links <= 4) { links = 0; }

            if (i.name != "") { name = i.name + "_" + links; } else { name = i.typeLine + "_" + links; }
            if(i.frameType == 1 || i.frameType == 2) { name = i.typeLine + "_" + links; }

            name = name.Replace("<<set:MS>><<set:M>><<set:S>>", null);
            name = name.Replace("Superior", null);
            name = name.Replace("Mirrored", null);
            name = name.Trim(' ');

            source = i.inventoryId;
            numberOfItems = Math.Max(1,i.stackSize);
            if (priceOf.ContainsKey(name)) { price = priceOf[name]; }

            id = i.id;
        }
    }
         
}
