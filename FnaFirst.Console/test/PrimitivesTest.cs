using core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class PrimitivesTest : Game
{
	private SpriteBatch batch;

	public PrimitivesTest()
	{
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		// Create the batch...
		batch = new SpriteBatch(GraphicsDevice);
	}

	protected override void UnloadContent()
	{
		batch.Dispose();
	}
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		batch.Begin();

		//DrawRectangle Examples
		batch.DrawRectangle(new Rectangle(100, 100, 100, 200), Color.Purple, 20);

		batch.DrawRectangle(new Rectangle(10, 10, 100, 200), Color.Yellow, 2);
		batch.DrawRectangle(new Rectangle(20, 20, 100, 200), Color.Yellow, 1);
		batch.DrawRectangle(new Vector2(30, 30), new Vector2(100, 200), Color.Yellow);
		batch.DrawRectangle(new Vector2(40, 40), new Vector2(100, 200), Color.Yellow, 5);

		//DrawCircle Examples
		batch.DrawCircle(400, 100, 90, 3, Color.White * 0.2f);
		batch.DrawCircle(400, 100, 90, 4, Color.White * 0.3f);
		batch.DrawCircle(400, 100, 90, 5, Color.White * 0.4f);
		batch.DrawCircle(400, 100, 90, 6, Color.White * 0.5f);
		batch.DrawCircle(400, 100, 90, 7, Color.White * 0.6f);
		batch.DrawCircle(400, 100, 90, 8, Color.White * 0.7f);
		batch.DrawCircle(400, 100, 90, 9, Color.White * 0.8f);
		batch.DrawCircle(400, 100, 90, 10, Color.White * 0.9f);
		batch.DrawCircle(400, 100, 90, 20, Color.Red);

		batch.DrawCircle(600, 100, 100, 50, Color.Green, 10);
		batch.DrawCircle(new Vector2(600, 100), 40, 30, Color.Green, 30);

		//FillRectangle Examples
		batch.FillRectangle(350, 340, 100, 100, Color.Red * 0.4f);
		batch.FillRectangle(new Rectangle(350, 340, 100, 100), Color.Red * 0.3f, (float)Math.PI / 4f);
		batch.FillRectangle(new Vector2(350, 340), new Vector2(100, 100), Color.Red * 0.2f, (float)Math.PI / 3f);
		batch.FillRectangle(350, 340, 100, 100, Color.Red * 0.5f, (float)Math.PI);

		//DrawArc Examples
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(180), Color.Navy, 1);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(160), Color.Navy * 0.9f, 2);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(140), Color.Navy * 0.8f, 4);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(120), Color.Navy * 0.7f, 6);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(90), Color.Navy * 0.6f, 8);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(80), Color.Navy * 0.5f, 10);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(65), Color.Navy * 0.4f, 12);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(45), Color.Navy * 0.3f, 14);
		batch.DrawArc(new Vector2(600, 350), 100, 180, MathHelper.ToRadians(180), MathHelper.ToRadians(12), Color.Navy * 0.2f, 16);

		batch.DrawArc(new Vector2(600, 400), 80, 90, MathHelper.ToRadians(90), MathHelper.ToRadians(220), Color.Navy, 10);

		//DrawLine Examples
		batch.DrawLine(new Vector2(420, 220), new Vector2(480, 280), Color.Orange);
		batch.DrawLine(new Vector2(420, 230), new Vector2(480, 290), Color.Orange, 2);
		batch.DrawLine(new Vector2(420, 240), new Vector2(480, 300), Color.Orange, 5);
		batch.DrawLine(new Vector2(420, 255), new Vector2(480, 315), Color.Orange, 10);

		batch.DrawLine(new Vector2(100, 400), 40.0f, MathHelper.ToRadians(270), Color.Green, 3);
		batch.DrawLine(new Vector2(103, 400), 40.0f, MathHelper.ToRadians(105), Color.Blue, 3);
		batch.DrawLine(new Vector2(101, 400), 40.0f, MathHelper.ToRadians(0), Color.Red, 3);

		//PutPixel Examples
		batch.PutPixel(new Vector2(250, 150), Color.Red);
		batch.PutPixel(new Vector2(251, 150), Color.Red);
		batch.PutPixel(new Vector2(251, 151), Color.Red);
		batch.PutPixel(new Vector2(250, 151), Color.Red);

		batch.PutPixel(new Vector2(253, 150), Color.Red);
		batch.PutPixel(new Vector2(254, 150), Color.Red);
		batch.PutPixel(new Vector2(254, 151), Color.Red);
		batch.PutPixel(new Vector2(253, 151), Color.Red);

		batch.End();

		base.Draw(gameTime);
	}
		
}