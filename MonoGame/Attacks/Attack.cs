using MonoGame.Entities;

namespace MonoGame.Attacks;

public abstract class Attack
{
    private Attack _next;
    private float _time;

    protected abstract float Duration { get; }

    protected Attack()
    {
        _next = null;
        _time = 0f;
    }

    public Attack Then(Attack next)
    {
        if (_next == null)
            _next = next;
        else
            _next.Then(next);
        
        return this;
    }

    public bool Execute(Entity context, float deltaTime)
    {
        _time += deltaTime;

        if (_time - Duration >= deltaTime) 
            return _next?.Execute(context, _time - deltaTime * 2 > Duration ? deltaTime : _time - Duration) ?? true;
        
        OnExecute(context, _time < Duration ? deltaTime : deltaTime - (_time - Duration));
        return false;
    }

    protected abstract void OnExecute(Entity context, float deltaTime);
}