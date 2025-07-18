﻿using System;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using DeviceType = WelwiseGamesSDK.Shared.Types.DeviceType;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class UnityEnvironment : IEnvironment
    {
        public bool IsAvailable { get; }
        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string Language { get; }

        public UnityEnvironment(SDKSettings settings, bool isAvailable)
        {
            IsAvailable = isAvailable;
            if (string.IsNullOrEmpty(settings.DebugPlayerId) || !Guid.TryParse(settings.DebugPlayerId, out var id))
            {
                PlayerId = Guid.NewGuid();
            }
            else
            {
                PlayerId = id;
            }
            
            DeviceType = settings.DebugDeviceType;
            Language = settings.DebugLanguageCode;
        }
        
        public void RequestServerTime(Action<long> callback)
        {
            callback?.Invoke(DateTime.Now.Ticks);
        }
    }
}