﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpandedContent.Utilities;
using ExpandedContent.Extensions;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.ElementsSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.RuleSystem;
using Kingmaker.Utility;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.RuleSystem.Rules.Damage;
using ExpandedContent.Config;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Abilities.Components.Base;

namespace ExpandedContent.Tweaks.Domains {
    internal class RestorationDomain {

        public static void AddRestorationDomain() {

            var ClericClass = Resources.GetBlueprint<BlueprintCharacterClass>("67819271767a9dd4fbfd4ae700befea0");
            var EcclesitheurgeArchetype = Resources.GetBlueprint<BlueprintArchetype>("472af8cb3de628f4a805dc4a038971bc");
            var InquisitorClass = Resources.GetBlueprint<BlueprintCharacterClass>("f1a70d9e1b0b41e49874e1fa9052a1ce");
            var HunterClass = Resources.GetBlueprint<BlueprintCharacterClass>("34ecd1b5e1b90b9498795791b0855239");
            var DivineHunterArchetype = Resources.GetBlueprint<BlueprintArchetype>("f996f0a18e5d945459e710ee3a6dd485");
            var PaladinClass = Resources.GetBlueprint<BlueprintCharacterClass>("bfa11238e7ae3544bbeb4d0b92e897ec");
            var TempleChampionArchetype = Resources.GetModBlueprint<BlueprintArchetype>("TempleChampionArchetype");
            var HealingDomainGreaterFeature = Resources.GetBlueprint<BlueprintFeature>("b9ea4eb16ded8b146868540e711f81c8");
            var RestorationLesserFX = Resources.GetBlueprint<BlueprintAbility>("e84fc922ccf952943b5240293669b171").GetComponent<AbilitySpawnFx>();
            var Stabilize = Resources.GetBlueprint<BlueprintAbility>("0557ccee0a86dc44cb3d3f6a3b235329");

            var RestorationDomainBaseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>("RestorationDomainBaseResource", bp => {
                bp.m_MaxAmount = new BlueprintAbilityResource.Amount {
                    BaseValue = 3,
                    IncreasedByLevel = false,
                    IncreasedByStat = true,
                    ResourceBonusStat = StatType.Wisdom,
                };
            });
            
            var RestorationDomainBaseAbility = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbility", bp => {
                bp.SetName("Restorative Touch");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });
            var RestorationDomainBaseAbilityDazed = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbilityDazed", bp => {
                bp.SetName("Restorative Touch - Dazed");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.SavingThrowType = SavingThrowType.Unknown;
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionDispelMagic() {
                            m_StopAfterCountRemoved = false,
                            m_CountToRemove = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 1,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_BuffType = ContextActionDispelMagic.BuffType.All,
                            m_MaxSpellLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_UseMaxCasterLevel = false,
                            m_MaxCasterLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_CheckType = RuleDispelMagic.CheckType.None,
                            m_Skill = StatType.Unknown,
                            CheckBonus = 0,
                            ContextBonus = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            Descriptor = SpellDescriptor.Daze,
                            OnSuccess = Helpers.CreateActionList(),
                            OnFail = Helpers.CreateActionList(),
                            OnlyTargetEnemyBuffs = false,
                            CheckSchoolOrDescriptor = false
                        }
                        );
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = RestorationLesserFX.PrefabLink;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.WeaponTarget = AbilitySpawnFxWeaponTarget.None;
                    c.DestroyOnCast = false;
                    c.Delay = 0;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });
            var RestorationDomainBaseAbilityFatigued = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbilityFatigued", bp => {
                bp.SetName("Restorative Touch - Fatigued");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.SavingThrowType = SavingThrowType.Unknown;
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionDispelMagic() {
                            m_StopAfterCountRemoved = false,
                            m_CountToRemove = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 1,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_BuffType = ContextActionDispelMagic.BuffType.All,
                            m_MaxSpellLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_UseMaxCasterLevel = false,
                            m_MaxCasterLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_CheckType = RuleDispelMagic.CheckType.None,
                            m_Skill = StatType.Unknown,
                            CheckBonus = 0,
                            ContextBonus = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            Descriptor = SpellDescriptor.Fatigue,
                            OnSuccess = Helpers.CreateActionList(),
                            OnFail = Helpers.CreateActionList(),
                            OnlyTargetEnemyBuffs = false,
                            CheckSchoolOrDescriptor = false
                        }
                        );
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = RestorationLesserFX.PrefabLink;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.WeaponTarget = AbilitySpawnFxWeaponTarget.None;
                    c.DestroyOnCast = false;
                    c.Delay = 0;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });
            var RestorationDomainBaseAbilityShaken = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbilityShaken", bp => {
                bp.SetName("Restorative Touch - Shaken");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.SavingThrowType = SavingThrowType.Unknown;
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionDispelMagic() {
                            m_StopAfterCountRemoved = false,
                            m_CountToRemove = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 1,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_BuffType = ContextActionDispelMagic.BuffType.All,
                            m_MaxSpellLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_UseMaxCasterLevel = false,
                            m_MaxCasterLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_CheckType = RuleDispelMagic.CheckType.None,
                            m_Skill = StatType.Unknown,
                            CheckBonus = 0,
                            ContextBonus = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            Descriptor = SpellDescriptor.Shaken,
                            OnSuccess = Helpers.CreateActionList(),
                            OnFail = Helpers.CreateActionList(),
                            OnlyTargetEnemyBuffs = false,
                            CheckSchoolOrDescriptor = false
                        }
                        );
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = RestorationLesserFX.PrefabLink;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.WeaponTarget = AbilitySpawnFxWeaponTarget.None;
                    c.DestroyOnCast = false;
                    c.Delay = 0;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });
            var RestorationDomainBaseAbilitySickened = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbilitySickened", bp => {
                bp.SetName("Restorative Touch - Sickened");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.SavingThrowType = SavingThrowType.Unknown;
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionDispelMagic() {
                            m_StopAfterCountRemoved = false,
                            m_CountToRemove = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 1,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_BuffType = ContextActionDispelMagic.BuffType.All,
                            m_MaxSpellLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_UseMaxCasterLevel = false,
                            m_MaxCasterLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_CheckType = RuleDispelMagic.CheckType.None,
                            m_Skill = StatType.Unknown,
                            CheckBonus = 0,
                            ContextBonus = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            Descriptor = SpellDescriptor.Sickened,
                            OnSuccess = Helpers.CreateActionList(),
                            OnFail = Helpers.CreateActionList(),
                            OnlyTargetEnemyBuffs = false,
                            CheckSchoolOrDescriptor = false
                        }
                        );
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = RestorationLesserFX.PrefabLink;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.WeaponTarget = AbilitySpawnFxWeaponTarget.None;
                    c.DestroyOnCast = false;
                    c.Delay = 0;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });
            var RestorationDomainBaseAbilityStaggered = Helpers.CreateBlueprint<BlueprintAbility>("RestorationDomainBaseAbilityStaggered", bp => {
                bp.SetName("Restorative Touch - Staggered");
                bp.SetDescription("You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch can remove the dazed, " +
                    "fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 + your Wisdom modifier.");
                bp.m_Icon = Stabilize.m_Icon;
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.SavingThrowType = SavingThrowType.Unknown;
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionDispelMagic() {
                            m_StopAfterCountRemoved = false,
                            m_CountToRemove = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 1,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_BuffType = ContextActionDispelMagic.BuffType.All,
                            m_MaxSpellLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_UseMaxCasterLevel = false,
                            m_MaxCasterLevel = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            m_CheckType = RuleDispelMagic.CheckType.None,
                            m_Skill = StatType.Unknown,
                            CheckBonus = 0,
                            ContextBonus = new ContextValue() {
                                ValueType = ContextValueType.Simple,
                                Value = 0,
                                ValueRank = AbilityRankType.Default,
                                ValueShared = AbilitySharedValue.Damage,
                                Property = UnitProperty.None
                            },
                            Descriptor = SpellDescriptor.Staggered,
                            OnSuccess = Helpers.CreateActionList(),
                            OnFail = Helpers.CreateActionList(),
                            OnlyTargetEnemyBuffs = false,
                            CheckSchoolOrDescriptor = false
                        }
                        );
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = RestorationLesserFX.PrefabLink;
                    c.Time = AbilitySpawnFxTime.OnApplyEffect;
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                    c.WeaponTarget = AbilitySpawnFxWeaponTarget.None;
                    c.DestroyOnCast = false;
                    c.Delay = 0;
                    c.PositionAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationAnchor = AbilitySpawnFxAnchor.None;
                    c.OrientationMode = AbilitySpawnFxOrientation.Copy;
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_AllowNonContextActions = false;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetPoint = false;
                bp.CanTargetEnemies = true;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = true;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.HasFastAnimation = false;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = 0;
                bp.LocalizedDuration = new Kingmaker.Localization.LocalizedString();
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();
            });

            RestorationDomainBaseAbility.AddComponent<AbilityVariants>(c => {
                c.m_Variants = new BlueprintAbilityReference[] {
                    RestorationDomainBaseAbilityDazed.ToReference<BlueprintAbilityReference>(),
                    RestorationDomainBaseAbilityFatigued.ToReference<BlueprintAbilityReference>(),
                    RestorationDomainBaseAbilityShaken.ToReference<BlueprintAbilityReference>(),
                    RestorationDomainBaseAbilitySickened.ToReference<BlueprintAbilityReference>(),
                    RestorationDomainBaseAbilityStaggered.ToReference<BlueprintAbilityReference>()
                };
            });
            //Spelllist
            var RemoveSicknessSpell = Resources.GetBlueprint<BlueprintAbility>("f6f95242abdfac346befd6f4f6222140");
            var RemoveDiseaseSpell = Resources.GetBlueprint<BlueprintAbility>("4093d5a0eb5cae94e909eb1e0e1a6b36");
            var CureSeriousWoundsCastSpell = Resources.GetBlueprint<BlueprintAbility>("3361c5df793b4c8448756146a88026ad");
            var NeutralizePoisonSpell = Resources.GetBlueprint<BlueprintAbility>("e7240516af4241b42b2cd819929ea9da");
            var BreakEnchantmentSpell = Resources.GetBlueprint<BlueprintAbility>("7792da00c85b9e042a0fdfc2b66ec9a8");
            var HealCastSpell = Resources.GetBlueprint<BlueprintAbility>("5da172c4c89f9eb4cbb614f3a67357d3");
            var RestorationGreaterSpell = Resources.GetBlueprint<BlueprintAbility>("fafd77c6bfa85c04ba31fdc1c962c914");
            var ProtectionFromSpellsSpell = Resources.GetBlueprint<BlueprintAbility>("42aa71adc7343714fa92e471baa98d42");
            var HealMassSpell = Resources.GetBlueprint<BlueprintAbility>("867524328b54f25488d371214eea0d90");
            var RestorationDomainSpellList = Helpers.CreateBlueprint<BlueprintSpellList>("RestorationDomainSpellList", bp => {
                bp.SpellsByLevel = new SpellLevelList[10] {
                    new SpellLevelList(0) {
                        SpellLevel = 0,
                        m_Spells = new List<BlueprintAbilityReference>()
                    },
                    new SpellLevelList(1) {
                        SpellLevel = 1,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            RemoveSicknessSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(2) {
                        SpellLevel = 2,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            RemoveDiseaseSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(3) {
                        SpellLevel = 3,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            CureSeriousWoundsCastSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(4) {
                        SpellLevel = 4,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            NeutralizePoisonSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(5) {
                        SpellLevel = 5,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            BreakEnchantmentSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(6) {
                        SpellLevel = 6,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            HealCastSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(7) {
                        SpellLevel = 7,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            RestorationGreaterSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(8) {
                        SpellLevel = 8,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            ProtectionFromSpellsSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                    new SpellLevelList(9) {
                        SpellLevel = 9,
                        m_Spells = new List<BlueprintAbilityReference>() {
                            HealMassSpell.ToReference<BlueprintAbilityReference>()
                        }
                    },
                };
            });     
            var RestorationDomainSpellListFeature = Helpers.CreateBlueprint<BlueprintFeature>("RestorationDomainSpellListFeature", bp => {
                bp.AddComponent<AddSpecialSpellList>(c => {
                    c.m_CharacterClass = ClericClass.ToReference<BlueprintCharacterClassReference>();
                    c.m_SpellList = RestorationDomainSpellList.ToReference<BlueprintSpellListReference>();
                });
                bp.m_AllowNonContextActions = false;
                bp.HideInUI = true;
                bp.IsClassFeature = true;
            });            
            var RestorationDomainBaseFeature = Helpers.CreateBlueprint<BlueprintFeature>("RestorationDomainBaseFeature", bp => {
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { RestorationDomainBaseAbility.ToReference<BlueprintUnitFactReference>() };
                });
                bp.AddComponent<AddAbilityResources>(c => { 
                    c.m_Resource = RestorationDomainBaseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.RestoreAmount = true;
                });
                bp.AddComponent<ReplaceAbilitiesStat>(c => {
                    c.m_Ability = new BlueprintAbilityReference[] { RestorationDomainBaseAbility.ToReference<BlueprintAbilityReference>() };
                    c.Stat = StatType.Wisdom;
                });
                bp.AddComponent<AddFeatureOnClassLevel>(c => {
                    c.m_Class = ClericClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 1;
                    c.m_Feature = RestorationDomainSpellListFeature.ToReference<BlueprintFeatureReference>();
                });
                bp.m_AllowNonContextActions = false;
                bp.SetName("Restoration Subdomain");
                bp.SetDescription("\nYour {g|Encyclopedia:TouchAttack}touch{/g} restores vigor, protecting against ailments, and your {g|Encyclopedia:Healing}healing{/g} magic is particularly " +
                    "vital and potent.\nRestorative Touch: You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch " +
                    "can remove the dazed, fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 " +
                    "+ your Wisdom modifier.\nHealer's Blessing: At 6th level, all of your cure {g|Encyclopedia:Spell}spells{/g} are treated as if they were empowered, increasing the amount of damage " +
                    "healed by half (+50%). This does not apply to damage dealt to undead with a cure spell. This does not stack with the Empower Spell metamagic {g|Encyclopedia:Feat}feat{/g}.");
                bp.IsClassFeature = true;
            });
            //Deity plug
            var RestorationDomainAllowed = Helpers.CreateBlueprint<BlueprintFeature>("RestorationDomainAllowed", bp => {
                // This may buff Ecclest when it shouldn't, needs test
                //bp.AddComponent<AddSpecialSpellListForArchetype>(c => {
                //    c.m_CharacterClass = ClericClass.ToReference<BlueprintCharacterClassReference>();
                //    c.m_SpellList = RestorationDomainSpellList.ToReference<BlueprintSpellListReference>();
                //    c.m_Archetype = EcclesitheurgeArchetype.ToReference<BlueprintArchetypeReference>();
                //});
                bp.m_AllowNonContextActions = false;
                bp.HideInUI = true;
                bp.IsClassFeature = true;                
            });            
            // Main Blueprint
            var RestorationDomainProgression = Helpers.CreateBlueprint<BlueprintProgression>("RestorationDomainProgression", bp => {
                bp.AddComponent<PrerequisiteFeature>(c => {
                    c.Group = Prerequisite.GroupType.All;
                    c.HideInUI = true;
                    c.m_Feature = RestorationDomainAllowed.ToReference<BlueprintFeatureReference>();
                });
                bp.AddComponent<LearnSpellList>(c => {
                    c.m_CharacterClass = ClericClass.ToReference<BlueprintCharacterClassReference>();
                    c.m_SpellList = RestorationDomainSpellList.ToReference<BlueprintSpellListReference>();
                    c.m_Archetype = EcclesitheurgeArchetype.ToReference<BlueprintArchetypeReference>();
                });
                bp.AddComponent<LearnSpellList>(c => {
                    c.m_CharacterClass = HunterClass.ToReference<BlueprintCharacterClassReference>();
                    c.m_SpellList = RestorationDomainSpellList.ToReference<BlueprintSpellListReference>();
                    c.m_Archetype = DivineHunterArchetype.ToReference<BlueprintArchetypeReference>();
                });
                bp.m_AllowNonContextActions = false;
                bp.SetName("Restoration Subdomain");
                bp.SetDescription("\nYour {g|Encyclopedia:TouchAttack}touch{/g} restores vigor, protecting against ailments, and your {g|Encyclopedia:Healing}healing{/g} magic is particularly " +
                    "vital and potent.\nRestorative Touch: You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch " +
                    "can remove the dazed, fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 " +
                    "+ your Wisdom modifier.\nHealer's Blessing: At 6th level, all of your cure {g|Encyclopedia:Spell}spells{/g} are treated as if they were empowered, increasing the amount of damage " +
                    "healed by half (+50%). This does not apply to damage dealt to undead with a cure spell. This does not stack with the Empower Spell metamagic {g|Encyclopedia:Feat}feat{/g}.\nDomain " +
                    "Spells: remove sickness, remove disease, cure serious wounds, neutralize poison, break enchantment, heal, greater restoration, protection from spells, mass heal.");
                bp.Groups = new FeatureGroup[] { FeatureGroup.Domain };
                bp.IsClassFeature = true;
                bp.m_Classes = new BlueprintProgression.ClassWithLevel[] {
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = ClericClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = InquisitorClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = HunterClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = PaladinClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                };
                bp.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[] {
                    new BlueprintProgression.ArchetypeWithLevel {
                        m_Archetype = DivineHunterArchetype.ToReference<BlueprintArchetypeReference>(),
                        AdditionalLevel = -2
                    },
                    new BlueprintProgression.ArchetypeWithLevel {
                        m_Archetype = TempleChampionArchetype.ToReference<BlueprintArchetypeReference>(),
                        AdditionalLevel = 0
                    }
                };                
                bp.LevelEntries = new LevelEntry[] {
                    Helpers.LevelEntry(1, RestorationDomainBaseFeature),
                    Helpers.LevelEntry(6, HealingDomainGreaterFeature)
                };
                bp.UIGroups = new UIGroup[] {
                    Helpers.CreateUIGroup(RestorationDomainBaseFeature, HealingDomainGreaterFeature)
                };
                bp.GiveFeaturesForPreviousLevels = true;
            });
            // Secondary Domain Progression
            var RestorationDomainProgressionSecondary = Helpers.CreateBlueprint<BlueprintProgression>("RestorationDomainProgressionSecondary", bp => {
                bp.AddComponent<PrerequisiteFeature>(c => {
                    c.Group = Prerequisite.GroupType.All;
                    c.HideInUI = true;
                    c.m_Feature = RestorationDomainAllowed.ToReference<BlueprintFeatureReference>();
                });
                bp.AddComponent<PrerequisiteNoFeature>(c => {
                    c.Group = Prerequisite.GroupType.All;
                    c.HideInUI = true;
                    c.m_Feature = RestorationDomainProgression.ToReference<BlueprintFeatureReference>();
                });                
                bp.m_AllowNonContextActions = false;
                bp.SetName("Restoration Subdomain");
                bp.SetDescription("\nYour {g|Encyclopedia:TouchAttack}touch{/g} restores vigor, protecting against ailments, and your {g|Encyclopedia:Healing}healing{/g} magic is particularly " +
                    "vital and potent.\nRestorative Touch: You can touch a creature, letting the healing power of your deity flow through you to relieve the creature of a minor condition. Your touch " +
                    "can remove the dazed, fatigued, shaken, sickened, or staggered condition. You choose which condition is removed. You can use this ability a number of times per day equal to 3 " +
                    "+ your Wisdom modifier.\nHealer's Blessing: At 6th level, all of your cure {g|Encyclopedia:Spell}spells{/g} are treated as if they were empowered, increasing the amount of damage " +
                    "healed by half (+50%). This does not apply to damage dealt to undead with a cure spell. This does not stack with the Empower Spell metamagic {g|Encyclopedia:Feat}feat{/g}.\nDomain " +
                    "Spells: remove sickness, remove disease, cure serious wounds, neutralize poison, break enchantment, heal, greater restoration, protection from spells, mass heal.");
                bp.Groups = new FeatureGroup[] { FeatureGroup.ClericSecondaryDomain };
                bp.IsClassFeature = true;
                bp.m_Classes = new BlueprintProgression.ClassWithLevel[] {
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = ClericClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                    new BlueprintProgression.ClassWithLevel {
                        m_Class = PaladinClass.ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    },
                };
                bp.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[] {
                    new BlueprintProgression.ArchetypeWithLevel {
                        m_Archetype = TempleChampionArchetype.ToReference<BlueprintArchetypeReference>(),
                        AdditionalLevel = 0
                    }
                };
                bp.LevelEntries = new LevelEntry[] {
                    Helpers.LevelEntry(1, RestorationDomainBaseFeature),
                    Helpers.LevelEntry(6, HealingDomainGreaterFeature)
                };
                bp.UIGroups = new UIGroup[] {
                    Helpers.CreateUIGroup(RestorationDomainBaseFeature, HealingDomainGreaterFeature)
                };
                bp.GiveFeaturesForPreviousLevels = true;
            });            
            RestorationDomainAllowed.IsPrerequisiteFor = new List<BlueprintFeatureReference>() { 
                RestorationDomainProgression.ToReference<BlueprintFeatureReference>(),
                RestorationDomainProgressionSecondary.ToReference<BlueprintFeatureReference>()
            };
            RestorationDomainProgression.AddComponent<PrerequisiteNoFeature>(c => {
                    c.Group = Prerequisite.GroupType.All;
                    c.HideInUI = true;
                    c.m_Feature = RestorationDomainProgressionSecondary.ToReference<BlueprintFeatureReference>();
            }); 
            RestorationDomainProgressionSecondary.AddComponent<PrerequisiteNoFeature>(c => {
                    c.Group = Prerequisite.GroupType.All;
                    c.HideInUI = true;
                    c.m_Feature = RestorationDomainProgressionSecondary.ToReference<BlueprintFeatureReference>();
            });
            var DomainMastery = Resources.GetBlueprint<BlueprintFeature>("2de64f6a1f2baee4f9b7e52e3f046ec5").GetComponent<AutoMetamagic>();
            DomainMastery.Abilities.Add(RestorationDomainBaseAbility.ToReference<BlueprintAbilityReference>());
            if (ModSettings.AddedContent.Domains.IsDisabled("Restoration Subdomain")) { return; }
            DomainTools.RegisterDomain(RestorationDomainProgression);
            DomainTools.RegisterSecondaryDomain(RestorationDomainProgressionSecondary);
            DomainTools.RegisterDivineHunterDomain(RestorationDomainProgression);
            DomainTools.RegisterTempleDomain(RestorationDomainProgression);
            DomainTools.RegisterSecondaryTempleDomain(RestorationDomainProgressionSecondary);
            DomainTools.RegisterImpossibleSubdomain(RestorationDomainProgression, RestorationDomainProgressionSecondary);
        }
    }
}
