﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Gameobjects;

namespace Detection
{
    /// <summary>
    /// Encounter showing multiple coherent combatcomponents across multiple ticks
    /// </summary>
    public class Encounter
    {
        /// <summary>
        /// Components which form this encounter
        /// </summary>
        public List<CombatComponent> cs;

        /// <summary>
        /// Tick in which this encounter was first built
        /// </summary>
        public int tick_id;

        /// <summary>
        /// Build a encounter with a componant
        /// </summary>
        /// <param name="comp"></param>
        public Encounter(CombatComponent comp)
        {
            this.tick_id = comp.tick_id;
            cs = new List<CombatComponent>();
            AddComponent(comp);
        }

        /// <summary>
        /// Build a encounter with a list of components -> use the tickid of the oldest component
        /// </summary>
        /// <param name="tick_id"></param>
        /// <param name="newcs"></param>
        public Encounter(List<CombatComponent> newcs)
        {
            int encounter_tick_id = newcs.OrderBy(x => x.tick_id).ElementAt(0).tick_id; //find oldest tickid(smallest)
            this.tick_id = encounter_tick_id;
            cs = newcs.OrderBy(x => x.tick_id).ToList();
            cs.AsParallel().ForAll(x => x.parent = this);
        }

        /// <summary>
        /// Update a encounter with a combatcomp (if comp is successor)
        /// </summary>
        /// <param name="update"></param>
        public void update(CombatComponent update)
        {
            AddComponent(update);
            cs = cs.OrderBy(x => x.tick_id).ToList();
        }

        /// <summary>
        /// Add a component to a encounter
        /// </summary>
        /// <param name="comp"></param>
        public void AddComponent(CombatComponent comp)
        {
            cs.Add(comp);
            comp.parent= this;
        }

        public int getLatestTick()
        {
            return cs.Max(c => c.tick_id);
        }

        public int getTickRange()
        {
            return cs.Max(c => c.tick_id) - cs.Min(c => c.tick_id);
        }

        public bool isDamageEncounter()
        {
            return cs.Any(component => component.links.Any( link => link.Impact > 0));
        }

        public bool isKillEncounter()
        {
            return cs.Any(component => component.links.Any(link => link.IsKill));
        }

        public int getEncounterKillEvents()
        {
            return cs.Sum(cs => cs.contained_kill_events);
        }

        public int getEncounterHurtEvents()
        {
            return cs.Sum(cs => cs.contained_hurt_events);
        }

        public int getEncounterSpottedEvents()
        {
            return cs.Sum(cs => cs.contained_spotted_events);
        }

        public int getEncounterWeaponfireEvents()
        {
            return cs.Sum(cs => cs.contained_weaponfire_events);
        }

        public int getParticipatingPlayerCount()
        {
            if (cs.Count == 0) throw new Exception("No components in encounter");
            List<Player> pplayers = new List<Player>();
            foreach(var c in cs)
            {
                if (c.players.Count == 10) return 10;
                foreach (var p in c.players)
                    if (!pplayers.Contains(p))
                        pplayers.Add(p);

            }
            var list = pplayers.ToList();
            list.RemoveAll( p => p.player_id == 0); //Remove bots from encounters
            var s = "";
            foreach(var p in pplayers)
            {
                s += p.ToString();
            }
            if (pplayers.Count > 10 || pplayers.Count == 0) throw new Exception("Too many or to few players in encounter: "+pplayers.Count+s);
            return pplayers.Count;
        }

        public override string ToString()
        {
            var s = "Encounter-TickID: " + tick_id + " Compcount: " + cs.Count;
            foreach (var c in cs)
            {
                s += c.ToString() + "\n";
            }
            return s;
        }

        override public bool Equals(object other)
        {
            var en = other as Encounter;
            if (en == null)
                return false;
            if (this.tick_id == en.tick_id)
                return true;

            /*var intersection = cs.Intersect(en.cs);

            if (intersection.Count() == cs.Count && intersection.Count() == en.cs.Count)
                return true;*/

            return false;
        }

        public override int GetHashCode()
        {
            return tick_id;
        }


    }
}
