namespace HearThis.Script
{
	public class ScriptLine
	{
		public string Text;
		public bool Bold;
		public bool Centered;
		public int FontSize;
		public string FontName;
		public string Verse;

		public ScriptLine()
		{

		}
		public ScriptLine(string text)
		{
			Text = text;
		}
	}
}