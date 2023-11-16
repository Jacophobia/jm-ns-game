using System;
using Microsoft.Xna.Framework;

namespace client.Controllers;

public class HostingClient : Game
{
    protected override void OnActivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);
    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
        base.OnDeactivated(sender, args);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void BeginRun()
    {
        base.BeginRun();
    }

    protected override void EndRun()
    {
        base.EndRun();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override bool BeginDraw()
    {
        return base.BeginDraw();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    protected override void EndDraw()
    {
        base.EndDraw();
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        base.OnExiting(sender, args);
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}