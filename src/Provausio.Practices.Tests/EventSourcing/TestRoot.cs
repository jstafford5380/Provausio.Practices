using System;
using Provausio.Practices.EventSourcing.Aggregate;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class TestRoot : AggregateRoot
    {
        private TestEntity _entity;

        public string Prop1 { get; private set; }
        public int Prop2 { get; private set; }
        public bool Action1Occured { get; private set; }
        public string EntityProp => _entity?.Prop1;

        public static TestRoot Create(string prop1, int prop2)
        {
            var test = new TestRoot();
            test.Raise<CreationEvent>(e =>
            {
                e.Id = Guid.NewGuid();
                e.Prop1 = prop1;
                e.Prop2 = prop2;
            });

            return test;
        }

        public void Action1()
        {
            Raise<Action1Occured>(e => { Console.Out.WriteLine("Action 1 occured"); });
        }

        public void CreateEntity(string prop1)
        {
            Raise<EntityCreated>(c => { c.Prop1 = prop1; });
        }

        public void UpdateEntity(string prop1)
        {
            _entity.Update(prop1);
        }

        protected override void RegisterHandlers()
        {
            RegisterHandler<CreationEvent>(Apply);
            RegisterHandler<Action1Occured>(Apply);
            RegisterHandler<EntityCreated>(Apply);
        }

        private void Apply(CreationEvent e)
        {
            Id = e.RootId;
            Prop1 = e.Prop1;
            Prop2 = e.Prop2;
        }

        private void Apply(Action1Occured e)
        {
            Action1Occured = true;
        }

        private void Apply(EntityCreated e)
        {
            _entity = CreateEntity<TestEntity>();
            _entity.Prop1 = e.Prop1;
            _entity.Id = e.Id.ToString();
        }
    }

    public class TestEntity : Entity
    {
        public string Prop1 { get; set; }

        public void Update(string prop1)
        {
            Root.Raise<EntityUpdated>(updated => updated.Prop1 = prop1);
        }

        private void Apply(EntityUpdated e)
        {
            Prop1 = e.Prop1;
        }

        public override void RegisterHandlers(AggregateRoot root)
        {
            root.RegisterHandler<EntityUpdated>(Apply);
        }
    }

    public class EntityCreated : AggregateEvent
    {
        public string Prop1 { get; set; }
    }

    public class EntityUpdated : AggregateEvent
    {
        public string Prop1 { get; set; }
    }

    public class CreationEvent : AggregateEvent
    {
        public string Prop1 { get; set; }
        public int Prop2 { get; set; }
    }

    public class Action1Occured : AggregateEvent
    {

    }
}
