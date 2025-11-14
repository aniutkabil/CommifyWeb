namespace Tams.TestAutomation.Common.Core.Attributes
{
	[AttributeUsage(AttributeTargets.All)]
	public class TitlesAttribute(params string[] titles) : Attribute
	{
		public List<string> Titles { get; private set; } = [.. titles];
	}
}
