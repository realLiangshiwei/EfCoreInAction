﻿// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_DeleteOneToOne
    {
        private readonly ITestOutputHelper _output;

        public Ch09_DeleteOneToOne(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var track = new TrackedEntity {OneToOne = new TrackedOne()};
                context.Add(track);
                var notify = new NotifyEntity {OneToOne = new NotifyOne()};
                context.Add(notify);
                context.SaveChanges();

                //VERIFY
                context.Tracked.Count().ShouldEqual(1);
                context.Notify.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeleteOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new TrackedEntity { OneToOne = new TrackedOne() };
                context.Add(track);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Tracked.Include(x => x.OneToOne).Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.Tracked.Count().ShouldEqual(0);
                context.Set<TrackedOne>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeleteRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var notify = new NotifyEntity { OneToOne = new NotifyOne() };
                context.Add(notify);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Notify.Include(x => x.OneToOne).Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.Notify.Count().ShouldEqual(0);
                context.Set<NotifyOne>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new TrackedEntity { OneToOne = new TrackedOne() };
                context.Add(track);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.Tracked.Include(x => x.OneToOne).Single();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var notify = new NotifyEntity { OneToOne = new NotifyOne() };
                context.Add(notify);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = context.Notify.Include(x => x.OneToOne).Single();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteRequiredWithNewOneToOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var notify = new NotifyEntity();
                context.Add(notify);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = context.Notify.Single();
                entity.OneToOne = new NotifyOne();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }
    }
}