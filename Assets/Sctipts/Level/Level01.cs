public class Level01 : LevelBase {

	public Level01 ()  {
		data = new int[,] {
			{1,1,7},	// new Wave
			{1,2,3},
			{2,1,16},	// new Wave
			{2,2,6}
		};

		base.init ();
	}
}