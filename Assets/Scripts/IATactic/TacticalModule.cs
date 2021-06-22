using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TacticalModule
{
  protected internal abstract List<Accion> getStrategyActions(List<PersonajeNPC> npcs);
}
