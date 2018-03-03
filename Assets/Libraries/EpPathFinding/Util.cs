﻿namespace Assets.Libraries.EpPathFinding
{
    public class Util
    {
        public static DiagonalMovement GetDiagonalMovement(bool iCrossCorners, bool iCrossAdjacentPoint)
        {

            if (iCrossCorners && iCrossAdjacentPoint)
            {
                return DiagonalMovement.Always;
            }
            else if (iCrossCorners)
            {
                return DiagonalMovement.IfAtLeastOneWalkable;
            }
            else
            {
                return DiagonalMovement.OnlyWhenNoObstacles;
            }
        }
    }
}
