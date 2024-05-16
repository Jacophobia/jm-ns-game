using System.Collections.Generic;
using System.Threading.Tasks;
using OverWorld.GameObjects;
using OverWorld.GeneralActions;
using OverWorld.Interactions;

namespace OverWorld.Partition;

public class LayeredGrid
{
    private readonly List<List<IGameObject>> _layers;

    public LayeredGrid()
    {
        _layers = new List<List<IGameObject>>();
    }

    public void Add(IGameObject gameObject)
    {
        _layers[gameObject.Layer].Add(gameObject);
    }

    public void Remove(IGameObject gameObject)
    {
        Remove(gameObject.Layer, _layers[gameObject.Layer].IndexOf(gameObject));
    }

    public void Apply(ICollection<GeneralAction> actions, ICollection<Interaction> interactions)
    {
        for (var layer = 0; layer < _layers.Count; layer++)
        {
            for (var i = 0; i < _layers[layer].Count; i++)
            {
                foreach (var action in actions)
                {
                    action.Apply(_layers[layer][i]);
                }
                for (var j = i + 1; j < _layers[layer].Count; j++)
                {
                    foreach (var interaction in interactions)
                    {
                        interaction.Apply(_layers[layer][i], _layers[layer][j]);

                        if (_layers[layer][j].Layer != layer)
                        {
                            Move(layer, j);
                        }
                    }
                }

                if (_layers[layer][i].Layer != layer)
                {
                    Move(layer, i);
                }
            }
        }
    }

    public void Apply(ICollection<GeneralAction> actions, Interaction interaction)
    {
        Apply(actions, new[] { interaction });
    }

    public void Apply(GeneralAction action, ICollection<Interaction> interactions)
    {
        Apply(new[] { action }, interactions);
    }

    public void Apply(GeneralAction action, Interaction interaction)
    {
        Apply(new[] { action }, new[] { interaction });
    }

    public Task SaveAsync(string filepath)
    {
        throw new System.NotImplementedException();
    }

    public Task LoadAsync(string filepath)
    {
        throw new System.NotImplementedException();
    }

    private void Move(int layer, int objectIndex)
    {
        var gameObject = _layers[layer][objectIndex];

        var lastIndex = _layers[layer].Count - 1;

        (_layers[layer][objectIndex], _layers[layer][lastIndex]) = (_layers[layer][lastIndex], _layers[layer][objectIndex]);
                
        _layers[layer].RemoveAt(lastIndex);
        
        _layers[gameObject.Layer].Add(gameObject);
    }

    private void Remove(int layer, int objectIndex)
    {
        var lastIndex = _layers[layer].Count - 1;

        (_layers[layer][objectIndex], _layers[layer][lastIndex]) = (_layers[layer][lastIndex], _layers[layer][objectIndex]);
                
        _layers[layer].RemoveAt(lastIndex);
    }
}