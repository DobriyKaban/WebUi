using System;
using CerberusWareV3.Configuration;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

public class AntiAfkSystem : EntitySystem
{
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    private TimeSpan _nextClick;

    public override void Initialize()
    {
        base.Initialize();
        _nextClick = TimeSpan.Zero;
    }

    public override void Update(float frameTime)
    {
        if (!CerberusConfig.Misc.AntiAfkEnabled)
            return;

        if (_playerManager.LocalEntity == null)
            return;

        if (_timing.CurTime < _nextClick)
            return;

        var uid = _playerManager.LocalEntity.Value;
        var keyFunctionId = _inputManager.NetworkBindMap.KeyFunctionID(EngineKeyFunctions.UIClick);
        _nextClick = _timing.CurTime + TimeSpan.FromSeconds(5);
        
        var msg = new FullInputCmdMessage(
            _timing.CurTick,
            _timing.TickFraction,
            keyFunctionId,
            BoundKeyState.Down,
            GetNetCoordinates(_transformSystem.GetMoverCoordinates(uid)),
            _inputManager.MouseScreenPosition,
            GetNetEntity(uid)
        );

        RaisePredictiveEvent(msg);
    }
}