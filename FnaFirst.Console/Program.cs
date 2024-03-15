using System;
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

public struct ParticleData
{
	public float BirthTime;
	public float MaxAge;
	public Vector2 OriginalPosition;
	public Vector2 Acceleration;
	public Vector2 Direction;
	public Vector2 Position;
	public float Scaling;
	public Color ModColor;
}

class Main : Game
{
	private Texture2D background_texture;
	private Texture2D foreground_texture;
	private Texture2D carriage_texture;
	private Texture2D cannon_texture;
	private Texture2D rocket_texture;
	private Texture2D smoke_texture;
	private Texture2D ground_texture;
	private Texture2D explosion_texture;
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
	private Color[,] rocket_color_grid;
	private Color[,] _foregroundColorArray;
	private Color[,] carriage_color_grid;
	private Color[,] cannon_color_grid;

	private bool rocket_flying = false;
	private Vector2 rocket_position;
	private Vector2 rocket_direction;
	private float rocket_angle;
	private float rocket_scaling = 0.1f;
	private List<Vector2> smoke_list = new List<Vector2>();
	private Random randomizer = new Random();
	private int[] terrain_contour;
	List<ParticleData> particle_list = new List<ParticleData>();

	protected override void LoadContent()
	{
		background_texture = Content.Load<Texture2D>("background.jpg");
		carriage_texture = Content.Load<Texture2D>("carriage.png");
		cannon_texture = Content.Load<Texture2D>("cannon.png");
		rocket_texture = Content.Load<Texture2D>("rocket.png");
		smoke_texture = Content.Load<Texture2D>("smoke.png");
		ground_texture = Content.Load<Texture2D>("ground");
		explosion_texture = Content.Load<Texture2D>("explosion");

		sprite_batch = new SpriteBatch(GraphicsDevice);

		screen_width = GraphicsDevice.PresentationParameters.BackBufferWidth;
		screen_height = GraphicsDevice.PresentationParameters.BackBufferHeight;


		player_scaling = 40.0f / (float)carriage_texture.Width;

		generate_terrain_contour();
		set_up_players();
		flatten_terrain_beneath_players();
		create_foreground();

		rocket_color_grid = texture_to_2D_array(rocket_texture);
		carriage_color_grid = texture_to_2D_array(carriage_texture);
		cannon_color_grid = texture_to_2D_array(cannon_texture);
	}

	private Color[,] texture_to_2D_array(Texture2D texture)
	{

		// extract color data (pixels) from a Texture2D, to a flat array
		Color[] colors = new Color[texture.Width * texture.Height];
		texture.GetData(colors);

		// put colors into a 2D array
		Color[,] color_grid = new Color[texture.Width, texture.Height];
		for (int x = 0; x < texture.Width; x++)
		{
			for (int y = 0; y < texture.Height; y++)
			{
				color_grid[x, y] = colors[x + y * texture.Width];
			}
		}

		return color_grid;
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
	private void draw_explosion()
	{
		for (int i = 0; i < particle_list.Count; i++)
		{
			ParticleData particle = particle_list[i];
			sprite_batch.Draw(explosion_texture, particle.Position, null, particle.ModColor, i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
		}
	}

	private void add_explosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
	{
		for (int i = 0; i < numberOfParticles; i++)
		{
			add_explosion_particle(explosionPos, size, maxAge, gameTime);
		}
	}

	private void add_explosion_particle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
	{
		ParticleData particle = new ParticleData();

		particle.OriginalPosition = explosionPos;
		particle.Position = particle.OriginalPosition;

		particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
		particle.MaxAge = maxAge;
		particle.Scaling = 0.25f;
		particle.ModColor = Color.White;

		float particleDistance = (float)randomizer.NextDouble() * explosionSize;
		Vector2 displacement = new Vector2(particleDistance, 0);
		float angle = MathHelper.ToRadians(randomizer.Next(360));
		displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

		particle.Direction = displacement * 2.0f;
		particle.Acceleration = -particle.Direction;
		particle_list.Add(particle);
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
			int x = screen_width / (number_of_players + 1) * (i + 1);
			players[i].Position = new Vector2
			{
				X = x,
				Y = terrain_contour[x]
			};
		}

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

	private void flatten_terrain_beneath_players()
	{
		foreach (var player in players)
		{
			if (player.IsAlive)
			{
				for (int x = 0; x < 40; x++)
				{
					terrain_contour[(int)player.Position.X + x] = terrain_contour[(int)player.Position.X];
				}
			}
		}
	}

	private void create_foreground()
	{
		Color[,] ground_color_grid = texture_to_2D_array(ground_texture);
		var foreground_colors = new Color[screen_width * screen_height];

		for (int x = 0; x < screen_width; x++)
		{
			for (int y = 0; y < screen_height; y++)
			{
				if (y > terrain_contour[x])
				{
					// use modulo to wrap texture lookup positions within ground texture boundaries
					var lookup_x = x % ground_texture.Width;
					var lookup_y = y % ground_texture.Height;
					foreground_colors[x + y * screen_width] = ground_color_grid[lookup_x, lookup_y];
				}
				else
				{
					foreground_colors[x + y * screen_width] = Color.Transparent;
				}
			}
		}

		foreground_texture = new Texture2D(GraphicsDevice, screen_width, screen_height, false, SurfaceFormat.Color);
		foreground_texture.SetData(foreground_colors);
		_foregroundColorArray = texture_to_2D_array(foreground_texture);
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

	private void update_particles(GameTime gameTime)
	{
		float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
		for (int i = particle_list.Count - 1; i >= 0; i--)
		{
			ParticleData particle = particle_list[i];
			float time_alive = now - particle.BirthTime;

			if (time_alive > particle.MaxAge)
			{
				// particle is expired, remove it
				particle_list.RemoveAt(i);
			}
			else
			{
				// particle is active, progress it's lifespan
				float relative_age = time_alive / particle.MaxAge;
				particle.Position = 0.5f * particle.Acceleration * relative_age * relative_age + particle.Direction * relative_age + particle.OriginalPosition;
				float invAge = 1.0f - relative_age;
				particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));
				Vector2 positions_from_center = particle.Position - particle.OriginalPosition;
				float distance = positions_from_center.Length();
				particle.Scaling = (50.0f + distance) / 200.0f;
				particle_list[i] = particle;
			}
		}
	}


	private Vector2 textures_collide(Color[,] texture_a, Matrix transform_a, Color[,] texture_b, Matrix transform_b)
	{

		// todo - pass smaller textures in rather than check every time
		Color[,] tex_a;
		Color[,] tex_b;
		Matrix mat_a;
		Matrix mat_b;

		var is_smallest_a = texture_a.Length < texture_b.Length;
		if (is_smallest_a)
		{
			tex_a = texture_a;
			mat_a = transform_a;
			tex_b = texture_b;
			mat_b = transform_b;
		}
		else
		{
			tex_a = texture_b;
			mat_a = transform_b;
			tex_b = texture_a;
			mat_b = transform_a;

		}
		var matrix_a_to_b = mat_a * Matrix.Invert(mat_b);
		int width_a = tex_a.GetLength(0);
		int height_a = tex_a.GetLength(1);
		int width_b = tex_b.GetLength(0);
		int height_b = tex_b.GetLength(1);

		for (int x_a = 0; x_a < width_a; x_a++)
		{
			for (int y_a = 0; y_a < height_a; y_a++)
			{
				Vector2 pos_a = new Vector2(x_a, y_a);
				Vector2 pos_b = Vector2.Transform(pos_a, matrix_a_to_b);

				int x_b = (int)pos_b.X;
				int y_b = (int)pos_b.Y;

				if ((x_b >= 0) && (x_b < width_b))
				{
					if ((y_b >= 0) && (y_b < height_b))
					{
						if (tex_a[x_a, y_a].A > 0)
						{
							if (tex_b[x_b, y_b].A > 0)
							{
								return Vector2.Transform(pos_a, mat_a);
							}
						}
					}
				}
			}
		}

		return new Vector2(-1, -1);
	}

	private Vector2 check_terrain_collision()
	{
		Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) *
								 Matrix.CreateRotationZ(rocket_angle) *
								 Matrix.CreateScale(rocket_scaling) *
								 Matrix.CreateTranslation(rocket_position.X, rocket_position.Y, 0);
		Matrix terrainMat = Matrix.Identity;
		Vector2 terrainCollisionPoint = textures_collide(rocket_color_grid, rocketMat, _foregroundColorArray, terrainMat);
		return terrainCollisionPoint;
	}

