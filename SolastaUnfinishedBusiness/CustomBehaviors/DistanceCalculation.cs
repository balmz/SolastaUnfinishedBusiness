﻿using System;
using TA;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class DistanceCalculation
{
    internal static int CalculateDistanceFromTwoCharacters(GameLocationCharacter character1, GameLocationCharacter character2)
    {
        var character1ClosestCube = GetCharacterClosestCubeToPosition(character1, GetPositionCenter(character2));
        var character2ClosestCube = GetCharacterClosestCubeToPosition(character2, character1ClosestCube);

        var distance = GetDistanceFromTwoPositions(character1ClosestCube, character2ClosestCube);
        
        return distance;
    }

    private static int3 GetCharacterClosestCubeToPosition(GameLocationCharacter character1, int3 position)
    {
        var closestCharacter1Position = character1.LocationBattleBoundingBox.Min;
        var closestDistance = (closestCharacter1Position - position).magnitude;

        var character1NumberOfBoxes = (character1.LocationBattleBoundingBox.Size.x) *
                                      (character1.LocationBattleBoundingBox.Size.y) *
                                      (character1.LocationBattleBoundingBox.Size.z);

        return character1NumberOfBoxes is 1 ? closestCharacter1Position : GetBigCharacterClosestCubePosition(character1, position, closestDistance, closestCharacter1Position);
    }

    private static int3 GetBigCharacterClosestCubePosition(GameLocationCharacter character1, int3 position, float closestDistance,
        int3 closestCharacter1Position)
    {
        var minX = character1.LocationBattleBoundingBox.Min.x;
        var minY = character1.LocationBattleBoundingBox.Min.y;
        var minZ = character1.LocationBattleBoundingBox.Min.z;
        for (int x = minX; x < minX + character1.LocationBattleBoundingBox.Size.x; x++)
        {
            for (int y = minY; y < minY + character1.LocationBattleBoundingBox.Size.y; y++)
            {
                for (int z = minZ; z < minZ + character1.LocationBattleBoundingBox.Size.z; z++)
                {
                    var currentCubePosition = new int3(x, y, z);
                    if ((currentCubePosition - position).magnitude < closestDistance)
                    {
                        closestCharacter1Position = currentCubePosition;
                        closestDistance = (currentCubePosition - position).magnitude;
                    }
                }
            }
        }

        return closestCharacter1Position;
    }

    private static int3 GetPositionCenter(GameLocationCharacter gameLocationCharacter)
        => new((int)gameLocationCharacter.LocationBattleBoundingBox.Center.x, 
            (int)gameLocationCharacter.LocationBattleBoundingBox.Center.y, 
            (int)gameLocationCharacter.LocationBattleBoundingBox.Center.z);

    private static int GetDistanceFromTwoPositions(int3 position1, int3 position2)
    {
        var rawDistance = position1 - position2;
        var distance = Math.Max(Math.Max(Math.Abs(rawDistance.x), Math.Abs(rawDistance.z)), Math.Abs(rawDistance.y));

        return distance;
    }
}
