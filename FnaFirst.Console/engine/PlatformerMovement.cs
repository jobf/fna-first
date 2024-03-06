namespace engine
{
	public class PlatformerMovement : DeepnightMovement
	{
		float velocity_ascent;
		float velocity_descent;
		float gravity_ascent;
		float gravity_descent;
		int buffer_step_count_remaining = 0;
		int coyote_steps_remaining = 0;
		bool is_jump_in_progress = false;
		bool is_on_ground = true;

		private JumpConfig jump_config;

		public PlatformerMovement(int column, int row, int tile_size, Func<int, int, bool> has_wall_tile_at) : base(column, row, tile_size, has_wall_tile_at)
		{
			jump_config = new JumpConfig();

			// y velocity is determined by jump velocity and gravity so set friction to 0
			velocity.friction_y = 0;


			// calculate gravity
			gravity_ascent = -(-2.0f * jump_config.height_tiles_max / (jump_config.ascent_step_count * jump_config.ascent_step_count));
			gravity_descent = -(-2.0f * jump_config.height_tiles_max / (jump_config.descent_step_count * jump_config.descent_step_count));

			// calculate velocity
			velocity_ascent = -((2.0f * jump_config.height_tiles_max) / jump_config.ascent_step_count);
			velocity_descent = (float)Math.Sqrt(2 * gravity_descent * jump_config.height_tiles_min);
		}

		/** called from jump button or key press **/
		public void press_jump()
		{
			// jump ascent phase can start if we are on the ground or coyote time did not finish
			var is_within_coyote_time = coyote_steps_remaining > 0;
			if (is_on_ground || is_within_coyote_time)
			{
				ascend();
			}
			else
			{
				// if jump was pressed but could not be performed begin jump buffer
				buffer_step_count_remaining = jump_config.buffer_step_count;
			}
		}

		/** called from jump button or key release **/
		public void release_jump()
		{
			descend();
		}

		/** begin jump ascent phase **/
		void ascend()
		{
			// set ascent velocity
			velocity.delta_y = velocity_ascent;

			// if we are in ascent phase then jump is in progress
			is_jump_in_progress = true;

			// reset coyote time because we left the ground with a jump
			coyote_steps_remaining = 0;
		}

		/** begin jump descent phase **/
		void descend()
		{
			// set descent velocity
			velocity.delta_y = velocity_descent;
		}

		new void update()
		{
			/* 
						most of the update logic for the movement is called from the super class
						however we also perform extra jump logic
					*/


			// jump logic
			//------------

			// count down every step
			coyote_steps_remaining--;
			buffer_step_count_remaining--;

			if (is_on_ground)
			{
				// if we are on the ground then a jump is not in progress or has finished
				is_jump_in_progress = false;

				// reset coyote step counter every step that we are on the ground
				coyote_steps_remaining = jump_config.coyote_step_count;

				// jump ascent phase can be triggered if we are on the ground and jump buffer is in progress
				if (buffer_step_count_remaining > 0)
				{
					// trigger jump ascent phase
					ascend();
					// reset jump step counter because jump buffer has now ended
					buffer_step_count_remaining = 0;
				}
			}


			// movement logic
			//----------------

			// change position within grid cell by velocity
			update_movement_horizontal();
			update_movement_vertical();

			// check for adjacent tiles
			update_neighbours();

			// override gravity logic so use different values during jump
			override_update_gravity();

			// stop movement if colliding with a tile
			update_collision();
			// if delta_y is 0 and there is a wall tile below then movement stopped
			// because we collided with the ground
			is_on_ground = velocity.delta_y == 0 && is_wall_down;

			// update position within grid and cell
			update_position();
		}

		void override_update_gravity()
		{
			if (is_jump_in_progress)
			{
				// gravity has different values depending on jump phase
				// ascent phase if delta_y is negative (moving towards ceiling)
				// descent phase if delta_y is positive (moving towards floor)
				velocity.delta_y += velocity.delta_y <= 0 ? gravity_ascent : gravity_descent;
			}
			else
			{
				// use default gravity when jump is not in progress
				velocity.delta_y += gravity;
			}
		}

	}

	class JumpConfig
	{
		public float height_tiles_max = 7.0f;
		public float height_tiles_min = 2.5f;
		public int ascent_step_count = 20;
		public int descent_step_count = 12;
		public int buffer_step_count = 15;
		public int coyote_step_count = 5;
	}
}