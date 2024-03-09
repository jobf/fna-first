using System;
using Core;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Num = System.Numerics;

static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		using (var game = new Main())
		{
			var graphics = new GraphicsDeviceManager(game)
			{
				PreferredBackBufferWidth = 500,
				PreferredBackBufferHeight = 500,
				IsFullScreen = false
			};

			// graphics.ApplyChanges();
			game.Run();
		}
	}
}

class Main : Game
{
	private Texture2D background_texture;
	private SpriteBatch sprite_batch;
	private int screen_width;
	private int screen_height;

	// private GraphicsDeviceManager graphics;

	protected override void LoadContent()
	{
		// base.LoadContent();
		// device = GraphicsDevice;

		background_texture = Content.Load<Texture2D>("background.jpg");
		sprite_batch = new SpriteBatch(GraphicsDevice);

		screen_width = GraphicsDevice.PresentationParameters.BackBufferWidth;
		screen_height = GraphicsDevice.PresentationParameters.BackBufferHeight;
	}

	void draw_scenery()
	{
		Rectangle screenRectangle = new Rectangle(0, 0, screen_width, screen_height);
		sprite_batch.Draw(background_texture, screenRectangle, Color.White);
	}

	public Main()
	{
		// graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		Window.Title = "Missile MonoGame Tutorial";

		base.Initialize();
	}


	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		sprite_batch.Begin();
		draw_scenery();
		sprite_batch.End();

		base.Draw(gameTime);
	}

}