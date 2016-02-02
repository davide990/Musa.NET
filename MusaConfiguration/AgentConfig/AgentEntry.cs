using System.Xml.Serialization;
using System.Collections.Generic;

namespace MusaConfiguration
{
	public class AgentEntry
	{
        [XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlArray("Plans")]
		[XmlArrayItem("Plan", typeof(string))]
		public List<string> Plans { get; set; }

		[XmlArray("BeliefBase")]
		[XmlArrayItem("Belief", typeof(string))]
		public List<string> BeliefBase { get; set; }

		[XmlArray("Events")]
		[XmlArrayItem("Event", typeof(EventEntry))]
		public List<EventEntry> Events { get; set; }

		public AgentEntry ()
		{
			Plans = new List<string>();
			BeliefBase = new List<string>();
			Events = new List<EventEntry> ();
		}

	}
}

