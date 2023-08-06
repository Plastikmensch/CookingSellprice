using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace CookingSellprice
{
    public class ModEntry : Mod
    {
        //TODO: Should be a ModConfig option
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
                        DebugLog("recipe " + string.Join(",", recipe));
                        string[] ingredients = recipe[0].Split(' ');
                        // Some mods specify amount, even if it defaults to 1,
                        // to avoid bugs, amount gets omitted
                        int cookedItemId = int.Parse(recipe[2].Split(' ')[0]);
                        // the base value of cooked items
                        int price = 50;

                        // Iterate over ingredients
                        for (int i = 0; i < ingredients.Length; i += 2)
                        {
                            int ingredientId = int.Parse(ingredients[i]);
                            int ingredientAmount = int.Parse(ingredients[i + 1]);

                            switch (ingredientId)
                            {
                                // Fish Category
                                case -4:
                                    price += 100 * ingredientAmount;
                                    break;
                                // EggEggEgg
                                case -5:
                                    price += 50 * ingredientAmount;
                                    break;
                                // Milk Category
                                case -6:
                                    price += 125 * ingredientAmount;
                                    break;
                                default:
                                    string[] ingredientData = data[ingredientId].Split('/');
                                    price += int.Parse(ingredientData[1]) * ingredientAmount;
                                    break;
                            }
                        }

                        DebugLog("recipe[2] " + cookedItemId);
                        // Get cooked item data
                        string[] fields = data[cookedItemId].Split('/');
                        string oldPrice = fields[1];

                        // Don't decrease price
                        if (int.Parse(oldPrice) < price)
                        {
                            fields[1] = price.ToString();
                            string newinfo = string.Join("/", fields);
                            data[cookedItemId] = newinfo;
                            DebugLog($"new price for {pair.Key}: {price}. Was: {oldPrice}");
                        }
                        else
                        {
                            DebugLog($"price for {pair.Key}: {oldPrice}");
                        }

                        // Used to create the table in the Readme. Convenient.
                        //Monitor.Log($"{pair.Key} | {fields[1]}g");
                    }
                });
            }
        }

        // Used to log verbose information
        // Less spammy, since verbose logging in SMAPI will print verbose log of every installed mod
        private void DebugLog(string message)
        {
            if (debug)
            {
                Monitor.Log(message);
            }
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }
    }
}
