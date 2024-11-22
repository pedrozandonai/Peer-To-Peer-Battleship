﻿using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;

namespace PeerToPeerBattleship.Application.Games.Strategy.Strategies
{
    public class DataNotRecognizedStrategy : IGameStrategy
    {
        public Match ExecuteGameStrategy(string message, Match gameMatch)
        {
            throw new NotImplementedException();
        }
    }
}
