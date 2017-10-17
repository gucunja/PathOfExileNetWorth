using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PathOfExileNetWorth
{
    public class NinjaPrices
    {
        //this dictionary contains prices of all items from poe.ninja
        //manually added Chaos Orb and shard prices, they are not published on ninja api
        public static Dictionary<string, float> priceOf = new Dictionary<string, float>()
        {
            { "Chaos Orb_0", 1.0f},
            { "Chaos Shard_0", 0.05f},
            { "Alteration Shard_0", 0.0f },
            { "Transmutation Shard_0", 0.0f},
            { "Horizon Shard_0", 0.0f},
            { "Harbinger's Shard_0", 0.0f},
            { "Ancient Shard_0", 0.0f},
            { "Regal Shard_0", 0.0f},
            { "Scroll of Wisdom_0", 0.0f},
            { "Engineer's Shard_0", 0.0f},
            { "Binding Shard_0", 0.0f},
            { "Alchemy Shard_0", 0.0f}
        };

        public static bool isRefreshingActive = false;

        public static void RefreshPrices(List<NinjaCurrencyData> ccyPrices, List<NinjaItemData> itemPrices)
        {
            //get currency and fragment prices
            foreach (NinjaCurrencyData ccyP in ccyPrices)
            {
                foreach (CurrencyLine line in ccyP.lines)
                {
                    if (priceOf.ContainsKey(line.currencyTypeName + "_0"))
                    {
                        priceOf[line.currencyTypeName + "_0"] = line.chaosEquivalent;
                    }
                    else
                    {
                        priceOf.Add(line.currencyTypeName + "_0", line.chaosEquivalent);
                    }
                }
            }

            //get all other item prices
            foreach (NinjaItemData itemP in itemPrices)
            {
                foreach (Line line in itemP.lines)
                {
                    if (priceOf.ContainsKey(line.name +"_" + line.links))
                    {
                        priceOf[line.name + "_" + line.links] = line.chaosValue;
                    }
                    else
                    {
                        priceOf.Add(line.name + "_" + line.links, line.chaosValue);
                    }
                }
            }

            //manually update shard prices
            priceOf["Transmutation Shard_0"] = priceOf["Orb of Transmutation_0"] * 0.0f;
            priceOf["Alteration Shard_0"] = priceOf["Orb of Alteration_0"] * 0.05f;
            priceOf["Regal Shard_0"] = priceOf["Regal Orb_0"] * 0.05f;
            priceOf["Alchemy Shard_0"] = priceOf["Orb of Alchemy_0"] * 0.05f;
            priceOf["Binding Shard_0"] = priceOf["Orb of Binding_0"] * 0.05f;
            priceOf["Horizon Shard_0"] = priceOf["Orb of Horizons_0"] * 0.05f;
            priceOf["Engineer's Shard_0"] = priceOf["Engineer's Orb_0"] * 0.05f;
            priceOf["Harbinger's Shard_0"] = priceOf["Harbinger's Orb_0"] * 0.05f;
            priceOf["Ancient Shard_0"] = priceOf["Ancient Orb_0"] * 0.05f;
            priceOf["Scroll of Wisdom_0"] = priceOf["Portal Scroll_0"] * 0.25f;
        }
    }


    public class NinjaItemData
    {
        public List<Line> lines { get; set; }
    }

    public class Line
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public int mapTier { get; set; }
        public int levelRequired { get; set; }
        public string baseType { get; set; }
        public int stackSize { get; set; }
        public string variant { get; set; }
        public string prophecyText { get; set; }
        public string artFilename { get; set; }
        public int links { get; set; }
        public int itemClass { get; set; }
        public SparkLine sparkline { get; set; }
        public List<ImplicitModifier> implicitModifiers { get; set; }
        public List<ExplicitModifier> explicitModifiers { get; set; }
        public string flavourText { get; set; }
        public string itemType { get; set; }
        public float chaosValue { get; set; }
        public float exaltedValue { get; set; }
        public int count { get; set; }
    }

    public class NinjaCurrencyData
    {
        public List<CurrencyLine> lines { get; set; }
        public List<CurrencyDetail> currencyDetails { get; set; }
    }

    public class CurrencyLine
        {
            public string currencyTypeName { get; set; }
            public PayReceive pay { get; set; }                
            public PayReceive receive { get; set; }           
            public SparkLine paySparkLine { get; set; }        
            public SparkLine receiveSparkLine { get; set; }
            public float chaosEquivalent { get; set; }
        }

    public class PayReceive
    {
        public int id { get; set; }
        public int league_id { get; set; }
        public int pay_currency_id { get; set; }
        public int get_currency_id { get; set; }
        public string sample_time_utc { get; set; }
        public int count { get; set; }
        public float value { get; set; }
        public int data_point_count { get; set; }
    }

    public class SparkLine
    {
        public float?[] data { get; set; }
        public float totalChange { get; set; }
    }

    public class CurrencyDetail
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public int poeTradeId { get; set; }
    }

    public class ExplicitModifier
    {
        public string text { get; set; }
        public bool optional { get; set; }
    }

    public class ImplicitModifier
    {
        public string text { get; set; }
    }

}
