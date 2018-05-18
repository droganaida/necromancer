public class Level03 : LevelBase {

	public Level03 ()  {
		data = new int[,] {
			{1,1,20},	// new Wave
			{1,2,10},
			{2,1,30},	// new Wave
			{2,2,18},
			{3,1,50},	// new Wave
			{3,2,30},
			{4,1,40},	// new Wave
			{4,2,20}
		};

		base.init ();
	}
}
