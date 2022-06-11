﻿using DiskCardGame;
using JLPlugin.V2.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JLPlugin.Data
{
    [System.Serializable]
    public class buffCards
    {
        public string runOnCondition;
        public slotData slot;
        public string addStats;
        public string setStats;
        public string heal;
        public List<string> addAbilities;
        public List<string> removeAbilities;
        public string self;

        public static IEnumerator BuffCards(AbilityBehaviourData abilitydata)
        {
            yield return new WaitForSeconds(0.3f);
            if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.3f);
            }

            foreach (buffCards buffcardsinfo in abilitydata.buffCards)
            {
                if (SigilData.ConvertArgument(buffcardsinfo.runOnCondition, abilitydata) == "false")
                {
                    continue;
                }

                PlayableCard card = null;
                if (buffcardsinfo.self != null)
                {
                    card = abilitydata.self;
                }
                else
                {
                    CardSlot slot = slotData.GetSlot(buffcardsinfo.slot, abilitydata);
                    if (slot != null)
                    {
                        if (slot.Card != null)
                        {
                            card = slot.Card;
                        }
                    }
                }

                if (card != null)
                {
                    CardModificationInfo mod = new CardModificationInfo();
                    if (buffcardsinfo.heal != null)
                    {
                        if (card.Status.damageTaken > 0)
                        {
                            card.HealDamage(Math.Min(card.Status.damageTaken, int.Parse(SigilData.ConvertArgument(buffcardsinfo.heal, abilitydata))));
                        }
                    }
                    if (buffcardsinfo.addStats != null)
                    {
                        mod.attackAdjustment += int.Parse(SigilData.ConvertArgument(buffcardsinfo.addStats.Split('/')[0], abilitydata));
                        mod.healthAdjustment += int.Parse(SigilData.ConvertArgument(buffcardsinfo.addStats.Split('/')[1], abilitydata));
                    }
                    if (buffcardsinfo.setStats != null)
                    {
                        mod.attackAdjustment += int.Parse(SigilData.ConvertArgument(buffcardsinfo.setStats.Split('/')[0], abilitydata)) - card.Info.Attack;
                        mod.healthAdjustment += int.Parse(SigilData.ConvertArgument(buffcardsinfo.setStats.Split('/')[1], abilitydata)) - card.Info.Health;
                    }
                    if (buffcardsinfo.removeAbilities != null)
                    {
                        yield return new WaitForSeconds(0.15f);
                        card.Anim.PlayTransformAnimation();
                        yield return new WaitForSeconds(0.15f);
                        mod.negateAbilities = SigilData.ConvertArgument(buffcardsinfo.removeAbilities, abilitydata).Select(s => CardSerializeInfo.ParseEnum<Ability>(s)).ToList();
                        card.Status.hiddenAbilities.AddRange(SigilData.ConvertArgument(buffcardsinfo.removeAbilities, abilitydata).Select(s => CardSerializeInfo.ParseEnum<Ability>(s)));
                    }
                    if (buffcardsinfo.addAbilities != null)
                    {
                        yield return new WaitForSeconds(0.15f);
                        card.Anim.PlayTransformAnimation();
                        yield return new WaitForSeconds(0.15f);
                        mod.abilities = SigilData.ConvertArgument(buffcardsinfo.addAbilities, abilitydata).Select(s => CardSerializeInfo.ParseEnum<Ability>(s)).ToList();
                    }
                    card.AddTemporaryMod(mod);
                    card.RenderCard();
                    if (card.Health <= 0)
                    {
                        yield return card.Die(false);
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
            yield break;
        }
    }
}