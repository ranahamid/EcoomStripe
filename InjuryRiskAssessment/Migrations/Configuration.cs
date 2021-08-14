namespace InjuryRiskAssessment.Migrations
{
    using SaasEcom.Core.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<InjuryRiskAssessment.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(InjuryRiskAssessment.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var professionalMonthly = new SubscriptionPlan
            {
                Id = "prod_GxL3HN8QrS7esM",
                Name = "MyPainRecorder",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 15,
                Price = 199.00,
                Currency = "USD"
            };
            professionalMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "10000"
            });

            var businessMonthly = new SubscriptionPlan
            {
                Id = "prod_GxL1MXCBSw5BGu",
                Name = "Telemedicine",
                Interval = SubscriptionPlan.SubscriptionInterval.Monthly,
                TrialPeriodInDays = 26,
                Price = 149,
                Currency = "USD"
            };
            businessMonthly.Properties.Add(new SubscriptionPlanProperty
            {
                Key = "MaxNotes",
                Value = "10000"
            });

            context.SubscriptionPlans.AddOrUpdate(
                sp => sp.Id,
            
                professionalMonthly,
                businessMonthly);
        }
    }
}
