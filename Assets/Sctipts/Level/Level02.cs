public class Level02 : LevelBase {

	public Level02 ()  {
		data = new int[,] {
			{1,1,10},	// new Wave
			{1,2,3},
			{2,1,20},	// new Wave
			{2,2,6},
			{3,1,30},	// new Wave
			{3,2,13}
		};

		base.init ();
	}
}
