using AutoFixture;
using AutoFixture.Xunit2;
using Turing.Data.Entities;
using System;

namespace Turing.Business.Tests
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute()
            : base(() => new Fixture().Customize(new Customization()))
        {
        }

        private class Customization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                fixture.Customize<User>(composer =>
                    composer.With(u => u.Id, Guid.NewGuid().ToString()));
            }
        }
    }
}
