﻿using HarmonyLib;
using ExpandedContent.Config;
using ExpandedContent.Extensions;
using ExpandedContent.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpandedContent.Tweaks.Deities {

    internal class GreenFaith {

        
                
        public static void AddGreenFaith() {

            var GreenFaithIcon = AssetLoader.LoadInternal("Deities", "Icon_GreenFaith.jpg");
            var GreenFaithFeature = Resources.GetBlueprint<BlueprintFeature>("99a7a8f13c1300c42878558fa9471e2f");
            var WarpriestClass = Resources.GetBlueprint<BlueprintCharacterClass>("30b5e47d47a0e37438cc5a80c96cfb99");
            var MantisZealotArchetype = Resources.GetModBlueprint<BlueprintArchetype>("MantisZealotArchetype");
            var DreadKnightClass = Resources.GetModBlueprint<BlueprintCharacterClass>("DreadKnightClass");
            var ClawOfTheFalseWyrmArchetype = Resources.GetModBlueprint<BlueprintArchetype>("ClawOfTheFalseWyrmArchetype");

            GreenFaithFeature.AddComponent<PrerequisiteNoArchetype>(c => {
                c.HideInUI = true;
                c.m_CharacterClass = WarpriestClass.ToReference<BlueprintCharacterClassReference>();
                c.m_Archetype = MantisZealotArchetype.ToReference<BlueprintArchetypeReference>();
            });
            GreenFaithFeature.AddComponent<PrerequisiteNoArchetype>(c => {
                c.HideInUI = true;
                c.m_CharacterClass = DreadKnightClass.ToReference<BlueprintCharacterClassReference>();
                c.m_Archetype = ClawOfTheFalseWyrmArchetype.ToReference<BlueprintArchetypeReference>();
            });
            GreenFaithFeature.SetDescription("\nType: Druidic  " +
                        "\nLeader: \bThe Archdruid   " +
                        "\nDomains: Air, Earth, Animal, Fire, Plant. " +
                        "\nFavoured Weapons: Sickle, Quarterstaff" +
                        "\nThe Green Faith (also known as the Old Faith, the Great Eld, or the Wyrd) is a naturalistic philosophy based on " +
                        "the belief that natural forces are worthy of attention and respect. Followers of the Green Faith meditate daily, commune " +
                        "with natural forms of power, and show respect to nature in all things. They often hang fresh herbs from the lintels of " +
                        "doorways as a sign of respect for nature. Although the Green Faith is based on nature, one need not be a druid to value its " +
                        "tenets, nor do all druids necessarily count themselves as members of this philosophy, although almost all of " +
                        "them appreciate its values. On Golarion, the Green Faith is centred in Andoran, Nirmathas, Taldor, and the River Kingdoms. " +
                        "Sarkoris, prior to its destruction at the hands of demons, was formerly the Green Faith's greatest bastion; since then, " +
                        "Green Faith holdouts continue their fight to protect nature from the corruption of the Worldwound. Humans, elves, and " +
                        "half-elves make up the majority of the Green Faith's members. The Green Faith is one of the oldest forms of worship in Golarion, " +
                        "as evidenced by the numerous druidic symbols found in the cave drawings of early man. According to legend, in the earliest years " +
                        "of its existence, the Green Faith was split into four factions. One group venerated the strength and endurance of stone, " +
                        "another the power and ferocity of wild beasts, a third the bountiful earth, and the last, the cleansing purity of fire. For years, " +
                        "these four factions fought for supremacy, each claiming that the aspect of nature it venerated was the most important. Finally, the " +
                        "leaders of the four factions agreed to resolve their conflict through single combat. Before the battle could begin, however, a " +
                        "multicolored geyser sprang from the ground, equal parts water, earth, and flame. The geyser was followed by an enormous flock of " +
                        "multicolored birds, which flew off in all directions. The faction leaders saw this as a clear sign from nature that no one element " +
                        "could be more important than another, and that ultimately, the four factions shared the same basic philosophy.");
                    GreenFaithFeature.m_Icon = GreenFaithIcon;
                    GreenFaithFeature.RemoveComponents<PrerequisiteNoFeature>();
                
        }
                
            
    }
        
}
    


    


