using MonoGame.Entities;

namespace MonoGame.Decorators;

public class Drag : EntityDecorator
{
    public Drag(Entity @base, float restitutionCoefficient) : base(@base)
    {
        RestitutionCoefficient = restitutionCoefficient;
    }
}