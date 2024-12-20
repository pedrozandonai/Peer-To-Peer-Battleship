﻿using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using Serilog;
using System.Net;

namespace PeerToPeerBattleship.Core.Inputs
{
    public class UserInputHandler : IUserInputHandler
    {
        private readonly ILogger _logger;

        public UserInputHandler(IContextualLogger<UserInputHandler> contextualLogger)
        {
            _logger = contextualLogger.Logger;
        }

        public int ReadInt(string inputMessage)
        {
            int intInput;
            Console.Write(inputMessage);
            try
            {
                intInput = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception ex)
            {
                _logger.Error("Tipo de entrada inválida, tente novamente. {0}", ex.Message);
                intInput = ReadInt(inputMessage);
            }

            return intInput;
        }

        public short ReadShort(string inputMessage)
        {
            short shortInput;
            Console.Write(inputMessage);
            try
            {
                shortInput = Convert.ToInt16(Console.ReadLine());
            }
            catch (Exception ex)
            {
                _logger.Error("Tipo de entrada inválida, tente novamente. {0}", ex.Message);
                shortInput = ReadShort(inputMessage);
            }

            return shortInput;
        }

        public string ReadIpAddress(string inputMessage)
        {
            string? ipAddressInput;
            Console.Write(inputMessage);
            try
            {
                ipAddressInput = Console.ReadLine();

                if (string.IsNullOrEmpty(ipAddressInput))
                {
                    _logger.Error("Endereço IP nulo ou branco, por favor, digite um endereço de IP válido.");
                    ipAddressInput = ReadIpAddress(inputMessage);
                }

                if (IPAddress.TryParse(ipAddressInput, out _))
                    return ipAddressInput;
                else
                {
                    _logger.Error("Endereço de IP formatado errado, por favor, digite um endereço de IP válido.");
                    ipAddressInput = ReadIpAddress(inputMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Tipo de entrada inválida, tente novamente. {0}", ex.Message);
                ipAddressInput = ReadIpAddress(inputMessage);
            }

            return ipAddressInput!;
        }

        public (int X, int Y) ReadPositions(string inputMessage)
        {
            string? positionInput;
            Console.Write(inputMessage);

            try
            {
                positionInput = Console.ReadLine();

                if (string.IsNullOrEmpty(positionInput))
                {
                    _logger.Error("Entrada inválida. A posição não pode ficar em branco.");
                    return ReadPositions(inputMessage);
                }

                var parts = positionInput.Split(',');

                if (parts.Length != 2 ||
                    !int.TryParse(parts[0], out var x) ||
                    !int.TryParse(parts[1], out var y))
                {
                    _logger.Error("Entrada inválida. Use o formato X,Y (exemplo: 5,5).");
                    return ReadPositions(inputMessage);
                }

                if (x > 9 ||
                    y > 9)
                {
                    _logger.Error("Entrada inválida. A posição não pode ultrapassar o tamanho da grade (9).");
                    return ReadPositions(inputMessage);
                }

                return (x, y);
            }
            catch (Exception ex)
            {
                _logger.Error("Tipo de entrada inválida, tente novamente. {0}", ex.Message);
                return ReadPositions(inputMessage);
            }
        }
    }
}
