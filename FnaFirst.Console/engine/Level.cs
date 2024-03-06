using core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace engine
{
	public class Level
	{
		string[] tile_map;
		int player_x;
		int player_y;
		int width_tiles;
		int height_tiles;
		int width_pixels;
		int height_pixels;
		int tile_size;
		List<int[]> enemy_positions;
		List<Tile> tiles;
		public Level(string[] tile_map, int tile_size)
		{
			this.tile_map = tile_map;
			this.tile_size = tile_size;
			width_tiles = tile_map[0].Length - 1;
			height_tiles = tile_map.Length - 1;
			width_pixels = width_tiles * tile_size;
			height_pixels = height_pixels * tile_size;
			tiles = new List<Tile>();
			enemy_positions = new List<int[]>();
			var tile_index = 0;
			for (int row = 0; row < height_tiles; row++)
			{
				for (int column = 0; column < width_tiles; column++)
				{
					var tile_char = tile_at_coordinate(column, row);

					Tile? tile = tile_char
					switch
					{
						'#' => new Tile
						{
							color = Color.AntiqueWhite,
							geometry = new Rectangle(column * tile_size, row * tile_size, tile_size, tile_size)
						},

						_ => null
					};

					if (tile_char == 'v')
					{
						enemy_positions.Add(new int[] { column, row });
					}

					if (tile_char == 'o')
					{
						player_x = column;
						player_y = row;
					}

					if (tile != null)
					{
						tiles.Add(tile);
					}

					tile_index++;
				}
			}
		}

		char tile_at_coordinate(int column, int row) => tile_map[row][column];

		public void draw_tiles(SpriteBatch batch)
		{
			foreach (var tile in tiles)
			{
				batch.FillRectangle(tile.geometry, tile.color);
			}
		}
		public bool is_out_of_bounds(int column, int row) => column < 0 || row < 0 || width_tiles <= column || height_tiles <= row;
		public bool has_collision(int column, int row, char? match_char)
		{
			// we are treating out of bounds as a collision
			if (is_out_of_bounds(column, row))
			{
				return true;
			}

			char match_with = match_char ?? ' ';
			return tile_at_coordinate(column, row) == match_with;
		}
	}

	public class Tile
	{
		public Rectangle geometry { get; set; }
		public Color color { get; set; }
	}

	public class Data
	{
		public static string[] tile_map =
		{
			"##############################################################################",
			"#                                                                            #",
			"#                                                                            #",
			"#                                                                            #",
			"#                         ###        #  #  #  #   ###                        #",
			"#                                                                            #",
			"#               ########                               ########              #",
			"#                                      v                           # v       #",
			"#                                    ##########                    #####     #",
			"#     #####           v         v    v                                       #",
			"#               ##   ###       ###                        v                  #",
			"#                 #       ###                    ###      #                  #",
			"#                  #                                                 v       #",
			"#               ######               v                              ######## #",
			"#                              ################                              #",
			"#                                                      ########              #",
			"#                v           #                     #               #         #",
			"#               #######  #                                    #    #         #",
			"#                                                       #          ###       #",
			"##                        ###  o        v        ###                        ##",
			"#          ########################################################          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"##        ##                                                      ##        ##",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #                                                      #          #",
			"#          #          v           v             v          v      #          #",
			"##        ##########################################################        ##",
			"#                                                                            #",
			"##############################################################################",
		};
	}
}
