using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Common.FSM {
    public abstract class State <EntityType> {
        //Destroy the instance
        abstract public void Dispose ();


        abstract public void Enter (EntityType entity);

        abstract public void Exit (EntityType entity);

        abstract public void Execute (EntityType entity);
    }
}
