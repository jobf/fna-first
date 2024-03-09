﻿using System;
using Core;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

			game.Run();
		}
	}
}

public struct PlayerData
{
	public Vector2 Position;
	public bool IsAlive;
	public Color Color;
	public float Angle;
	public float Power;
}

class Main : Game
{
	private Texture2D background_texture;
	private Texture2D foreground_texture;
	private Texture2D carriage_texture;
	private Texture2D cannon_texture;
	private Texture2D rocket_texture;
	private Texture2D smoke_texture;
	private SpriteBatch sprite_batch;
	private int screen_width;
	private int screen_height;
	private float player_scaling;
	private PlayerData[] players;
	private int number_of_players = 4;
	private int current_player = 0;
	private Color[] player_colors = new Color[10]
	{
		  Color.Red,
		  Color.Green,
		  Color.Blue,
		  Color.Purple,
		  Color.Orange,
		  Color.Indigo,
		  Color.Yellow,
		  Color.SaddleBrown,
		  Color.Tomato,
		  Color.Turquoise
	};

	private bool rocket_flying = false;
	private Vector2 rocket_position;
	private Vector2 rocket_direction;
	private float rocket_angle;
	private float rocket_scaling = 0.1f;
	private List<Vector2> smoke_list = new List<Vector2>();
	private Random randomizer = new Random();
	private int[] terrain_contour;

	protected override void LoadContent()
	{
		background_texture = Content.Load<Texture2D>("background.jpg");
		carriage_texture = Content.Load<Texture2D>("carriage.png");
		cannon_texture = Content.Load<Texture2D>("cannon.png");
		rocket_texture = Content.Load<Texture2D>("rocket.png");
		smoke_texture = Content.Load<Texture2D>("smoke.png");

		sprite_batch = new SpriteBatch(GraphicsDevice);

		screen_width = GraphicsDevice.PresentationParameters.BackBufferWidth;
		screen_height = GraphicsDevice.PresentationParameters.BackBufferHeight;


		player_scaling = 40.0f / (float)carriage_texture.Width;
		set_up_players();

		generate_terrain_contour();
		create_foreground();
	}

	void draw_scenery()
	{
		Rectangle screenRectangle = new Rectangle(0, 0, screen_width, screen_height);
		sprite_batch.Draw(background_texture, screenRectangle, Color.White);
		sprite_batch.Draw(foreground_texture, screenRectangle, Color.White);
	}

