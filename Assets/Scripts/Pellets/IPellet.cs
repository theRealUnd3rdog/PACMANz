using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPellet
{
    void Collect();
}

public interface IScoreablePellet : IPellet
{
    
}

public interface IAbilityPellet : IPellet
{
    void ActivateAbility();
}

