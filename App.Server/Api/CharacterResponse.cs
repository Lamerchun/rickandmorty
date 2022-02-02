using System.Collections.Generic;

namespace us
{
	public partial class RickAndMortyApiController
	{
		public class CharacterResponse
		{
			public CharacterResponseInfo Info { get; set; }
			public List<Character> Results { get; set; }
		}
	}

	public class CharacterResponseInfo
	{
		public int Count { get; set; }
		public int Pages { get; set; }

		public string Prev { get; set; }
		public string Next { get; set; }
	}
}
