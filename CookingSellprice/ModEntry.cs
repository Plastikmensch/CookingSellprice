using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewModdingAPI.Events;

namespace CookingSellprice
{
    public class ModEntry : Mod
    {
        //TODO: Should be a ModConfig option
        private bool debug = false;

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects") && CraftingRecipe.cookingRecipes != null)
            {
                e.Edit(asset =>
                {
                    IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

                    // Iterate through cooking recipes
                    foreach (KeyValuePair<string, string> pair in CraftingRecipe.cookingRecipes)
                    {
                        string[] recipe = pair.Value.Split('/');
                        DebugLog("recipe " + string.Join(",", recipe));
                        string[] ingredients = recipe[0].Split(' ');
                        // Some mods specify amount, even if it defaults to 1,
                        // to avoid bugs, amount gets omitted
                        string cookedItemId = recipe[2].Split(' ')[0];
                        // the base value of cooked items
                        int price = 50;

                        // Iterate over ingredients
                        for (int i = 0; i < ingredients.Length; i += 2)
                        {
                            string ingredientId = ingredients[i];
                            int ingredientAmount = int.Parse(ingredients[i + 1]);

                            switch (ingredientId)
                            {
                                // Fish Category
                                case "-4":
                                    price += 100 * ingredientAmount;
                                    break;
                                // EggEggEgg
                                case "-5":
                                    price += 50 * ingredientAmount;
                                    break;
                                // Milk Category
                                case "-6":
                                    price += 125 * ingredientAmount;
                                    break;
                                default:
                                    price += data[ingredientId].Price * ingredientAmount;
                                    break;
                            }
                        }

                        DebugLog("cookedItemId " + cookedItemId);
                        int oldPrice = data[cookedItemId].Price;

                        // Don't decrease price
                        if (oldPrice < price)
                        {
                            data[cookedItemId].Price = price;
                            DebugLog($"new price for {pair.Key}: {price}. Was: {oldPrice}");
                        }
                        else
                        {
                            DebugLog($"price for {pair.Key}: {oldPrice}");
                        }

                        // Used to create the table in the Readme. Convenient.
                        //Monitor.Log($"{pair.Key} | {data[cookedItemId].Price}g");
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

        ///  <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }
    }
}
