using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace us;

public class Character
{
	public int Id { get; set; }
	public DateTime? Created { get; set; }

	public string Name { get; set; }
	public string Status { get; set; }
	public string Species { get; set; }
	public string Type { get; set; }
	public string Gender { get; set; }
	public string Image { get; set; }

	[JsonProperty("episode")]
	public IEnumerable<string> Episodes { get; set; }

	[JsonIgnore]
	public IEnumerable<CharacterEpisode> Episode { get; set; }

	public CharacterSubData Location { get; set; } = new();
	public CharacterSubData Origin { get; set; } = new();
}

public class CharacterSubData
{
	public string Name { get; set; }
	public string Url { get; set; }
}

public class CharacterEpisode
{
	public string Name { get; set; }
}
