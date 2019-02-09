using System;
using Xunit;


namespace Log4Net.RedisStream.Test
{
    public class AppenderTests
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, 4);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void MyFirstTheory(int value)
        {
            Assert.True(value % 2 == 1);
        }
    }
}
