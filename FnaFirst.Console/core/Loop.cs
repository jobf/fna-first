using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace core
{

	class Loop : Game
	{
		private float t = 0.0f;
		private float dt = 1.0f / 30.0f;
		private float step_time_accumulator = 0.0f;
		private float step_ratio = 0.0f;

		protected List<FixedStepObject> systems = new List<FixedStepObject>();
		protected SpriteBatch batch;

		protected override void LoadContent()
		{
			batch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			batch.Dispose();
		}

		protected override void Update(GameTime gameTime)
		{

			step_time_accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

			while (step_time_accumulator >= dt)
			{

				foreach (var system in systems)
				{
					system.update();
				}

				base.Update(new GameTime(TimeSpan.FromSeconds(t), TimeSpan.FromSeconds(dt)));
				t += dt;
				step_time_accumulator -= dt;
			}

			step_ratio = step_time_accumulator / dt;
		}
	}

	interface FixedStepObject
	{
		public void update();
		public void draw(float step_ratio);
	}
	// class FixedStepLoop : Game
	// {
	// 	// private float t = 0.0f;
	// 	// private float dt = 1.0f / 60.0f;
	// 	// private float step_time_accumulator = 0.0f;
	// 	// private float step_ratio = 0.0f;

	// 	protected List<FixedStepObject> systems = new List<FixedStepObject>();
	// 	protected SpriteBatch batch;

	// 	public FixedStepLoop(){
	// 		IsFixedTimeStep = true;
			
	// 	}

	// 	protected override void LoadContent()
	// 	{
	// 		batch = new SpriteBatch(GraphicsDevice);
	// 	}

	// 	protected override void UnloadContent()
	// 	{
	// 		batch.Dispose();
	// 	}

	// 	protected override void Update(GameTime gameTime)
	// 	{
	// 		base.Update(gameTime);
	// 		gameTime.
	// 	}

	// 	protected override void Draw(GameTime gameTime)
	// 	{
	// 		GraphicsDevice.Clear(Color.Black);

	// 		batch.Begin();

	// 		foreach (var system in systems)
	// 		{
	// 			system.draw(step_ratio);
	// 		}

	// 		batch.End();

	// 		base.Draw(gameTime);
	// 	}
	// }

}



