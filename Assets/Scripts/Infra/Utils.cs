using System.Collections.Generic;
using UnityEngine;

namespace Infra
{
    /// <summary>
    /// A static class for utils function used in many places.
    /// </summary>
    public static class Utils 
    {
        /// <summary>
        /// Loop over the board symbols view list to find a symbol view with the given position.
        /// </summary>
        /// <param name="boardSymbolsView">the board symbols view list</param>
        /// <param name="position">the symbol view position to be founded</param>
        /// <returns>Symbol view with the given position</returns>
        public static ISymbolView GetSymbolViewByPosition(List<ISymbolView> boardSymbolsView,Position? position)
        {
            if (position == null || boardSymbolsView == null)
            {
                return null;
            }
            return boardSymbolsView.Find(square => square.Position.IsEqual(position.Value));
        }

        /// <summary>
        /// the function receive a list of position and return a random available position from the list.
        /// </summary>
        /// <param name="availablePosition">available position list</param>
        /// <returns>available position</returns>
        public static Position? GetRandomPosition(List<Position> availablePosition)
        {
            if (availablePosition == null || availablePosition.Count <= 0)
            {
                Debug.LogError("Can't get random position");
                return null;
            }
            int randomIndex = Random.Range(0, availablePosition.Count);
            return availablePosition[randomIndex];
        }
    }
}

