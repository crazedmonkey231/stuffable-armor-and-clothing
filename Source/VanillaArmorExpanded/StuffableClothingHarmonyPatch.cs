using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaStuffableArmor
{
    [StaticConstructorOnStartup]
    class StuffableClothingHarmonyPatch
    {
        static StuffableClothingHarmonyPatch()
        {

            Harmony harmony = new Harmony("crazedmonkey231.stylestation.mood.patch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }

        [HarmonyPatch(typeof(RecipeDefGenerator), "SetIngredients")]
        [HarmonyPatch(new Type[] { typeof(RecipeDef), typeof(ThingDef), typeof(int) })]
        static class SetIngredients_patch
        {
            static void Prefix(RecipeDef r, ThingDef def, int adjustedCount)
            {

            }


        static void Postfix(RecipeDef r, ThingDef def, int adjustedCount)
            {
                Log.Message("setting ingredients");
                r.ingredients.Clear();
                r.adjustedCount = adjustedCount;
                if (def.IsApparel)
                {
                    IngredientCount ingredientCount = new IngredientCount();
                    if (def.CostStuffCount != 0)
                    {
                        ingredientCount.SetBaseCount((float)(def.CostStuffCount * adjustedCount));
                        ingredientCount.filter.SetAllowAllWhoCanMake(def);
                        r.ingredients.Add(ingredientCount);
                        r.fixedIngredientFilter.SetAllowAllWhoCanMake(def);
                        r.productHasIngredientStuff = true;
                    }
                    else
                    {
                        foreach (ThingDefCountClass cost in def.CostList)
                        {
                            ingredientCount = new IngredientCount();

                            ingredientCount.SetBaseCount((float)(cost.count * adjustedCount));
                            ingredientCount.filter.SetAllow(cost.thingDef, true);
                            r.ingredients.Add(ingredientCount);
                            r.fixedIngredientFilter.SetAllowAllWhoCanMake(def);
                            r.productHasIngredientStuff = true;
                        }

                    }
                }
                if (def.CostList == null)
                    return;
                foreach (ThingDefCountClass cost in def.CostList)
                {
                    IngredientCount ingredientCount = new IngredientCount();
                    ingredientCount.SetBaseCount((float)(cost.count * adjustedCount));
                    ingredientCount.filter.SetAllow(cost.thingDef, true);
                    r.ingredients.Add(ingredientCount);
                }
            }
        }
    }
}