	void draw_players()
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].IsAlive)
			{
				var x_pos = (int)players[i].Position.X;
				var y_pos = (int)players[i].Position.Y;
				var cannon_origin = new Vector2(11, 50);

				// by setting source rect as null we say that all the image should be used
				Nullable<Rectangle> source_rect = null;
				var tint = players[i].Color;
				var rotate = 0;
				var origin = new Vector2(0, carriage_texture.Height);
				var scale = player_scaling;
				var effect = SpriteEffects.None;
				var layer = 0;
				sprite_batch.Draw(carriage_texture, players[i].Position, source_rect, tint, rotate, origin, scale, effect, layer);
				sprite_batch.Draw(cannon_texture, new Vector2(x_pos + 20, y_pos - 10), source_rect, tint, players[i].Angle, cannon_origin, player_scaling, effect, layer);
			}
		}
	}

	void draw_rocket()
	{
		if (rocket_flying)
		{
			sprite_batch.Draw(rocket_texture, rocket_position, null, players[current_player].Color, rocket_angle, new Vector2(42, 240), rocket_scaling, SpriteEffects.None, 1);
		}
	}

	private void draw_smoke()
	{
		for (int i = 0; i < smoke_list.Count; i++)
		{
			sprite_batch.Draw(smoke_texture, smoke_list[i], null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
		}
	}
	void process_keyboard()
	{
		KeyboardState keybState = Keyboard.GetState();

		// rotate cannons anti-clockwise or clockwise 
		if (keybState.IsKeyDown(Keys.Left))
		{
			players[current_player].Angle -= 0.01f;
		}
		if (keybState.IsKeyDown(Keys.Right))
		{
			players[current_player].Angle += 0.01f;
		}

		// prevent rotation from hitting floor
		if (players[current_player].Angle > MathHelper.PiOver2)
		{
			players[current_player].Angle = -MathHelper.PiOver2;
		}
		if (players[current_player].Angle < -MathHelper.PiOver2)
		{
			players[current_player].Angle = MathHelper.PiOver2;
		}

		// set power
		if (keybState.IsKeyDown(Keys.Down))
		{
			players[current_player].Power -= 1;
		}
		if (keybState.IsKeyDown(Keys.Up))
		{
			players[current_player].Power += 1;
		}
		if (keybState.IsKeyDown(Keys.PageDown))
		{
			players[current_player].Power -= 20;
		}
		if (keybState.IsKeyDown(Keys.PageUp))
		{
			players[current_player].Power += 20;
		}

		if (players[current_player].Power > 1000)
		{
			players[current_player].Power = 1000;
		}
		if (players[current_player].Power < 0)
		{
			players[current_player].Power = 0;
		}

		// launch rocket
		if (keybState.IsKeyDown(Keys.Enter) || keybState.IsKeyDown(Keys.Space))
		{
			rocket_flying = true;
			rocket_position = players[current_player].Position;
			rocket_position.X += 20;
			rocket_position.Y -= 10;
			rocket_angle = players[current_player].Angle;

			// define the Up vector which is along the negative Y axis
			var up = new Vector2(0, -1);

			// rotate the Up vector with rotation matrix
			var rot_matrix = Matrix.CreateRotationZ(rocket_angle);
			rocket_direction = Vector2.Transform(up, rot_matrix);
			rocket_direction *= players[current_player].Power / 50.0f;
		}
	}

	void set_up_players()
	{
		players = new PlayerData[number_of_players];
		for (int i = 0; i < number_of_players; i++)
		{
			players[i].IsAlive = true;
			players[i].Color = player_colors[i];
			players[i].Angle = MathHelper.ToRadians(90);
			players[i].Power = 100;
		}

		players[0].Position = new Vector2(100, 193);
		players[1].Position = new Vector2(200, 212);
		players[2].Position = new Vector2(300, 361);
		players[3].Position = new Vector2(400, 164);
	}

	void generate_terrain_contour()
	{
		terrain_contour = new int[screen_width];

		double rand1 = randomizer.NextDouble() + 1;
		double rand2 = randomizer.NextDouble() + 2;
		double rand3 = randomizer.NextDouble() + 3;

		float offset = screen_height / 2;
		float peak_height = 100;
		float flatness = 70;

		for (int x = 0; x < screen_width; x++)
		{
			double height = peak_height / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
			height += peak_height / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
			height += peak_height / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
			height += offset;
			terrain_contour[x] = (int)height;
		}
	}

	private void create_foreground()
	{
		var foreground_colors = new Color[screen_width * screen_height];

		for (int x = 0; x < screen_width; x++)
		{
			for (int y = 0; y < screen_height; y++)
			{
				if (y > terrain_contour[x])
				{
					foreground_colors[x + y * screen_width] = Color.Green;
				}
				else
				{
					foreground_colors[x + y * screen_width] = Color.Transparent;
				}
			}
		}

		foreground_texture = new Texture2D(GraphicsDevice, screen_width, screen_height, false, SurfaceFormat.Color);
		foreground_texture.SetData(foreground_colors);
	}

	public Main()
	{
		Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		Window.Title = "Missile MonoGame Tutorial";

		base.Initialize();
	}

	private void update_rocket()
	{
		if (rocket_flying)
		{

			Vector2 gravity = new Vector2(0, 1);
			rocket_direction += gravity / 10.0f;
			rocket_position += rocket_direction;
			rocket_angle = (float)Math.Atan2(rocket_direction.X, -rocket_direction.Y);

			for (int i = 0; i < 5; i++)
			{
				Vector2 smokePos = rocket_position;
				smokePos.X += randomizer.Next(10) - 5;
				smokePos.Y += randomizer.Next(10) - 5;
				smoke_list.Add(smokePos);
			}
			if (rocket_position.Y > 600)
			{
				rocket_flying = false;
				smoke_list = new List<Vector2>();
			}
		}
	}

	protected override void Update(GameTime gameTime)
	{
		process_keyboard();
		update_rocket();
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		sprite_batch.Begin();
		draw_scenery();
		draw_players();
		draw_rocket();
		draw_smoke();
		sprite_batch.End();

		base.Draw(gameTime);
	}

}