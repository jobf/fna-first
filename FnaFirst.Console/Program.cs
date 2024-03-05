using Microsoft.Xna.Framework;

static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		using (var g = new ImGuiTest())
		// using (var g = new PrimitivesTest())
		{
			new GraphicsDeviceManager(g);
			g.Run();
		}
	}
}