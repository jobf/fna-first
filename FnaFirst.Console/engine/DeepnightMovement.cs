namespace engine
{

	public class DeepnightMovement
	{
		public Position position;
		public Velocity velocity;
		public Size size;

		// velocity.delta_y is incremented by this each frame
		public float gravity = 0.05f;

		Func<int, int, bool> has_wall_tile_at;
		// delegate bool has_wall_tile_at(int column, int row);
		protected bool is_wall_left = false;
		protected bool is_wall_right = false;
		protected bool is_wall_up = false;
		protected bool is_wall_down = false;

		public DeepnightMovement(int column, int row, int tile_size, Func<int, int, bool> has_wall_tile_at)
		{
			this.has_wall_tile_at = has_wall_tile_at;
			var cell_ratio = 0.5f;

			position = new Position
			{
				column = column,
				row = row,
				cell_ratio_x = cell_ratio,
				cell_ratio_y = cell_ratio,
				x = (column + cell_ratio) * tile_size,
				y = (row + cell_ratio) * tile_size,
			};

			velocity = new Velocity
			{
				delta_x = 0.0f,
				delta_y = 0.0f,
				friction_x = 0.0f,
				friction_y = 0.0f,
			};

			size = new Size
			{
				tile_size = tile_size,
				radius = tile_size / 2
			};
		}

		public void update()
		{
			update_movement_horizontal();
			update_movement_vertical();
			update_neighbours();
			update_gravity();
			update_collision();
			update_position();
		}

		protected void update_movement_vertical()
		{
			position.cell_ratio_y += velocity.delta_y;
			velocity.delta_y *= (1.0f - velocity.friction_y);
		}

		protected void update_movement_horizontal()
		{
			position.cell_ratio_x += velocity.delta_x;
			velocity.delta_x *= (1.0f - velocity.friction_x);
		}


		protected void update_gravity()
		{
			velocity.delta_y += gravity;
		}

		protected void update_neighbours()
		{
			is_wall_left = has_wall_tile_at(position.column + 1, position.row);
			is_wall_right = has_wall_tile_at(position.column - 1, position.row);
			is_wall_up = has_wall_tile_at(position.column, position.row - 1);
			is_wall_down = has_wall_tile_at(position.column, position.row + 1);
		}

		protected void update_collision()
		{
			// Right collision
			if (position.cell_ratio_x >= 0.7 && is_wall_right)
			{
				position.cell_ratio_x = 0.7f; // clamp position
				velocity.delta_x = 0; // stop horizontal movement
			}

			// Left collision
			if (position.cell_ratio_x <= 0.3 && is_wall_left)
			{
				position.cell_ratio_x = 0.3f; // clamp position
				velocity.delta_x = 0; // stop horizontal movement
			}

			// Ceiling collision
			if (position.cell_ratio_y < 0.2 && is_wall_up)
			{
				position.cell_ratio_y = 0.2f; // clamp position
				velocity.delta_y = 0; // stop vertical movement
			}

			// Floor collision
			if (position.cell_ratio_y >= 0.5 && is_wall_down)
			{
				position.cell_ratio_y = 0.5f; // clamp position
				velocity.delta_y = 0; // stop vertical movement
			}

		}

		protected void update_position()
		{
			// advance position.grid position if crossing edge
			while (position.cell_ratio_x > 1)
			{
				position.cell_ratio_x--;
				position.column++;
			}
			while (position.cell_ratio_x < 0)
			{
				position.cell_ratio_x++;
				position.column--;
			}

			// resulting position
			position.x = (float)Math.Floor((position.column + position.cell_ratio_x) * size.tile_size);

			// advance position.grid position if crossing edge
			while (position.cell_ratio_y > 1)
			{
				position.row++;
				position.cell_ratio_y--;
			}
			while (position.cell_ratio_y < 0)
			{
				position.row--;
				position.cell_ratio_y++;
			}

			// resulting position
			position.y = (float)Math.Floor((position.row + position.cell_ratio_y) * size.tile_size);
		}

	}

	public class Position
	{
		// tile map coordinates
		public int column;
		public int row;

		// ratios are 0.0 to 1.0  (position inside grid cell)
		public float cell_ratio_x;
		public float cell_ratio_y;

		// resulting pixel coordinates
		public float x;
		public float y;
	}

	public class Velocity
	{
		// deltas applied to grid cell ratio each frame
		public float delta_x;
		public float delta_y;

		// friction applied each frame 
		// 0.0 for no friction, e.g. no reduction
		// 1.0 for maximum, e.g. full reduction
		public float friction_x;
		public float friction_y;
	}

	public class Size
	{
		public int tile_size;
		public float radius;
	}

}