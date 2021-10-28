using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.TowerSets;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using CardMonkey.Displays.Projectiles;
using MelonLoader;

namespace Magical Monkey
{
    /// <summary>
    /// The main class that adds the core tower to the game
    /// </summary>
    public class MagicMonkey : ModTower
    {
        // public override string Portrait => "Don't need to override this, using the default of Name-Portrait";
        // public override string Icon => "Don't need to override this, using the default of Name-Icon";

        public override string TowerSet => Magic;
        public override string BaseTower => TowerType.Wizard;
        public override int Cost => 1200;

        public override int TopPathUpgrades => 5;
        public override int MiddlePathUpgrades => 5;
        public override int BottomPathUpgrades => 5;
        public override string Description => "Uses magical powers to take down the bloons!!!";

        public override string DisplayName => "Magical Monkey"
        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.range += 15;
            var attackModel = towerModel.GetAttackModel();
            attackModel.range += 15;

            var projectile = attackModel.weapons[0].projectile;
            projectile.ApplyDisplay<RedCardDisplay>(); // Make the projectiles look like Cards
            projectile.pierce += 4;
        }

        /// <summary>
        /// Make Card Monkey go right after the Boomerang Monkey in the shop
        /// <br/>
        /// If we didn't have this, it would just put it at the end of the Primary section
        /// </summary>
        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return towerSet.First(model => model.towerId == TowerType.BoomerangMonkey).towerIndex + 1;
        }

        /// <summary>
        /// Support the Ultimate Crosspathing Mod by generating all the Tower Tiers if the mod exists
        /// <br/>
        /// That mod will handle actually allowing the upgrades to happen in the UI
        /// </summary>
        public override IEnumerable<int[]> TowerTiers()
        {
            if (MelonHandler.Mods.OfType<BloonsTD6Mod>().Any(m => m.GetModName() == "UltimateCrosspathing"))
            {
                for (var top = 0; top <= TopPathUpgrades; top++)
                {
                    for (var mid = 0; mid <= MiddlePathUpgrades; mid++)
                    {
                        for (var bot = 0; bot <= BottomPathUpgrades; bot++)
                        {
                            yield return new[] { top, mid, bot };
                        }
                    }
                }
            } else
            {
                foreach (var towerTier in base.TowerTiers())
                {
                    yield return towerTier;
                }
            }
        }
    }
}
