using SickDev.CommandSystem;
using System;

namespace MonsterTrainConsole
{
    public static class Hand
    {
        [Command(
            description = "Discard a card in hand. @index: The index of card in hard. 1 is the leftmost card, and 10 is the rightmost card if your hand is full.",
            useClassName = true
        )]
        public static void Discard(int index)
        {
            CheatCommon.AssertInBattle();

            if (index < 1 || index > 10)
                throw new ArgumentException($"@index must be from 1 to 10.");

            var saveManager = SingletonAccessor.SaveManager;
            saveManager.StartCoroutine(saveManager.Cheat_Discard(index - 1));
        }

        [Command(
            description = "Discard ALL cards in hand.",
            useClassName = true
        )]
        public static void DiscardAll()
        {
            CheatCommon.AssertInBattle();

            var saveManager = SingletonAccessor.SaveManager;
            saveManager.StartCoroutine(saveManager.Cheat_DiscardAll());
        }

        [Command(
            description = "Draw cards. @count: number of cards to draw.",
            useClassName = true
        )]
        public static void Draw(int count)
        {
            CheatCommon.AssertInBattle();

            if (count < 1)
                throw new ArgumentException($"@count must be positive.");

            SingletonAccessor.CardManager.DrawCards(count);
        }
    }
}
