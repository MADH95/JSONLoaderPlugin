﻿using DiskCardGame;
using System.Collections;
using UnityEngine;

namespace JLPlugin.Data
{
    [System.Serializable]
    public class moveCards
    {
        public string runOnCondition;
        public slotData moveFromSlot;
        public slotData moveToSlot;
        public string replace;
        public string self;
        public strafeData strafe;

        public bool movingLeft;

        public static IEnumerator MoveCards(AbilityBehaviourData abilitydata)
        {
            yield return new WaitForSeconds(0.3f);
            if (Singleton<ViewManager>.Instance.CurrentView != View.Board)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.3f);
            }

            foreach (moveCards movecardinfo in abilitydata.moveCards)
            {
                if (SigilData.ConvertArgument(movecardinfo.runOnCondition, abilitydata) == "false")
                {
                    continue;
                }

                Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                CardSlot slotFrom = slotData.GetSlot(movecardinfo.moveFromSlot, abilitydata);
                CardSlot slotTo = slotData.GetSlot(movecardinfo.moveToSlot, abilitydata);
                if (slotFrom?.Card != null)
                {
                    if (slotTo != null)
                    {
                        if (slotTo.Card != null && (SigilData.ConvertArgument(movecardinfo.replace, abilitydata) ?? "true") == "true")
                        {
                            slotTo.Card.ExitBoard(0, new Vector3(0, 0, 0));
                        }

                        if (slotTo.Card == null)
                        {
                            PlayableCard cardToSet = slotFrom.Card;
                            cardToSet.SetIsOpponentCard(!slotTo.IsPlayerSlot);
                            yield return Singleton<BoardManager>.Instance.AssignCardToSlot(slotFrom.Card, slotTo);
                        }

                    }
                    if (movecardinfo.strafe != null)
                    {
                        movecardinfo.strafe.Strafe(abilitydata, movecardinfo, slotFrom);
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
            yield break;
        }
    }
}