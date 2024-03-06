using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace engine
{
	class Actor
	{
		Sprite sprite;
		PlatformerMovement movement;

		float position_x_previous;
		float position_y_previous;

		float acceleration_x = 0.15f;

		public int velocity_x = 0; // todo rename this direction?
		public int facing = 0;

		public float velocity_x_max = 0.62f;
		public float velocity_y_max = 0.7f;

		bool is_jumping = false;
		float jump_velocity = -0.85f;

		public Actor(Sprite sprite, int column, int row, int tile_size, Func<int, int, bool> has_wall_tile_at){
			this.sprite = sprite;
			movement = new PlatformerMovement(column, row, tile_size, has_wall_tile_at);
			position_x_previous = movement.position.x;
			position_y_previous = movement.position.y;
		}

		public void update()
		{
			if (velocity_x != 0)
			{
				// accelerate horizontally
				movement.velocity.delta_x += (velocity_x * acceleration_x);
			}

			// cap speed
			if (movement.velocity.delta_x > velocity_x_max)
			{
				movement.velocity.delta_x = velocity_x_max;
			}
			if (movement.velocity.delta_x < -velocity_x_max)
			{
				movement.velocity.delta_x = -velocity_x_max;
			}

			if (velocity_y_max > 0 && movement.velocity.delta_y > velocity_y_max)
			{
				movement.velocity.delta_y = velocity_y_max;
			}
			if (velocity_y_max < 0 && movement.velocity.delta_y < -velocity_y_max)
			{
				movement.velocity.delta_y = -velocity_y_max;
			}

			position_x_previous = movement.position.x;
			position_y_previous = movement.position.y;

			movement.update();
		}

		public void draw(float step_ratio)
		{
			sprite.position.X = MathHelper.Lerp(position_x_previous, movement.position.x, step_ratio);
			sprite.position.Y = MathHelper.Lerp(position_y_previous, movement.position.y, step_ratio);
		}

		public void change_velocity_x(int velocity)
		{
			facing = velocity;
			velocity_x = velocity;
		}

		public void stop_x()
		{
			velocity_x = 0;
		}

		public void jump()
		{
			movement.press_jump();
		}

		public void drop()
		{
			movement.release_jump();
		}
	}

}