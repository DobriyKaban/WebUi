using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hexa.NET.ImGui;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Robust.Shared.Maths.Color;


[CompilerGenerated]
public sealed class EntityPreviewSystem : EntitySystem
{
	public override void Initialize()
	{
		base.Initialize();
		this._spriteControl = new EntityPreviewSystem.ContentSpriteControl(this);
		this._uiManager.RootControl.AddChild(this._spriteControl);
	}
	public override void Shutdown()
	{
		base.Shutdown();
		this._uiManager.RootControl.RemoveChild(this._spriteControl);
	}
	public async Task RenderSpriteAsync(EntityUid entity, Direction direction = Direction.South, CancellationToken cancelToken = default)
	{
		if (this._timing.IsFirstTimePredicted)
		{
			return;
		}

		SpriteComponent spriteComp;
		if (!base.TryComp(entity, out spriteComp))
		{
			return;
		}

		Vector2i size = Vector2i.Zero;

		foreach (ISpriteLayer layer in spriteComp.AllLayers)
		{
			if (layer.Visible)
			{
				size = Vector2i.ComponentMax(size, layer.PixelSize);
			}
		}

		if (size == Vector2i.Zero)
		{
			return;
		}

		IRenderTexture renderTarget = this._clyde.CreateRenderTarget(
			size, 
			new RenderTargetFormatParameters(RenderTargetColorFormat.Rgba8Srgb, false), 
			null, 
			"player_preview");

		var tcs = new TaskCompletionSource<bool>();
		
		using (cancelToken.Register(() => tcs.TrySetCanceled()))
		{
			this._spriteControl.QueueRender((renderTarget, entity, direction, tcs));
        
			try 
			{
				await tcs.Task.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				renderTarget.Dispose();
			}
		}
	}
	public ImTextureID GetImGuiTexture(EntityUid entity)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
		defaultInterpolatedStringHandler.AppendLiteral("player_");
		defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		return TextureLoader.GetImage(text);
	}
	private ImTextureID ConvertToImGuiTexture(IRenderTexture rt, string key)
	{
		rt.CopyPixelsToMemory<Rgba32>(delegate(Image<Rgba32> image)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				image.SaveAsPng(memoryStream);
				TextureLoader.AddImage(key, memoryStream.ToArray(), TextureLoader.ImageFilterMode.Nearest);
			}
		}, null);
		return TextureLoader.GetImage(key);
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IClyde _clyde = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IUserInterfaceManager _uiManager = null;
	private EntityPreviewSystem.ContentSpriteControl _spriteControl = null;
	
	private sealed class ContentSpriteControl : Control
	{
		public ContentSpriteControl(EntityPreviewSystem system)
		{
			IoCManager.InjectDependencies<EntityPreviewSystem.ContentSpriteControl>(this);
			this._system = system;
		}
		public void QueueRender(ValueTuple<IRenderTexture, EntityUid, Direction, TaskCompletionSource<bool>> job)
		{
			this._queue.Enqueue(job);
		}
		protected override void Draw(DrawingHandleScreen handle)
		{
			base.Draw(handle);
			for (;;)
			{
				ValueTuple<IRenderTexture, EntityUid, Direction, TaskCompletionSource<bool>> job;
				bool flag = this._queue.TryDequeue(out job);
				if (!flag)
				{
					break;
				}
				handle.RenderInRenderTarget(job.Item1, delegate
				{
					DrawingHandleScreen handle2 = handle;
					EntityUid item = job.Item2;
					Vector2 vector = job.Item1.Size / 2;
					Vector2 one = Vector2.One;
					Angle? angle = new Angle?(Angle.Zero);
					Direction? direction = new Direction?(job.Item3);
					handle2.DrawEntity(item, vector, one, angle, default(Angle), direction, null, null, null);
				}, new Color?(Color.Transparent));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("player_");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(job.Item2);
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				this._system.ConvertToImGuiTexture(job.Item1, text);
				job.Item1.Dispose();
				job.Item4.SetResult(true);
			}
		}
		private readonly EntityPreviewSystem _system;
		private readonly Queue<ValueTuple<IRenderTexture, EntityUid, Direction, TaskCompletionSource<bool>>> _queue = new Queue<ValueTuple<IRenderTexture, EntityUid, Direction, TaskCompletionSource<bool>>>();
	}
}