	private Vector2 check_players_collision()
	{
		Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) *
								  Matrix.CreateRotationZ(rocket_angle) *
								  Matrix.CreateScale(rocket_scaling) *
								  Matrix.CreateTranslation(rocket_position.X, rocket_position.Y, 0);

		for (int i = 0; i < number_of_players; i++)
		{
			PlayerData player = players[i];
			if (player.IsAlive)
			{
				if (i != current_player)
				{
					int xPos = (int)player.Position.X;
					int yPos = (int)player.Position.Y;

					Matrix carriageMat = Matrix.CreateTranslation(0, -carriage_texture.Height, 0) *
													Matrix.CreateScale(player_scaling) *
													Matrix.CreateTranslation(xPos, yPos, 0);
					Vector2 carriageCollisionPoint = textures_collide(carriage_color_grid, carriageMat, rocket_color_grid, rocketMat);
				}
			}
		}
		return new Vector2(-1, -1);
	}

	private bool check_out_of_screen()
	{
		bool rocketOutOfScreen = rocket_position.Y > screen_height;
		rocketOutOfScreen |= rocket_position.X < 0;
		rocketOutOfScreen |= rocket_position.X > screen_width;

		return rocketOutOfScreen;
	}

	private void check_collisions(GameTime gameTime)
	{

		// todo : reduce collisions by checking if outlines collide

		Vector2 terrain_collision_point = check_terrain_collision();
		Vector2 player_collision_point = check_players_collision();
		bool rocket_out_of_screen = check_out_of_screen();

		if (player_collision_point.X > -1)
		{
			rocket_flying = false;

			smoke_list = new List<Vector2>();
			add_explosion(player_collision_point, 10, 80.0f, 2000.0f, gameTime);
			next_player();
		}

		if (terrain_collision_point.X > -1)
		{
			rocket_flying = false;

			smoke_list = new List<Vector2>();
			add_explosion(terrain_collision_point, 4, 30.0f, 1000.0f, gameTime);
			next_player();
		}

		if (rocket_out_of_screen)
		{
			rocket_flying = false;

			smoke_list = new List<Vector2>();
			next_player();
		}
	}

	void next_player()
	{
		current_player = current_player + 1;
		current_player = current_player % number_of_players;
		while (!players[current_player].IsAlive)
		{
			current_player = ++current_player % number_of_players;
		}
	}

	protected override void Update(GameTime gameTime)
	{
		// prevent keyboard when rocket or particles are in progress
		// to ensure one turn has really ended before the next turn
		if (!rocket_flying && particle_list.Count == 0)
		{
			process_keyboard();
		}

		if (rocket_flying)
		{
			update_rocket();
			check_collisions(gameTime);
		}

		if (particle_list.Count > 0)
		{
			update_particles(gameTime);
		}

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

		sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
		draw_explosion();
		sprite_batch.End();

		base.Draw(gameTime);
	}

}