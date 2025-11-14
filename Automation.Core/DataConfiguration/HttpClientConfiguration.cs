namespace Automation.Core.DataConfiguration
{
	public class HttpClientConfiguration : BaseConfiguration
	{
		public Dictionary<string, Uri> Endpoints { get; set; } = [];
	}
}
