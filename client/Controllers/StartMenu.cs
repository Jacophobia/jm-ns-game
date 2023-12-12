using Microsoft.Xna.Framework.Graphics;
using MonoGame.Controllers;
using MonoGame.Orchestration;
using MonoGame.Output;
using MonoGame.Players;

namespace client.Controllers;

public class StartMenu : GameController
{
    private PageManager _pageManager;
    
    public StartMenu() : base(false)
    {
    }

    protected override void OnLoadContent()
    {
        var font = Content.Load<SpriteFont>("path/to/font");
        var buttonTexture = Content.Load<Texture2D>("path/to/button_texture");
        var checkboxTexture = Content.Load<Texture2D>("path/to/checkbox_texture");
        var sliderTexture = Content.Load<Texture2D>("path/to/slider_texture");
        var sliderThumbTexture = Content.Load<Texture2D>("path/to/slider_thumb_texture");
        var controllerTexture = Content.Load<Texture2D>("path/to/controller_texture");

        var menuFactory = new MenuFactory(font, buttonTexture, checkboxTexture, sliderTexture, sliderThumbTexture, controllerTexture);
        _pageManager = menuFactory.CreateMenu(new Host(new Camera(), Renderer), new []{ "character one", "character two" });
        _pageManager.SwitchToPage("Landing");
    }

    protected override void OnUpdate(float deltaTime)
    {
        _pageManager.Update();
    }

    protected override void OnDraw(float deltaTime)
    {
        _pageManager.Draw();
    }
}