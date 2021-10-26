using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Main.Scenes;
using Assets.Scripts.Models;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Powers;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Assets.Scripts.Unity.UI_New.Upgrade;
using Assets.Scripts.Utils;
using Harmony;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;
using BTD_Mod_Helper.Extensions;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using System.Net;
using Assets.Scripts.Unity.UI_New.Popups;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper;
using UnhollowerRuntimeLib;
namespace bloonchipper {
  class Bloon_Chipper: BloonsMod {
    //https://github.com/gurrenm3/BloonsTD6-Mod-Helper/releases
    public override void OnApplicationStart() {
      base.OnApplicationStart();
      Console.WriteLine("bloonchipper mod loaded");
    }
    public class BloonChipper: ModTower {
      public override string BaseTower => "PatFusty 20";
      public override string Name => "Bloonchipper";
      public override int Cost => 0;
      public override string TowerSet => "Military";
      public override int TopPathUpgrades => 0;
      public override int MiddlePathUpgrades => 5;
      public override int BottomPathUpgrades => 0;
      public override string DisplayName => "Bloonchipper";
      public override string Description => "Rapidly sucks up and shreds bloons, spitting what's left out the back.";
      public override string Get2DTexture(int[] tiers) {
        return "BloonchipperDisplay";
      }
      public override bool Use2DModel => true;
      public override void ModifyBaseTowerModel(TowerModel towerModel) {
        towerModel.RemoveBehavior < HeroModel > ();
        towerModel.RemoveBehavior < CreateSoundOnBloonEnterTrackModel > ();
        towerModel.RemoveBehavior < CreateSoundOnBloonLeakModel > ();
        towerModel.RemoveBehavior < CreateSoundOnSelectedModel > ();
        towerModel.AddBehavior(Game.instance.model.GetTowerFromId("DartMonkey").GetBehavior < CreateSoundOnUpgradeModel > ().Duplicate());
        towerModel.RemoveBehavior < CreateEffectOnUpgradeModel > ();
        towerModel.AddBehavior(Game.instance.model.GetTowerFromId("DartMonkey").GetBehavior < CreateEffectOnUpgradeModel > ().Duplicate());
        var squeeze = towerModel.GetAbility(1).GetBehavior < ActivateAttackModel > ().attacks[0].Duplicate < AttackModel > ();
        squeeze.weapons[0].Rate = 5;
        squeeze.weapons[0].GetBehavior < SwitchAnimStateForBloonTypeModel > ().nonMoabsAnimId = 4;
        squeeze.weapons[0].GetBehavior < SwitchAnimStateForBloonTypeModel > ().moabAnimId = 4;
        squeeze.weapons[0].GetBehavior < SwitchAnimStateForBloonTypeModel > ().bfbAnimId = 4;
        squeeze.weapons[0].GetBehavior < SwitchAnimStateForBloonTypeModel > ().zomgAnimId = 4;
        squeeze.weapons[0].GetBehavior < SwitchAnimStateForBloonTypeModel > ().ddtAnimId = 4;
        squeeze.weapons[0].projectile.filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterOutTagModel("FilterOutTagModel_ProjectileSqueeze", "Moabs", new Il2CppStringArray(0)),
            new FilterInvisibleModel("FilterInvisibleModel_", true, false)
        });
        squeeze.weapons[0].projectile.GetBehavior < ProjectileFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterOutTagModel("FilterOutTagModel_ProjectileSqueeze", "Moabs", new Il2CppStringArray(0)),
            new FilterInvisibleModel("FilterInvisibleModel_", true, false)
        });
        squeeze.GetBehavior < AttackFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterOutTagModel("FilterOutTagModel_ProjectileSqueeze", "Moabs", new Il2CppStringArray(0)),
            new FilterInvisibleModel("FilterInvisibleModel_", true, false),
        });
        squeeze.GetBehavior < TargetStrongModel > ().isSelectable = true;
        squeeze.AddBehavior < TargetFirstModel > (new TargetFirstModel("TargetFirstModel_", true, false));
        squeeze.weapons[0].projectile.pierce = 999999 f;
        squeeze.weapons[0].projectile.maxPierce = 999999;
        squeeze.weapons[0].projectile.CapPierce(999999);
        towerModel.RemoveBehavior < AttackModel > ();
        towerModel.AddBehavior < AttackModel > (squeeze);
        towerModel.RemoveBehavior < AbilityModel > ();
        towerModel.RemoveBehavior < AbilityModel > ();
        towerModel.GetAttackModel().weapons[0].projectile.RemoveBehavior < CreateSoundOnDelayedCollisionModel > ();
      }
      public override string Icon => "Bloonchipper";
      public override string Portrait => "Bloonchipper";
      public override float PixelsPerUnit => 4.4 f;
    }
    public class LongerSucc: ModUpgrade < BloonChipper > {
      public override string Name => "LongerSucc";
      public override string DisplayName => "Longer Range";
      public override string Description => "Increased range and pierce.";
      public override int Cost => 0;
      public override int Path => MIDDLE;
      public override int Tier => 1;
      public override string Icon => "Bloonchipper";
      public override void ApplyUpgrade(TowerModel towerModel) {
        AttackModel attackModel = towerModel.GetBehavior < AttackModel > ();
        attackModel.weapons[0].projectile.pierce = 999999 f;
        attackModel.range += 999999;
        towerModel.range += 999999;
        attackModel.weapons[0].projectile.radius += 999999;
      }
    }
    public class FasterSucc: ModUpgrade < BloonChipper > {
      public override string Name => "FasterSucc";
      public override string DisplayName => "Faster Shred";
      public override string Description => "Attacks twice as fast.";
      public override int Cost => 0;
      public override int Path => MIDDLE;
      public override int Tier => 2;
      public override string Icon => "Bloonchipper";
      public override void ApplyUpgrade(TowerModel towerModel) {
        AttackModel attackModel = towerModel.GetBehavior < AttackModel > ();
        attackModel.weapons[0].projectile.pierce = 999999 f;
        attackModel.weapons[0].Rate *= 0;
        attackModel.weapons[0].projectile.GetBehavior < DelayBloonChildrenSpawningModel > ().Lifespan *= 4 f;
      }
    }
    public class MOABSucc: ModUpgrade < BloonChipper > {
      public override string Name => "MOABSucc";
      public override string DisplayName => "MOAB Killer";
      public override string Description => "Can now destroy moabs. Ability doubles attack speed.";
      public override int Cost => 0;
      public override int Path => MIDDLE;
      public override int Tier => 3;
      public override string Icon => "Bloonchipper";
      public override void ApplyUpgrade(TowerModel towerModel) {
        AttackModel attackModel = towerModel.GetBehavior < AttackModel > ();
        attackModel.weapons[0].projectile.GetBehavior < DelayBloonChildrenSpawningModel > ().Lifespan *= 0.5 f;
        attackModel.AddWeapon(attackModel.weapons[0].Duplicate < WeaponModel > ());
        attackModel.weapons[1].projectile.pierce = 999999;
        attackModel.weapons[1].projectile.maxPierce = 999999;
        attackModel.weapons[1].projectile.CapPierce(999999);
        attackModel.weapons[1].projectile.filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterWithTagModel("FilterWithTagModel_", "Moabs", true) {
            growTag = true, camoTag = true, moabTag = true, fortifiedTag = true
          },
        });
        attackModel.weapons[1].projectile.GetBehavior < ProjectileFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterWithTagModel("FilterWithTagModel_", "Moabs", true) {
            growTag = true, camoTag = true, moabTag = true, fortifiedTag = true
          },
        });
        attackModel.behaviors[1].Cast < AttackFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          //new FilterOutTagModel("FilterOutTagModel_ProjectileSqueeze","Moabs",new Il2CppStringArray(0)),
          new FilterInvisibleModel("FilterInvisibleModel_", true, false),
        });
        AbilityModel abilityModel = Game.instance.model.GetTowerFromId("PatFusty 20").GetAbility().Duplicate();
        abilityModel.icon = GetSpriteReference(mod, "Bloonchipper");
        ActivateTowerDamageSupportZoneModel a = abilityModel.GetBehavior < ActivateTowerDamageSupportZoneModel > ();
        ActivateRateSupportZoneModel arszm = new ActivateRateSupportZoneModel(a.name, a.mutatorId, a.isUnique, 0.5 f, 0.1 f, 1, a.canEffectThisTower, a.lifespan, null, a.buffLocsName, a.buffIconName, a.filters, false);
        abilityModel.AddBehavior < ActivateRateSupportZoneModel > (arszm);
        abilityModel.RemoveBehavior < ActivateTowerDamageSupportZoneModel > ();
        towerModel.AddBehavior(abilityModel);
        towerModel.AddBehavior(new OverrideCamoDetectionModel("OverrideCamoDetectionModel_", true));
        attackModel.weapons[0].Rate *= 0 f;
        attackModel.weapons[0].projectile.GetBehavior < DelayBloonChildrenSpawningModel > ().Lifespan *= 3 f;
      }
    }
    public class ZOMGSucc: ModUpgrade < BloonChipper > {
      public override string Name => "Attack All Bloons";
      public override string DisplayName => "ZOMG Killer";
      public override string Description => "Can now destroy Every Baloon! and instakill bloons.";
      public override int Cost => 0;
      public override int Path => MIDDLE;
      public override int Tier => 4;
      public override string Icon => "Bloonchipper";
      public override void ApplyUpgrade(TowerModel towerModel) {
        AttackModel attackModel = towerModel.GetBehavior < AttackModel > ();
        attackModel.weapons[1].projectile.filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterInvisibleModel("FilterInvisibleModel_", true, false),
            new FilterWithTagModel("FilterWithTagModel_", "Moabs", false) {
              growTag = true, camoTag = true, moabTag = true, fortifiedTag = true
            },
        });
        attackModel.weapons[1].projectile.GetBehavior < ProjectileFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          new FilterInvisibleModel("FilterInvisibleModel_", true, false),
            new FilterWithTagModel("FilterWithTagModel_", "Moabs", false) {
              growTag = true, camoTag = true, moabTag = true, fortifiedTag = true
            },
        });
        attackModel.behaviors[1].Cast < AttackFilterModel > ().filters = new Il2CppReferenceArray < FilterModel > (new FilterModel[] {
          //new FilterOutTagModel("FilterOutTagModel_ProjectileSqueeze","Moabs",new Il2CppStringArray(0)),
          new FilterInvisibleModel("FilterInvisibleModel_", true, false),
        });
        attackModel.weapons[0].projectile.GetBehavior < DamageModel > ().distributeToChildren = true;
        attackModel.weapons[0].projectile.GetBehavior < DamageModel > ().maxDamage = 999999;
        attackModel.weapons[0].projectile.GetBehavior < DamageModel > ().overrideDistributeBlocker = true;
      }
    }
    public class QuadSucc: ModUpgrade < BloonChipper > {
      public override string Name => "QuadSucc";
      public override string DisplayName => "Quad Killer";
      public override string Description => "Can now destroy 4 blimps at once, and attacks bloons much faster. Even Attacks Every Baloon At One Time!";
      public override int Cost => 0;
      public override int Path => MIDDLE;
      public override int Tier => 5;
      public override string Icon => "Bloonchipper";
      public override void ApplyUpgrade(TowerModel towerModel) {
        AttackModel attackModel = towerModel.GetBehavior < AttackModel > ();
        attackModel.weapons[1].projectile.pierce += 999999 f;
        attackModel.range += 5000;
        towerModel.range += 5000;
        attackModel.weapons[1].Rate *= 0.3 f;
        attackModel.weapons[1].projectile.GetBehavior < DelayBloonChildrenSpawningModel > ().Lifespan *= 1 f;
        attackModel.weapons[0].Rate *= 0.3 f;
        attackModel.weapons[0].projectile.GetBehavior < DelayBloonChildrenSpawningModel > ().Lifespan *= 1 f;
        attackModel.weapons[0].projectile.pierce = 999999 f;
        attackModel.weapons[0].projectile.radius += 999999;
      }
    }
  }
}
