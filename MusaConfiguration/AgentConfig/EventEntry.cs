﻿using System.Xml.Serialization;
using System.Collections.Generic;

namespace MusaConfiguration
{
	public class EventEntry
	{
		[XmlAttribute("Formula", typeof(string))]
		public string formula { get; set; }

		[XmlAttribute("Plan", typeof(string))]
		public string plan { get; set; }

        [XmlAttribute("Perception", typeof(string))]
		public string perception { get; set; }

		[XmlArray("EventArgs")]
		[XmlArrayItem("EventArg", typeof(EventArgEntry))]
		public List<EventArgEntry> EventArgs { get; set; }

		public EventEntry()
		{
			EventArgs = new List<EventArgEntry> ();
		}

        public override string ToString()
        {
            return string.Format("[EventEntry: formula={0}, plan={1}, perception={2}]", formula, plan, perception);
        }
        
	}
}

