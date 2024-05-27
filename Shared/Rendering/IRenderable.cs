using Shared.View;

namespace Shared.Rendering;

public interface IRenderable
{
    void Render(IRenderer renderer, Camera camera);
}