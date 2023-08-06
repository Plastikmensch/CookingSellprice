using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace CookingSellprice
{
    public class ModEntry : Mod
    {
        // Should be a ModConfig option
        private bool debug = false;

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                    // Iterate through cooking recipes
                    foreach (KeyValuePair<string, string> pair in CraftingRecipe.cookingRecipes)
                    {
                        string[] recipe = pair.Value.Split('/');
                        if (debug) Monitor.Log("recipe " + string.Join(",", recipe));
                        string[] ingredients = recipe[0].Split(' ');
                        // Some mods specify amount, even if it defaults to 1,
                        // to avoid bugs, amount gets omitted
                        string yield = recipe[2].Split(' ')[0];
                        // the base value of cooked items
                        int price = 50;
                        // Iterate over ingredients
                        for (int i = 0; i < ingredients.Length; i += 2)
                        {
                            switch (int.Parse(ingredients[i]))
                            {
                                // Fish Category
                                case -4:
                                    price += 100 * int.Parse(ingredients[i + 1]);
                                    break;
                                // EggEggEgg
                                case -5:
                                    price += 50 * int.Parse(ingredients[i + 1]);
                                    break;
                                // Milk Category
                                case -6:
                                    price += 125 * int.Parse(ingredients[i + 1]);
                                    break;
                                default:
                                    string[] information = data[int.Parse(ingredients[i])].Split('/');
                                    price += int.Parse(information[1]) * int.Parse(ingredients[i + 1]);
                                    break;
                            }
                        }
                        if (debug) Monitor.Log("recipe[2] " + yield);
                        string[] fields = data[int.Parse(yield)].Split('/');
                        // Don't decrease price
                        if (int.Parse(fields[1]) < price)
                        {
                            fields[1] = price.ToString();
                            string newinfo = string.Join("/", fields);
                            data[int.Parse(yield)] = newinfo;
                        }
                        //Monitor.VerboseLog($"new price for {pair.Key}: {price}");
                    }
                });
            }
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }
    }
}
