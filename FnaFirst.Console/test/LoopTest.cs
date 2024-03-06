using core;
using engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class LoopTest : Loop
{
	private TestObject test_object;
	private Level level;

	protected override void LoadContent()
	{
		base.LoadContent();
		test_object = new TestObject(batch);
		systems.Add(test_object);

		level = new Level(Data.tile_map, 32);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		batch.Begin();

		level.draw_tiles(batch);

		batch.End();

		base.Draw(gameTime);
	}

}

class TestObject : FixedStepObject
{
	private float position_x = 0;
	private float position_y = 0;

	private float position_previous_x = 0;
	private float position_previous_y = 0;
	private SpriteBatch batch;
	private Rectangle graphic_rect;

	public TestObject(SpriteBatch batch)
	{
		this.batch = batch;
		graphic_rect = new Rectangle((int)position_x, (int)position_y, 32, 32);
	}

	public void draw(float step_ratio)
	{
		graphic_rect.X = (int)Math.Ceiling(MathHelper.Lerp(position_previous_x, position_x, step_ratio));
		graphic_rect.Y = (int)MathHelper.Lerp(position_previous_y, position_y, step_ratio);
		batch.DrawRectangle(graphic_rect, Color.Beige);
	}

	public void update()
	{
		position_previous_x = position_x;
		position_previous_y = position_y;

		position_x += 1.2f;
	}
}