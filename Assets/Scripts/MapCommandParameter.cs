using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EscapeHorror.Prototype { 
    public interface ICommand {
        void Execute(Dictionary<MapEventData.Event, string> param);
    }
    public class MapCommandParameter : MonoBehaviour
    {
        [SerializeField]
        ICommand[] commands;

        public ICommand[] GetCommands(Type type)
        {
            var hit = (ICommand[])commands.Where(c => c.GetType() == type);
            return hit;
        }
    }
}