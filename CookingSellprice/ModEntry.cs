using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace CookingSellprice
{
    public class ModEntry : Mod, IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                //iterate through cooking recipes
                foreach (KeyValuePair<string, string> pair in CraftingRecipe.cookingRecipes)
                {
                    string[] recipe = pair.Value.Split('/');
                    string[] ingredients = recipe[0].Split(' ');
                    int price = 50;
                    for (int i = 0; i < ingredients.Length; i += 2)
                    {
                        switch (Convert.ToInt32(ingredients[i]))
                        {
                            //Fish Category
                            case -4:
                                price += 100;
                                break;
                            //EggEggEgg
                            case -5:
                                price += 50;
                                break;
                            //Milk Category
                            case -6:
                                price += 125;
                                break;
                            default:
                                string[] information = data[Convert.ToInt32(ingredients[i])].Split('/');
                                price += Convert.ToInt32(information[1]) * Convert.ToInt32(ingredients[i + 1]);
                                break;
                        }
                    }
                    string[] fields = data[Convert.ToInt32(recipe[2])].Split('/');
                    //Don't decrease price
                    if (Convert.ToInt32(fields[1]) < price)
                    {
                        fields[1] = $"{price}";
                        string newinfo = string.Join("/", fields);
                        data[Convert.ToInt32(recipe[2])] = newinfo;
                    }
                    //Monitor.Log($"new price for {pair.Key}: {price}");
                }
            }
        }

        public override void Entry(IModHelper helper)
        {
            //Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        /*
        public void OnGameLaunched (object sender, GameLaunchedEventArgs e)
        {
            //Lucky Lunch(225G/250), Pink Cake(450G/480), Rhubarb Pie(370G/400), Eggplant Parm.(170G/200), Pale Broth(100G/150), Stir Fry(325G/335), Fruit Salad(400G/450), Blackberry Cobbler(190G/260), Fiddlehead Risotto(300G/350)
            foreach(KeyValuePair<string,string> pair in CraftingRecipe.cookingRecipes)
            {
                string[] recipe = pair.Value.Split('/');
                string[] ingredients = recipe[0].Split(' ');
                int price = 50;
                for (int i=0;i<ingredients.Length;i+=2)
                {
                    switch (Convert.ToInt32(ingredients[i]))
                    {
                        //Fish Category
                        case -4:
                            price += 100;
                            break;
                        //EggEggEgg
                        case -5:
                            price += 50;
                            break;
                        //Milk Category
                        case -6:
                            price += 125;
                            break;
                        default:
                            string[] information = Game1.objectInformation[Convert.ToInt32(ingredients[i])].Split('/');
                            price += Convert.ToInt32(information[1]) * Convert.ToInt32(ingredients[i + 1]);
                            break;
                    }
                }
                string[] info = Game1.objectInformation[Convert.ToInt32(recipe[2])].Split('/');
                //Don't decrease price
                if(Convert.ToInt32(info[1]) < price)
                {
                    info[1] = $"{price}";
                    string newinfo = String.Join("/", info);
                    Game1.objectInformation[Convert.ToInt32(recipe[2])] = newinfo;
                }
                //Monitor.Log($"new price for {pair.Key}: {price}");
            }
        }
        */
    }
}
