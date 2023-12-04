using MonoGame.Entities;

namespace MonoGame.Decorators;

public class Friction : EntityDecorator
{
    public Friction(Entity @base, float restitutionCoefficient) : base(@base)
    {
        RestitutionCoefficient = restitutionCoefficient;
    }
}