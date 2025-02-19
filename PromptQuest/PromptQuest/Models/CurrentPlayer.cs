namespace PromptQuest.Models
{
	public static class CurrentPlayer
	{
		// container for the player model, the controller will override and pull this
		public static PlayerModel _player;

		// override's the stored player
		public static void SetPlayer(PlayerModel player) 
		{
			_player = player;
		}

		// grabs the current player
		public static PlayerModel GetPlayer() 
		{
			return _player;
		}
	}
}
