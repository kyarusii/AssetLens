using NUnit.Framework;

public class ForceFailedTest
{
	[SetUp]
	public void SetupTests()
	{
        
	}
    
	[Test]
	public void MustBeFailed()
	{
		Assert.False(true);
	}
}