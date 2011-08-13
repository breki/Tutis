using System;
using MbUnit.Framework;
using Rhino.Mocks;
using TreasureChest.Policies;
using TreasureChest.Policies.ServicePolicies;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ComponentCreatorTests
    {
        [Test]
        public void WhenInstanceIsCreated()
        {
            IConstructionPolicy constructionPolicy = MockRepository.GenerateStub<IConstructionPolicy>();
            object instance = "something";
            constructionPolicy.Stub(x => x.CreateInstance(registration, resolvingContext))
                .Return(instance);

            IAfterComponentCreatedAction afterComponentCreatedAction
                = MockRepository.GenerateMock<IAfterComponentCreatedAction>();
            afterComponentCreatedAction.Expect(x => x.AfterCreated(instance));
            registration.AddPolicy(afterComponentCreatedAction);

            chestPolicies.AddPolicy(constructionPolicy);

            creator
                .CreateInstance(registration, resolvingContext);

            afterComponentCreatedAction.VerifyAllExpectations();
        }

        [SetUp]
        private void Setup()
        {
            chestPolicies = new PolicyCollection();
            creator = new ComponentCreator(chestPolicies, new NullLogger());
            dependencyGraph = new ObjectDependencyGraph(chestPolicies);
            registration = new ServiceRegistration();
            resolvingContext = new ResolvingContext(dependencyGraph, typeof(IServiceX));
        }

        private PolicyCollection chestPolicies;
        private ComponentCreator creator;
        private ObjectDependencyGraph dependencyGraph;
        private ServiceRegistration registration;
        private ResolvingContext resolvingContext;
    }
}