using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace engine
{
	public class Sprite
	{
		public Texture2D texture;
		public Vector2 position;
		public Color color;

		public void draw(SpriteBatch batch)
		{
			batch.Draw(texture, position, color);
		}
	}

}
